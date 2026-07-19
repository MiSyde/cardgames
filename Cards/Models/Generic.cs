using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
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
    public class Generic : IHand, INotifyPropertyChanged
    {
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
        public ObservableCollection<Image> CardImages { get; }
        /*
        private List<Button> _buttons = new();
        public List<Button> Buttons { get => _buttons; set => _buttons = value; }
        */
        private int roundBet;
        public int Bet 
        { 
            get => roundBet; 
            set { roundBet = value; } 
        }
        private int cardScore;
        public int CardScore 
        {
            get => cardScore;
            set { cardScore = value; } 
        }
        private bool twoCardBlackjack;
        public bool TwoCardBlackjack 
        {
            get => twoCardBlackjack;
            set { twoCardBlackjack = value; } 
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
        public bool IsSplit { get; }
        public int PlayerId { get; }
        public bool Insured { get; set; }
        public int InsuredBet { get; set; }
        public bool Surrendered { get; set; }
        /*
        private StackPanel cardVisualBlock;
        private TextBlock cardScoreBlock;
        public Grid CardGrid;
        protected TextBox _betBox;
        public TextBox BetBox => _betBox;
        public Grid ChipsGrid;
        private TextBlock _chipsBox;
        public TextBlock ChipsBox => _chipsBox;
        */

        public Generic(Blackjack g)
        {
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
            twoCardBlackjack = false;
            CurrentCards.CollectionChanged += UpdateScore;
            cardScore = 0;
            //SetUpCardGrid();
        }
        /*
        private void SetUpCardGrid()
        {
            CardGrid = new();

            cardVisualBlock = new();
            cardVisualBlock.Orientation = Orientation.Horizontal;
            cardScoreBlock = new();
            cardScoreBlock.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center;

            RowDefinition cardRow = new();
            cardRow.Height = Microsoft.UI.Xaml.GridLength.Auto;
            RowDefinition scoreRow = new();

            CardGrid.RowDefinitions.Add(cardRow);
            CardGrid.RowDefinitions.Add(scoreRow);

            Grid.SetRow(cardVisualBlock, 0);
            Grid.SetRow(cardScoreBlock, 1);

            CardGrid.Children.Add(cardVisualBlock);
            CardGrid.Children.Add(cardScoreBlock);
        }
        
        public List<Button> GetButtons()
        {
            return _buttons;
        }
        */

        public void UpdateCommands()
        {
            foreach (RelayCommand<int> c in Commands)
            {
                c.NotifyCanExecuteChanged();
            }
        }
        private void UpdateScore(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            cardScore = 0;
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
                cardScore += c.Value;
            }
            for (int i = 0; i < Aces.Count(); ++i)
            {
                if (cardScore + 10 > 21)
                {
                    cardScore += 1;
                }
                else
                {
                    cardScore += 10;
                }
            }
            if (cardScore >= 21)
            {
                IsActive = false;
            }
            if (cardScore == 21 && currentCards.Count == 2)
            {
                twoCardBlackjack = true;
            }

            //cardScoreBlock.Text = cardScore.ToString();
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
                if(CurrentCards.Count > 1) 
                    CardImages.RemoveAt(1);
            }
        }

        private Image Load_Image(Card? c)
        {
            Image img = new();
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
            img.Source = bitmapImage;
            img.Height = 201;
            img.Width = 150;
            img.Margin = new Microsoft.UI.Xaml.Thickness(3, 3, 3, 3);
            return img;
        }

        
    }
}
