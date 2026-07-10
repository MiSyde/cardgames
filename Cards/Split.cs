using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    public class Split : Generic
    {
        private int myIdx;
        public int MyIdx { get { return myIdx; } }
        private Player player;
        public Player Player { get { return player; } }
        public Split(Card c, int i, Player p) : base() { CurrentCards.Add(c); myIdx = i; player = p; }

    }
}
