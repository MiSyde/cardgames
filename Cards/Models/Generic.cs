using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.UI;

namespace Cards.Models
{
    public partial class Generic : IHand, INotifyPropertyChanged
    {
        public virtual Visibility ChipsBoxVisibility { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand<int> HitCommand;
        public RelayCommand<int> StandCommand;
        public RelayCommand<int> DoubleDownCommand;
        public RelayCommand<int> SurrenderCommand;
        public RelayCommand<int> InsureCommand;
        public RelayCommand<int> SplitCommand;
        List<RelayCommand<int>> Commands;
        private ObservableCollection<Card> currentCards;
        public ObservableCollection<Card> CurrentCards => currentCards;
        public ObservableCollection<ImageSource> CardImages { get; }
        private bool _canChangeBet;
        public bool CanChangeBet
        {
            get => _canChangeBet;
            set
            {
                if(_canChangeBet != value)
                {
                    _canChangeBet = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanChangeBet)));
                }
            }
        }
        private int roundBet;
        public int Bet 
        { 
            get => roundBet; 
            set { 
                roundBet = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bet)));
            } 
        }
        private int cardScore;
        public int CardScore 
        {
            get => cardScore;
            set 
            { 
                cardScore = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CardScore)));
            } 
        }
        private bool twoCardBlackjack;
        public bool TwoCardBlackjack 
        {
            get => twoCardBlackjack;
            set 
            { 
                twoCardBlackjack = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TwoCardBlackjack)));
            } 
        }
        private int _chips;
        public int Chips
        {
            get => _chips;
            set
            {
                if (_chips != value)
                {
                    _chips = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Chips)));
                }
            }
        }
        public bool IsActive { get; set; }
        public int Id { get; set; }
        public virtual bool IsSplit { get; }
        public virtual int PlayerId { get; }
        private bool _insured;
        public bool Insured
        {
            get => _insured;
            set
            {
                if (_insured != value)
                {
                    _insured = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Insured)));
                }
            }
        }
        private int _insuredBet;
        public int InsuredBet
        {
            get => _insuredBet;
            set
            {
                if (_insuredBet != value)
                {
                    _insuredBet = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InsuredBet)));
                }
            }
        }
        
        public bool Surrendered { get; set; }

        public Generic(Blackjack g)
        {
            CanChangeBet = true;
            Commands = new();

            HitCommand = new RelayCommand<int>((id) => g.Hit(id), (id) => g.CanAct(id));
            Commands.Add(HitCommand);

            StandCommand = new RelayCommand<int>((id) => g.Stand(id), (id) => g.CanAct(id));
            Commands.Add(StandCommand);

            DoubleDownCommand = new RelayCommand<int>((id) => g.DoubleDown(id), (id) => g.CanAct(id));
            Commands.Add(DoubleDownCommand);

            SurrenderCommand = new RelayCommand<int>((id) => g.Surrender(id), (id) => g.CanSurrender(id));
            Commands.Add(SurrenderCommand);

            InsureCommand = new RelayCommand<int>((id) => g.Insurance(id), (id) => g.CanInsure(id));
            Commands.Add(InsureCommand);

            SplitCommand = new RelayCommand<int>((id) => g.Split(id), (id) => g.CanSplit(id));
            Commands.Add(SplitCommand);

            CardImages = new();
            currentCards = new();

            CurrentCards.CollectionChanged += UpdateScore;

            Insured = false;
            twoCardBlackjack = false;
            cardScore = 0;
            InsuredBet = 0;
        }

        public void UpdateCommands()
        {
            foreach (RelayCommand<int> c in Commands)
            {
                c.NotifyCanExecuteChanged();
            }
        }
        private void UpdateScore(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CardScore = 0;
            List<Card> Aces = new();
            foreach (Card c in currentCards)
            {
                if (c.IsFaceCard)
                {
                    if (c.FaceCardType.Equals(Enums.FaceCard.Ace))
                    {
                        Aces.Add(c);
                        continue;
                    }
                }
                CardScore += c.Value;
            }
            for (int i = 0; i < Aces.Count(); ++i)
            {
                if (CardScore + 10 > 21)
                {
                    CardScore += 1;
                }
                else
                {
                    CardScore += 10;
                }
            }
            if (CardScore >= 21)
            {
                IsActive = false;
            }
            if (CardScore == 21 && currentCards.Count == 2)
            {
                TwoCardBlackjack = true;
            }

            Handle_Images(e.Action, currentCards.LastOrDefault());
        }

        private void Handle_Images(System.Collections.Specialized.NotifyCollectionChangedAction action, Card? c)
        {
            if(action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                CardImages.Add(Load_Image(c));
            } 
            else
            {
                if(CurrentCards.Count > 0) 
                    CardImages.RemoveAt(1);
            }
        }

        private ImageSource Load_Image(Card? c)
        {
            BitmapImage bitmapImage = new();
            string CardSuit = c.SuitType.GetDescription();
            string CardValue = string.Empty;
            if(c.IsFaceCard)
            {
                CardValue = c.FaceCardType.GetDescription();
            } 
            else
            {
                CardValue = c.Value.ToString();
            }
            bitmapImage.UriSource = new Uri("ms-appx:///Assets/CardImages/" + CardSuit + "/" + CardValue + ".png");
            return bitmapImage;
        }

    }
}
