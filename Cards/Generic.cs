using Cards.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    public abstract class Generic : IHand
    {
        private int roundBet;
        private int cardScore;
        private bool twoCardBlackjack;
        private ObservableCollection<Card> currentCards;
        public ObservableCollection<Card> CurrentCards => currentCards;
        public int Bet { get { return roundBet; } set { roundBet = value; } }
        public int CardScore { get { return cardScore; } set { cardScore = value; } }
        public bool TwoCardBlackjack { get { return twoCardBlackjack; } set { twoCardBlackjack = value; } }
        public bool IsActive { get; set; }
        public int Id { get; set; }
        public bool IsSplit { get; }
        public int PlayerId { get; }
        public bool Insured { get; set; }
        public int InsuredBet { get; set; }
        public bool Surrendered { get; set; }
        private StackPanel cardVisualBlock;
        private TextBlock cardScoreBlock;
        public Grid CardGrid;
        protected TextBox _betBox;
        public TextBox BetBox => _betBox;
        public Grid ChipsGrid;
        private List<Button> _buttons = new();
        public List<Button> Buttons { get => _buttons; set => _buttons = value; }
        public Generic()
        {
            currentCards = new();
            twoCardBlackjack = false;
            CurrentCards.CollectionChanged += UpdateScore;
            cardScore = 0;
            SetUpCardGrid();
        }

        public abstract void SetUpChipsGrid();

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

        public void UpdateButtons()
        {
            foreach (Button b in _buttons)
            {
                if (b.Command is RelayCommand<int> command)
                {
                    command.NotifyCanExecuteChanged();
                }
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

            cardScoreBlock.Text = cardScore.ToString();
            Handle_Images(e.Action, currentCards.LastOrDefault());
        }

        private void Handle_Images(System.Collections.Specialized.NotifyCollectionChangedAction action, Card? c)
        {
            if(action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                cardVisualBlock.Children.Add(Load_Image(c));
            } 
            else
            {
                if(cardVisualBlock.Children.Count > 0) 
                    cardVisualBlock.Children.RemoveAt(1);
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
