using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards.Models
{
    public class Split : Generic
    {
        public new bool IsSplit => true;
        public new int PlayerId { get; }

        private Player _player;
        public Player Player { get { return _player; } }
        public Split(Card c, int i, Player p, Blackjack b) : base(b) 
        {
            //SetUpChipsGrid();
            CurrentCards.Add(c);
            IsActive = true;
            Id = i; 
            _player = p; 
            PlayerId = p.PlayerId; 
        }
        /*
        public override void SetUpChipsGrid()
        {
            ChipsGrid = new();

            _betBox = new();
            _betBox.TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center;
            _betBox.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center;

            RowDefinition betRow = new();

            ChipsGrid.RowDefinitions.Add(betRow);

            Grid.SetRow(_betBox, 1);

            ChipsGrid.Children.Add(_betBox);
        }
        */
    }
}
