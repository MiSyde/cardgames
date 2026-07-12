using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace Cards
{
    public class Blackjack
    {
        private BlackjackDealer dealer;
        // Hit, Stand, Double Down, Surrender, Insurance, Split 
        private Dictionary<int, List<Button>> buttons;
        public Dictionary<int, List<Button>> Buttons { get { return buttons; } }
        private List<Player> players;
        public int PlayerCount { get { return players.Count; } }
        private ObservableCollection<int> currentPlayers;
        private List<int> allPlayers;
        private int playerIdx;
        private bool joinable;
        public bool Joinable { get { return joinable; } }
        public Blackjack() 
        { 
            dealer = new BlackjackDealer(new Random(), new Deck()); 
            players = new(); 
            currentPlayers = new(); 
            currentPlayers.CollectionChanged += PlayerExited;
            allPlayers = new();
            buttons = new();
            joinable = true;
        }

        private List<Button> GenerateButtons()
        {
            Button StandB = new();
            StandB.Content = "Stand";
            StandB.Command = new RelayCommand(Stand, () => StandB.Name == playerIdx.ToString());
            Button HitB = new();
            HitB.Content = "Hit";
            HitB.Command = new RelayCommand(Hit, () => HitB.Name == playerIdx.ToString());
            Button SurrenderB = new();
            SurrenderB.Content = "Surrender";
            SurrenderB.Command = new RelayCommand(Surrender, () => SurrenderB.Name == playerIdx.ToString());
            Button InsuranceB = new();
            InsuranceB.Content = "Insurance";
            InsuranceB.Command = new RelayCommand(Insurance, () => InsuranceB.Name == playerIdx.ToString());
            Button DoubleDownB = new();
            DoubleDownB.Content = "Double Down";
            DoubleDownB.Command = new RelayCommand(DoubleDown, () => DoubleDownB.Name == playerIdx.ToString());
            Button SplitB = new();
            SplitB.Content = "Split";
            SplitB.Command = new RelayCommand(Split, () => SplitB.Name == playerIdx.ToString());
            List<Button> list = new();
            list.Add(HitB);
            list.Add(StandB);
            list.Add(DoubleDownB);
            list.Add(SurrenderB);
            list.Add(InsuranceB);
            list.Add(SplitB);
            return list;
        }

        private void PlayerExited(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(currentPlayers.Count() == 0)
            {
                dealer.FinishDrawing();
                CheckWinners();
            }
        }

        private void CheckWinners()
        {
            if(dealer.CardScore > 21)
            {
                foreach (Player p in players)
                {
                    if(p.Split != null)
                    {
                        p.Chips += p.Split.Bet * 2;
                    }
                    p.Chips += p.Bet * 2;
                }
            }
            else if(dealer.TwoCardBlackjack)
            {
                PushAll();
            } else if(dealer.CardScore == 21)
            {
                int dealerCardCount = dealer.CurrentCards.Count;
                foreach(Player p in players)
                {
                    if(p.CardScore == 21)
                    {
                        int playerCardCount = p.CurrentCards.Count;
                        if (dealerCardCount > playerCardCount)
                        {
                            dealer.Chips += p.Bet;
                            p.Bet = 0;
                        }
                        else
                        {
                            Push(p);
                        }
                    }
                    if(p.Split != null)
                    {
                        if (p.Split.CardScore == 21)
                        {
                            int playerSplitCardCount = p.Split.CurrentCards.Count;
                            if (dealerCardCount > playerSplitCardCount)
                            {
                                dealer.Chips += p.Bet;
                                p.Bet = 0;
                            }
                            else
                            {
                                Push(p.Split);
                            }
                        }
                    }
                }
            } else
            {
                foreach(Player p in players)
                {
                    if(p.CardScore > dealer.CardScore && p.CardScore <= 21)
                    {
                        p.Chips += p.Bet * 2;
                    }
                    else if(p.CardScore == dealer.CardScore && p.CurrentCards.Count < dealer.CurrentCards.Count)
                    {
                        p.Chips += p.Bet * 2;
                    } else
                    {
                        Push(p);
                    }
                }
            }
        }

        // Döntetlen, játékos visszakapja a tétjét
        private void Push(Generic g)
        {
            if(g is Player)
            {
                Player p = g as Player;
                if (p.TwoCardBlackjack)
                {
                    p.Chips += p.Bet;
                }
                else
                {
                    dealer.Chips += p.Bet;
                }
                p.Bet = 0;
            } else
            {
                Split s = g as Split;
                Player p = s.Player;
                if (s.TwoCardBlackjack)
                {
                    p.Chips += s.Bet;
                }
                else
                {
                    dealer.Chips += s.Bet;
                }
                s.Bet = 0;
            }
        }
        private void PushAll()
        {
            foreach (Player p in players)
            {
                if (p.TwoCardBlackjack)
                {
                    p.Chips += p.Bet;
                }
                else
                {
                    dealer.Chips += p.Bet;
                }
                if (p.Split != null)
                {
                    if (p.Split.TwoCardBlackjack)
                    {
                        p.Chips += p.Split.Bet;
                    }
                    else
                    {
                        dealer.Chips += p.Split.Bet;
                    }
                    p.Split.Bet = 0;
                }
                p.Bet = 0;
            }
        }

        private void DealCard()
        {
            Player current = players.ElementAt(playerIdx);
            Card newCard = dealer.Deal();
            current.CurrentCards.Add(newCard);
        }

        public void Hit() 
        {
            DealCard();
            AdvancePlayerIdx();
        }

        public void DoubleDown() 
        {
            DealCard();
            Stand();
            AdvancePlayerIdx();
        }

        public void Split()
        {
            Player p = players.ElementAt(playerIdx);
            if(allPlayers.Contains(playerIdx+1))
            {
                for(int i = playerIdx+1; i < currentPlayers.Count; ++i)
                {
                    int x = i + 1;
                    currentPlayers.Remove(i);
                    currentPlayers.Add(x);
                }
                currentPlayers.Add(playerIdx + 1);
                currentPlayers = new(currentPlayers.OrderBy(i => i));
                for(int i = playerIdx+1; i < allPlayers.Count; ++i)
                {
                    int x = i + 1;
                    allPlayers.Remove(i);
                    allPlayers.Add(x);
                }
                allPlayers.Add(playerIdx + 1);
                allPlayers.Sort();
                
            } else
            {
                currentPlayers.Add(playerIdx + 1);
                allPlayers.Add(playerIdx + 1);
            }
            p.InitSplit(playerIdx + 1);
        }
        public void Insurance()
        {
            /*
             * Ha az osztó színével felfelé látszó lapja Ász, akkor a játékos ezzel a bemondással „biztosítást” köthet. A tét legfeljebb az eredeti tét másfélszerese lehet. 
             * Ha az osztó másik kártyájának értéke 10 (10-es, Bubi, Dáma vagy Király), akkor a játékos a tétet 2:1 arányban kapja vissza. Ha az osztó másik kártyájának értéke a 10-estől eltérő, 
             * akkor az osztó nyer.
             */
        }

        public void Surrender()
        {
            /*
             * A játékos által, a játék feladására használt kifejezés. Ha a játékosnak csak az osztás utáni két lapja van még meg és úgy ítéli meg, hogy a játékot nem tudja megnyerni, 
             * akkor ezzel a bemondással feladhatja a játékot, és a tétje felét elveszti, a másik felét visszakapja.
             */
        }

        public void Stand()
        {
            Player current = players.ElementAt(playerIdx);
            current.StillInGame = false;
            AdvancePlayerIdx();
        }


        public int AddPlayer()
        {
            int db = currentPlayers.Count()+1;
            Player newPlayer = new(db);
            players.Add(newPlayer);
            currentPlayers.Add(db);
            allPlayers.Add(db);
            buttons.Add(db, GenerateButtons());
            return db;
        }

        // If the player busted/got 21/stood with their last action, they get removed from the game and the index points to the next active player
        private void AdvancePlayerIdx()
        {
            if(!players.ElementAt(playerIdx).StillInGame)
            {
                currentPlayers.Remove(playerIdx);
            }
            int idx = currentPlayers.IndexOf(playerIdx);
            playerIdx = currentPlayers.ElementAt(idx + 1);
        }
    }
}
