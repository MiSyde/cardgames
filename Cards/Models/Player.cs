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
    public partial class Player : Generic
    {
        public override bool IsSplit => false;
        public override int PlayerId => Id;
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
            ChipsBoxVisibility = Visibility.Visible;
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
    }
}
