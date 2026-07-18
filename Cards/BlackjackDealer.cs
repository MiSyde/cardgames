using Cards.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    public class BlackjackDealer : Generic
    {
        private Random deal;
        private Deck deck;
        private int chips;
        public new bool IsSplit => false;
        public int Chips { get { return chips; } set { chips = value; } }

        public BlackjackDealer(Random deal, Deck deck) : base()
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

        public override void SetUpChipsGrid()
        {
            throw new NotImplementedException();
        }
    }

}
