using Cards.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards.Models
{
    public class Player : Generic
    {
        public new bool IsSplit => false;
        public new int PlayerId => Id;
        private bool _didSplit;
        public bool DidSplit 
        {
            get => _didSplit; 
            set { _didSplit = value; } 
        }
        private int _splitId;
        public int SplitId
        {
            get => _splitId;
            set { _splitId = value; }
        }

        public Player(int i, Blackjack b) : base(b)
        {
            //SetUpChipsGrid();
            Id = i;
            IsActive = true;
            DidSplit = false;
            _splitId = -1;
        }

        public bool CanSplit()
        {
            if (DidSplit) return false;
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
                    if (c1.Value == c2.Value)
                    {
                        return true;
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
        /*
        public override void SetUpChipsGrid()
        {
            ChipsGrid = new();

            _betBox = new();
            _betBox.TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center;
            _chipsBox = new();
            _chipsBox.TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center;
            _betBox.Tag = "Bet";

            Binding chipsBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(Chips)),
                Mode = BindingMode.OneWay
            };

            _chipsBox.SetBinding(TextBlock.TextProperty, chipsBinding);

            RowDefinition chipsRow = new();
            RowDefinition betRow = new();

            ChipsGrid.RowDefinitions.Add(chipsRow);
            ChipsGrid.RowDefinitions.Add(betRow);

            Grid.SetRow(_chipsBox, 0);
            Grid.SetRow(_betBox, 1);

            ChipsGrid.Children.Add(_chipsBox);
            ChipsGrid.Children.Add(_betBox);
        }
        */
    }
}
