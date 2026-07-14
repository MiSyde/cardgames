using Cards.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    partial class Blackjack : INotifyPropertyChanged
    {
        private BlackjackDealer _dealer;
        public BlackjackDealer Dealer => _dealer;
        // Hit, Stand, Double Down, Surrender, Insurance, Split 
        private Dictionary<int, List<Button>> _buttons;
        public Dictionary<int, List<Button>> Buttons => _buttons;
        private Dictionary<int, IHand> _hands;
        public Dictionary<int, IHand> Hands => _hands;

        public int playerCount;
        private ObservableCollection<int> _activeHands;
        private int _currentHandId;
        private int _nextHandId;
        private bool _joinable;
        public event PropertyChangedEventHandler? PropertyChanged;
        private int _playerCount;
        private bool _over;
        public bool Over
        {
            get => _over;
            set
            {
                if(_over != value)
                {
                    _over = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Over)));
                }
            }
        }
        public int PlayerCount
        {
            get => _playerCount;
            set { _playerCount = value; }
        }

        public bool Joinable => _joinable;
        public Blackjack() 
        { 
            _dealer = new BlackjackDealer(new Random(), new Deck()); 
            _hands = new();
            _buttons = new();
            _joinable = true;
            _currentHandId = 1;
            _nextHandId = 1;
            _activeHands = new();
            _over = false;
        }

        public void StartGame()
        {
            _joinable = false;

            _activeHands = new ObservableCollection<int>(_hands.Keys);
            _activeHands.CollectionChanged += PlayerExited;
            Over = false;
            _dealer.Draw();
            UpdateButtonsForHand(_currentHandId);
        }
        private void UpdateAllButtons()
        {
            foreach (var handId in _hands.Keys)
            {
                UpdateButtonsForHand(handId);
            }
        }

        private void UpdateButtonsForHand(int id)
        {
            if (!_buttons.TryGetValue(id, out List<Button>? buttons)) return;
            if (!_hands.TryGetValue(id, out _)) return;

            foreach (Button btn in buttons)
            {
                if (btn.Command is RelayCommand<int> command)
                {
                    command.NotifyCanExecuteChanged();
                }
            }
        }

        private List<Button> GenerateButtons(int handId)
        {
            List<Button> list = new();

            Button HitB = new();
            HitB.Content = "Hit";
            HitB.Command = new RelayCommand<int>(id => Hit(id), id => CanAct(id));
            HitB.CommandParameter = handId;
            list.Add(HitB);

            Button StandB = new();
            StandB.Content = "Stand";
            StandB.Command = new RelayCommand<int>(id => Stand(id), id => CanAct(id));
            StandB.CommandParameter = handId;
            list.Add(StandB);

            Button DoubleDownB = new();
            DoubleDownB.Content = "Double Down";
            DoubleDownB.Command = new RelayCommand<int>(id => DoubleDown(id), id => CanAct(id));
            DoubleDownB.CommandParameter = handId;
            list.Add(DoubleDownB);

            Button SurrenderB = new();
            SurrenderB.Content = "Surrender";
            SurrenderB.Command = new RelayCommand<int>(id => Surrender(id), id => CanSurrender(id));
            SurrenderB.CommandParameter = handId;
            list.Add(SurrenderB);

            Button InsuranceB = new();
            InsuranceB.Content = "Insurance";
            InsuranceB.Command = new RelayCommand<int>(id => Insurance(id), id => CanInsure(id));
            InsuranceB.CommandParameter = handId;
            list.Add(InsuranceB);

            Button SplitB = new();
            SplitB.Content = "Split";
            SplitB.Command = new RelayCommand<int>(id => Split(id), id => CanSplit(id));
            SplitB.CommandParameter = handId;
            list.Add(SplitB);

            return list;
        }
        private bool CanAct(int id)
        {
            if (!_hands.TryGetValue(id, out IHand? hand)) return false;
            return hand.IsActive && hand.Id == _currentHandId;
        }

        private bool CanInsure(int id)
        {
            if (!_hands.TryGetValue(id, out IHand? hand)) return false;
            if (_dealer.CurrentCards.Count != 1) return false;

            Card c = _dealer.CurrentCards.First();
            return c.IsFaceCard && c.FaceCardType is FaceCard.Ace && hand.IsActive && hand.Id == _currentHandId;
        }

        private bool CanSurrender(int id)
        {
            if (!_hands.TryGetValue(id, out IHand? hand)) return false;
            Generic? g = hand as Generic;
            return g.CurrentCards.Count == 2 && hand.IsActive && hand.Id == _currentHandId;
        }

        private void PlayerExited(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(_activeHands.Count() == 0)
            {
                _dealer.FinishDrawing();
                PlayerCount = 0;
                CheckWinners();
                Over = true;
            }
        }
        private bool CanSplit(int id)
        {
            if (!_hands.TryGetValue(id, out IHand? hand)) return false;
            if (hand.IsSplit || hand is not Player player) return false;

            return player.CanSplit() && player.Id == _currentHandId && player.IsActive;
        }
        private void CheckWinners()
        {
            if(_dealer.CardScore > 21)
            {
                foreach (IHand h in _hands.Values)
                {
                    if(h.IsSplit)
                    {
                        Split? s = h as Split;
                        if (s.Surrendered) continue;
                        s.Player.Chips += s.Bet * 2;
                    } 
                    else
                    {
                        Player? p = h as Player;
                        if (p.Surrendered) continue;
                        p.Chips += p.Bet * 2;
                    }
                }
            }
            else if(_dealer.TwoCardBlackjack)
            {
                PushAll();
            } 
            else if(_dealer.CardScore == 21)
            {
                int dealerCardCount = _dealer.CurrentCards.Count;
                foreach(IHand h in _hands.Values)
                {
                    if(h.IsSplit)
                    {
                        Split? s = h as Split;
                        if (s.Surrendered) continue;
                        if(s.CardScore == 21)
                        {
                            int splitCardCount = s.CurrentCards.Count;
                            if (dealerCardCount > splitCardCount)
                            {
                                _dealer.Chips += s.Bet;
                                s.Bet = 0;
                            }
                            else
                            {
                                Push(s);
                            }
                        }
                    }
                    else
                    {
                        Player? p = h as Player;
                        if (p.Surrendered) continue;
                        if (p.CardScore == 21)
                        {
                            int splitCardCount = p.CurrentCards.Count;
                            if (dealerCardCount > splitCardCount)
                            {
                                _dealer.Chips += p.Bet;
                                p.Bet = 0;
                            }
                            else
                            {
                                Push(p);
                            }
                        }
                    }
                }
            } 
            else
            {
                foreach (IHand h in _hands.Values)
                {
                    if(h.IsSplit)
                    {
                        Split? s = h as Split;
                        if (s.Surrendered) continue;
                        if (s.CardScore > _dealer.CardScore && s.CardScore <= 21)
                        {
                            s.Player.Chips += s.Bet * 2;
                        }
                        else if (s.CardScore == _dealer.CardScore && s.CurrentCards.Count < _dealer.CurrentCards.Count)
                        {
                            s.Player.Chips += s.Bet * 2;
                        }
                        else
                        {
                            Push(s);
                        }
                    } 
                    else
                    {
                        Player? p = h as Player;
                        if (p.Surrendered) continue;
                        if (p.CardScore > _dealer.CardScore && p.CardScore <= 21)
                        {
                            p.Chips += p.Bet * 2;
                        }
                        else if (p.CardScore == _dealer.CardScore && p.CurrentCards.Count < _dealer.CurrentCards.Count)
                        {
                            p.Chips += p.Bet * 2;
                        }
                        else
                        {
                            Push(p);
                        }
                    }
                }
            }
        }

        // Döntetlen, játékos visszakapja a tétjét
        private void Push(Generic g)
        {
            if(g is Player)
            {
                Player? p = g as Player;
                if (p.TwoCardBlackjack)
                {
                    p.Chips += p.Bet;
                }
                else
                {
                    _dealer.Chips += p.Bet;
                }
                p.Bet = 0;
            } else
            {
                Split? s = g as Split;
                Player p = s.Player;
                if (s.TwoCardBlackjack)
                {
                    p.Chips += s.Bet;
                }
                else
                {
                    _dealer.Chips += s.Bet;
                }
                s.Bet = 0;
            }
        }
        private void PushAll()
        {
            foreach (IHand h in _hands.Values)
            {
                if(h.IsSplit)
                {
                    Split? s = h as Split;
                    if (s.Surrendered) continue;
                    if(s.Insured)
                    {
                        s.Player.Chips += s.InsuredBet;
                        s.InsuredBet = 0;
                        s.Bet = 0;
                        continue;
                    }
                    if (s.TwoCardBlackjack)
                    {
                        s.Player.Chips += s.Bet;
                    }
                    else
                    {
                        _dealer.Chips += s.Bet;
                    }
                    s.Bet = 0;
                }
                else
                {
                    Player? p = h as Player;
                    if (p.Surrendered) continue;
                    if (p.Insured)
                    {
                        p.Chips += p.InsuredBet;
                        p.InsuredBet = 0;
                        p.Bet = 0;
                        continue;
                    }
                    if (p.TwoCardBlackjack)
                    {
                        p.Chips += p.Bet;
                    }
                    else
                    {
                        _dealer.Chips += p.Bet;
                    }
                    p.Bet = 0;
                }
            }
        }

        private IHand? GetHand(int id)
        {
            _hands.TryGetValue(id, out IHand? hand);
            return hand;
        }

        private void DealCard(int id)
        {
            IHand? currentHand = GetHand(id);
            if (currentHand == null || !currentHand.IsActive) return;
            Card newCard = _dealer.Deal();
            if (currentHand.IsSplit)
            {
                Split? current = currentHand as Split;
                current?.CurrentCards.Add(newCard);
            } 
            else
            {
                Player? current = currentHand as Player;
                current?.CurrentCards.Add(newCard);
            }
        }

        public void Hit(int id) 
        {
            DealCard(id);
            AdvancePlayerIdx(id);
            UpdateButtonsForHand(id);
        }

        public void DoubleDown(int id) 
        {
            DealCard(id);
            Stand(id);
            AdvancePlayerIdx(id);
            UpdateButtonsForHand(id);
        }

        public void Split(int id)
        {
            IHand? hand = GetHand(id);
            if (hand == null || hand.IsSplit) return;

            Player? p = hand as Player;
            Card c = p.CurrentCards[1];
            p.CurrentCards.RemoveAt(1);

            int splitId = _nextHandId++;
            Split newSplit = new(c, id, p);
            _hands[splitId] = newSplit;
            _buttons[splitId] = GenerateButtons(id);
            newSplit.Buttons = _buttons[splitId];
            p.SplitId = splitId;
            int currentIndex = _activeHands.IndexOf(id);
            if (currentIndex >= 0)
            {
                _activeHands.Insert(currentIndex + 1, splitId);
            }
            else
            {
                _activeHands.Add(splitId);
            }
            UpdateButtonsForHand(id);
            UpdateButtonsForHand(splitId);
            PlayerCount++;
        }

        public void Insurance(int id)
        {
            IHand? currentHand = GetHand(id);
            if(currentHand.IsSplit && currentHand is Split split)
            {
                split.Insured = true;
                int insurance;
                if ((int)Math.Round(split.Bet * 1.5) > split.Player.Chips) 
                {
                    insurance = split.Player.Chips;
                }
                else
                {
                    insurance = (int)Math.Round(split.Bet * 1.5); 
                }
                split.InsuredBet = insurance;
                split.Player.Chips -= insurance;
            } 
            else
            {
                Player? player = currentHand as Player;
                player.Insured = true;
                int insurance;
                if ((int)Math.Round(player.Bet * 1.5) > player.Chips)
                {
                    insurance = player.Chips;
                }
                else
                {
                    insurance = (int)Math.Round(player.Bet * 1.5);
                }
                player.InsuredBet = insurance;
                player.Chips -= insurance;
            }
        }

        public void Surrender(int id)
        {
            IHand? currentHand = GetHand(id);
            if (currentHand.IsSplit)
            {
                Split? current = currentHand as Split;
                current.IsActive = false;
                int halfBet = (int)Math.Round((double)current.Bet / 2);
                current.Player.Chips += halfBet;
                current.Surrendered = true;
                _dealer.Chips += halfBet;
            }
            else
            {
                Player? current = currentHand as Player;
                current.IsActive = false;
                int halfBet = (int)Math.Round((double)current.Bet / 2);
                current.Chips += halfBet;
                current.Surrendered = true;
                _dealer.Chips += halfBet;
            }
            AdvancePlayerIdx(id);
        }

        public void Stand(int id)
        {
            IHand? currentHand = GetHand(id);
            if (currentHand.IsSplit)
            {
                Split? current = currentHand as Split;
                current.IsActive = false;
            }
            else
            {
                Player? current = currentHand as Player;
                current.IsActive = false;
            }
            AdvancePlayerIdx(id);
        }

        public int AddPlayer()
        {
            int id = _nextHandId++;
            List<Button> actionButtons = GenerateButtons(id);
            Player newPlayer = new(id);
            _hands[id] = newPlayer;
            _activeHands.Add(id);
            _buttons[id] = actionButtons;
            newPlayer.Buttons = actionButtons;
            PlayerCount++;
            return id;
        }

        // If the player busted/got 21/stood with their last action, they get removed from the game and the index points to the next active player
        private void AdvancePlayerIdx(int id)
        {
            IHand? currentHand = GetHand(id);
            if (currentHand == null) return;
            int currentIndex = _activeHands.IndexOf(_currentHandId);
            if (!currentHand.IsActive)
            {
                _activeHands.Remove(id);
                
            }
            if (_activeHands.Count == 1 || _activeHands.Count == 0)
            {
                UpdateAllButtons();
                return;
            }
            if(!currentHand.IsSplit && currentHand is Player player)
            {
                if(player.DidSplit && player.SplitId != -1)
                {
                    int splitId = player.SplitId;

                    if(_hands.TryGetValue(splitId, out IHand? splitHand) && splitHand.IsActive)
                    {
                        _currentHandId = splitId;
                        UpdateAllButtons();
                        return;
                    }
                    else
                    {
                        _activeHands.Remove(splitId);
                    }
                } 
            }
            if(!_activeHands.Contains(_currentHandId))
            {
                _currentHandId = _activeHands[currentIndex]; // currentIndex's hand got deleted, so in it's place is now the next available hand's id
                UpdateAllButtons();
                return;
            }

            _currentHandId = _activeHands[currentIndex + 1];
            
            UpdateAllButtons();
        }
    }
}
