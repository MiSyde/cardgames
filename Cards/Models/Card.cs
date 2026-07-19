using Cards.Enums;
using System;

namespace Cards.Models
{
    public class Card
    {
        private int value;
        private bool isFaceCard;
        private FaceCard? faceCardType;
        private SuitType suitType;
        public int Value { get { return value; } }
        public bool IsFaceCard { get { return isFaceCard; } }
        public FaceCard? FaceCardType { get { return faceCardType; } }
        public SuitType SuitType { get { return suitType; } }

        public Card(int Value, bool IsFaceCard, SuitType SType, FaceCard? FCType = null)
        {
            value = Value;
            isFaceCard = IsFaceCard;
            faceCardType = FCType;
            suitType = SType;
        }
    }
}
