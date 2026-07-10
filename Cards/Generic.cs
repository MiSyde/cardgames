using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    public class Generic
    {
        private bool stillInGame;
        private int roundBet;
        private int cardScore;
        private bool twoCardBlackjack;
        private ObservableCollection<Card> currentCards;
        public ObservableCollection<Card> CurrentCards { get { return currentCards; } }
        public int Bet { get { return roundBet; } set { roundBet = value; } }
        public int CardScore { get { return cardScore; } set { cardScore = value; } }
        public bool StillInGame { get { return stillInGame; } set { stillInGame = value; } }
        public bool TwoCardBlackjack { get { return twoCardBlackjack; } set { twoCardBlackjack = value; } }
        public Generic()
        {
            currentCards = new();
            stillInGame = true;
            twoCardBlackjack = false;
            CurrentCards.CollectionChanged += UpdateScore;
            cardScore = 0;
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
                stillInGame = false;
            }
            if (cardScore == 21 && currentCards.Count == 2)
            {
                twoCardBlackjack = true;
            }
        }
    }
}
