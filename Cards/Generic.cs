using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    public class Generic : IHand
    {
        private int roundBet;
        private int cardScore;
        private bool twoCardBlackjack;
        private ObservableCollection<Card> currentCards;
        public ObservableCollection<Card> CurrentCards { get { return currentCards; } }
        public int Bet { get { return roundBet; } set { roundBet = value; } }
        public int CardScore { get { return cardScore; } set { cardScore = value; } }
        public bool TwoCardBlackjack { get { return twoCardBlackjack; } set { twoCardBlackjack = value; } }
        public bool IsActive { get; set; }
        public int Id { get; set; }
        public bool IsSplit { get; }
        public int PlayerId { get; }
        public bool Insured { get; set; }
        public int InsuredBet { get; set; }
        public bool Surrendered { get; set; }
        private List<Button> _buttons = new();
        public List<Button> Buttons { get => _buttons; set => _buttons = value; }
        public Generic()
        {
            currentCards = new();
            twoCardBlackjack = false;
            CurrentCards.CollectionChanged += UpdateScore;
            cardScore = 0;
        }
        public List<Button> GetButtons()
        {
            return _buttons;
        }

        public void UpdateButtons()
        {
            foreach (Button b in _buttons)
            {
                if (b.Command is RelayCommand<int> command)
                {
                    command.NotifyCanExecuteChanged();
                }
            }
        }
        private void UpdateScore(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            cardScore = 0;
            List<Card> Aces = new();
            foreach (Card c in currentCards)
            {
                if (c.IsFaceCard)
                {
                    if (c.FaceCardType.Equals(Enums.FaceCard.Ace))
                    {
                        Aces.Add(c);
                        continue;
                    }
                }
                cardScore += c.Value;
            }
            for (int i = 0; i < Aces.Count(); ++i)
            {
                if (cardScore + 10 > 21)
                {
                    cardScore += 1;
                }
                else
                {
                    cardScore += 10;
                }
            }
            if (cardScore >= 21)
            {
                IsActive = false;
            }
            if (cardScore == 21 && currentCards.Count == 2)
            {
                twoCardBlackjack = true;
            }
        }
    }
}
