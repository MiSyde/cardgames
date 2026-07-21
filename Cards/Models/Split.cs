using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards.Models
{
    public partial class Split : Generic
    {
        public override bool IsSplit => true;
        public override int PlayerId { get; }

        private Player _player;
        public Player Player { get { return _player; } }
        public Split(Card c, int i, Player p, Blackjack b) : base(b) 
        {
            CanChangeBet = false;
            ChipsBoxVisibility = Visibility.Collapsed;
            CurrentCards.Add(c);
            IsActive = true;
            Id = i; 
            _player = p; 
            PlayerId = p.PlayerId; 
        }
    }
}
