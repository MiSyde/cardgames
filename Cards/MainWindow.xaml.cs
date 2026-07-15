using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Cards
{
    public sealed partial class MainWindow : Window
    {
        readonly Blackjack game;
        RelayCommand AddPlayerCommand;
        RelayCommand StartGameCommand;
        Thickness defaultThickness;
        Dictionary<int, int> handColumns;
        public MainWindow()
        {
            game = new();
            InitializeComponent();
            AddPlayerCommand = new RelayCommand(AddPlayer, () => game.Joinable);
            StartGameCommand = new RelayCommand(StartGame, () => game.PlayerCount >= 1 && game.Joinable);
            defaultThickness = new Thickness(5, 5, 5, 5);
            handColumns = new();
        }

        private void StartGame()
        {
            game.StartGame();
            DealerCardBlock(game.Dealer.CardVisualBlock);
            AddPlayerCommand.NotifyCanExecuteChanged();
            StartGameCommand.NotifyCanExecuteChanged();
            game.ActiveHands.CollectionChanged += SplitWatcher;
        }
        // !
        private void ContinueGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SplitWatcher(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (game.Joinable || e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add) return;

            int newHandId = (int)e.NewItems[0];

            int insertedIndex = game.ActiveHands.IndexOf(newHandId);
            int previousHandId = game.ActiveHands[insertedIndex - 1];

            if (handColumns.TryGetValue(previousHandId, out int previousColumn))
            {
                int newColumn = previousColumn + 1;

                foreach (var kvp in handColumns.Where(kv => kv.Value >= newColumn).ToList())
                {
                    int handId = kvp.Key;
                    int shiftedColumn = kvp.Value + 1;
                    handColumns[handId] = shiftedColumn;

                    if (game.Hands.TryGetValue(handId, out IHand? existingHand) && existingHand is Generic existingGeneric)
                    {
                        Grid.SetColumn(existingGeneric.CardVisualBlock, shiftedColumn);
                    }
                    List<Button>? shiftedButtons = game.Buttons.GetValueOrDefault(handId);
                    if (shiftedButtons != null)
                    {
                        foreach (Button b in shiftedButtons)
                        {
                            Grid.SetColumn(b, shiftedColumn);
                        }
                    }
                }

                MainGrid.ColumnDefinitions.Insert(newColumn, new ColumnDefinition());
                handColumns[newHandId] = newColumn;

                if (game.Hands.TryGetValue(newHandId, out IHand? newHand) && newHand is Generic newGeneric)
                {
                    DefaultObject(newGeneric.CardVisualBlock, newColumn, 1);
                }
                List<Button>? newButtons = game.Buttons.GetValueOrDefault(newHandId);
                if (newButtons != null)
                {
                    int row = 2;
                    foreach (Button b in newButtons)
                    {
                        DefaultObject(b, newColumn, row++);
                    }
                }
            }

            Grid.SetColumnSpan(game.Dealer.CardVisualBlock, Grid.GetColumnSpan(MainGrid));

            game.AdvanceAfterSplitEvent();
        }

        private void AddPlayer()
        {
            ColumnDefinition Column = new();
            MainGrid.ColumnDefinitions.Add(Column);

            int id = game.AddPlayer();
            int column = game.PlayerCount - 1;

            List<Button>? buttons = game.Buttons.GetValueOrDefault(id);
            game.Hands.TryGetValue(id, out IHand? hand);
            handColumns[id] = column;

            Generic? g = hand as Generic;
            TextBlock textBlock = g.CardVisualBlock;

            DefaultObject(textBlock, column, 1);
            if (buttons != null)
            {
                int i = 2;
                foreach(Button b in buttons)
                {
                    DefaultObject(b, column, i++);
                }
            }

            StartGameCommand.NotifyCanExecuteChanged();
        }

        private void DefaultObject(FrameworkElement UIelement,int column, int row)
        {
            Grid.SetColumn(UIelement, column);
            Grid.SetRow(UIelement, row);
            MainGrid.Children.Add(UIelement);
            UIelement.Margin = defaultThickness;
            UIelement.HorizontalAlignment = HorizontalAlignment.Center;
            UIelement.VerticalAlignment = VerticalAlignment.Center;
        }
        
        private void DealerCardBlock(FrameworkElement UIelement)
        {
            Grid.SetColumn(UIelement, 1);
            Grid.SetRow(UIelement, 0);
            Grid.SetColumnSpan(UIelement, Grid.GetColumnSpan(MainGrid));
            MainGrid.Children.Add(UIelement);
            UIelement.Margin = defaultThickness;
            UIelement.HorizontalAlignment = HorizontalAlignment.Center;
            UIelement.VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
