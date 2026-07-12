using Cards.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    public class Player : Generic
    {
        private int chips;
        private bool didSplit;
        private Split? split;
        public int Chips { get { return chips; } set { chips = value; } }
        public Split? Split { get { return split; } }
        public bool DidSplit { get { return didSplit; } }
        private int myIdx;
        public int MyIdx { get { return myIdx; } set { myIdx = value; } }
        public void InitSplit(int i)
        {
            didSplit = true;
            split = new(CurrentCards.First(), i, this);
            CurrentCards.RemoveAt(0);
            if(chips >= Bet)
            {
                split.Bet = Bet;
                chips -= Bet;
            } else
            {
                split.Bet = chips;
                chips = 0;
            }
        }

        public Player(int i) : base()
        {
            myIdx = i;
            didSplit = false;
        }

        public bool CanSplit()
        {
            if(CurrentCards.Count == 2)
            {
                Card c1 = CurrentCards.First();
                Card c2 = CurrentCards.ElementAt(1);
                if(c1.FaceCardType != null)
                {
                    FaceCard fc = (FaceCard) c1.FaceCardType;
                    if(c2.FaceCardType != null)
                    {
                        FaceCard fc2 = (FaceCard) c2.FaceCardType;
                        if(fc == fc2) 
                        { 
                            return true; 
                        }
                        else
                        {
                            return false;
                        }
                    } 
                    else
                    {
                        return false;
                    }
                } 
                else
                {
                    if(c2.FaceCardType != null)
                    {
                        if(c1.Value == c2.Value)
                        {
                            return true;
                        } 
                        else
                        {
                            return false;
                        }
                    } 
                    else
                    {
                        return false;
                    }
                }
            } else
            {
                return false;
            }
        }
    }
}
