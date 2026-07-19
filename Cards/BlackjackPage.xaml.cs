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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Cards.Models;

namespace Cards
{
    public sealed partial class BlackjackPage : Page
    {
        readonly Blackjack game;
        RelayCommand AddPlayerCommand;
        RelayCommand StartGameCommand;
        Thickness defaultThickness;
        Dictionary<int, int> handColumns;
        string _text;
        string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    AddPlayerCommand.NotifyCanExecuteChanged();
                }
            }
        }
        public BlackjackPage()
        {
            game = new();
            _text = string.Empty;
            AddPlayerCommand = new RelayCommand(AddPlayer, CanAddPlayer);
            StartGameCommand = new RelayCommand(StartGame, CanStartGame);
            defaultThickness = new Thickness(5, 5, 5, 5);
            handColumns = new();
            InitializeComponent();
        }

        private bool CanAddPlayer()
        {
            return game.Joinable && ChipsBox.Text != string.Empty;
        }

        private bool CanStartGame()
        {
            /*
            foreach (IHand hand in game.Hands.Values)
            {
                Player? p = hand as Player;
                int.TryParse(p?.BetBox.Text, out int bet);
                if (bet > p?.Chips || p?.BetBox.Text == string.Empty)
                {
                    p.BetBox.Text = string.Empty;
                    return false;
                }
            }
            */
            return game.PlayerCount >= 1 && game.Joinable;
        }

        private void StartGame()
        {
            game.StartGame();
            //DealerCardBlock(game.Dealer.CardGrid);
            AddPlayerCommand.NotifyCanExecuteChanged();
            StartGameCommand.NotifyCanExecuteChanged();
            //game.ActiveHands.CollectionChanged += SplitWatcher;
        }
        // !
        private void ContinueGame_Click(object sender, RoutedEventArgs e)
        {
            game.NextGame();
        }
        /*
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
                        Grid.SetColumn(existingGeneric.CardGrid, shiftedColumn);
                        if (!existingHand.IsSplit)
                            Grid.SetColumn(existingGeneric.ChipsGrid, shiftedColumn);
                        else
                            Grid.SetColumn(existingGeneric.BetBox, shiftedColumn);
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
                    DefaultObject(newGeneric.CardGrid, newColumn, 1);
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

            Grid.SetColumnSpan(game.Dealer.CardGrid, Grid.GetColumnSpan(MainGrid));

            game.AdvanceAfterSplitEvent();
        }
        */
        private void AddPlayer()
        {
            int id = game.AddPlayer(ChipsBox.Text);

            //ColumnDefinition Column = new();
            //MainGrid.ColumnDefinitions.Add(Column);
            
            int column = game.PlayerCount - 1;

            //List<Button>? buttons = game.Buttons.GetValueOrDefault(id);
            //game.Hands.TryGetValue(id, out IHand? hand);
            handColumns[id] = column;

            //Generic? g = hand as Generic;
            //int i = 1;
            //DefaultObject(g.CardGrid, column, i++);
            /*
            if (buttons != null)
            {
                foreach (Button b in buttons)
                {
                    DefaultObject(b, column, i++);
                }
            }
            */
            //DefaultObject(g.ChipsGrid, column, i);

            //Grid.SetColumnSpan(game.Dealer.CardGrid, Grid.GetColumnSpan(MainGrid));

            ChipsBox.Text = string.Empty;
            //g.BetBox.TextChanged += TextBox_TextChanged;

            StartGameCommand.NotifyCanExecuteChanged();
        }

        private void DefaultObject(FrameworkElement UIelement, int column, int row)
        {
            Grid.SetColumn(UIelement, column);
            Grid.SetRow(UIelement, row);
            MainGrid.Children.Add(UIelement);
            UIelement.Margin = defaultThickness;
            UIelement.HorizontalAlignment = HorizontalAlignment.Center;
            UIelement.VerticalAlignment = VerticalAlignment.Center;
        }
        /*
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
        */
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "[^0-9]"))
            {
                textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
            }
            if (textBox.Tag.ToString() == "Bet") StartGameCommand.NotifyCanExecuteChanged();
        }
    }
}
