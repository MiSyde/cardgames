using Microsoft.UI.Xaml.Controls;
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
        public new bool IsSplit => true;
        public new int PlayerId { get; }

        private Player _player;
        public Player Player { get { return _player; } }
        public Split(Card c, int i, Player p) : base() 
        { 
            CurrentCards.Add(c);
            IsActive = true;
            Id = i; 
            _player = p; 
            PlayerId = p.PlayerId; 
        }
    }
}
