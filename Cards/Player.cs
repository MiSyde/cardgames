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
            didSplit = false;
            myIdx = i;
        }
    }
}
