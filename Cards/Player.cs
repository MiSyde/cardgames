using Cards.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
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
        public new bool IsSplit => false;
        public new int PlayerId => Id;
        private int _chips;
        private int _splitId;
        public int SplitId { get { return _splitId; } set { _splitId = value; } }
        private bool _didSplit;
        public bool DidSplit { get { return _didSplit; } set { _didSplit = value; } }

        public int Chips { get { return _chips; } set { _chips = value; } }

        public Player(int i) : base()
        {
            
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
