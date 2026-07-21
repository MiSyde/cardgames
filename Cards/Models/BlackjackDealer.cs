using Cards.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards.Models
{
    public partial class BlackjackDealer : Generic
    {
        private Random deal;
        private Deck deck;
        public new bool IsSplit => false;

        public BlackjackDealer(Random deal, Deck deck, Blackjack b) : base(b)
        { 
            this.deal = deal;
            this.deck = deck;
        }
        public Card Deal()
        {
            int r = deal.Next(0, deck.CurrentSize-1);
            Card c = deck.Cards.ElementAt(r);
            deck.Remove(c);
            return c;
        }
        public void Draw()
        {
            CurrentCards.Add(Deal());
        }
        public void FinishDrawing()
        {
            Draw();
            if (CardScore >= 21) return;
            if (CardScore <= 15) FinishDrawing();
            return;
        }

        public void ResetDeck()
        {
            deck = new();
        }
    }

}
