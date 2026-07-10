using Cards.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    public class Deck
    {
        private List<Card> cards;
        private int currentSize;
        public List<Card> Cards { get { return cards; } }
        public int CurrentSize { get { return currentSize; }  }

        public Deck()
        {
            currentSize = 52;
            cards = new List<Card>();
            List<SuitType> suits = new();
            suits.Add(SuitType.Spades);
            suits.Add(SuitType.Hearts);
            suits.Add(SuitType.Clubs);
            suits.Add(SuitType.Diamonds);
            List<FaceCard> faces = new();
            faces.Add(FaceCard.Jack);
            faces.Add(FaceCard.Queen);
            faces.Add(FaceCard.King);
            faces.Add(FaceCard.Ace);
            for (int j = 0; j < 4; ++j)
            {
                for (int i = 2; i < 11; ++i)
                {
                    cards.Add(new Card(i, false, suits.ElementAt(j)));
                }
                for (int i = 0; i < 4; ++i)
                {
                    cards.Add(new Card(10, true, suits.ElementAt(j), faces.ElementAt(j)));
                }
            }
        }
        public void Remove(Card c)
        {
            --currentSize;
            cards.Remove(c);
        }
    }

}
