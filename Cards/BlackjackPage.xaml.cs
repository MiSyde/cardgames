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
using System.Collections.ObjectModel;

namespace Cards
{
    public sealed partial class BlackjackPage : Page
    {
        readonly Blackjack game;
        RelayCommand AddPlayerCommand;
        RelayCommand StartGameCommand;
        Dictionary<int, int> handColumns;
        string _text;
        string Text
        {
            get => _text;
            set
            {
                _text = value;
                AddPlayerCommand.NotifyCanExecuteChanged();
            }
        }
        public BlackjackPage()
        {
            game = new();
            _text = string.Empty;
            AddPlayerCommand = new RelayCommand(AddPlayer, CanAddPlayer);
            StartGameCommand = new RelayCommand(StartGame, CanStartGame);
            handColumns = new();
            InitializeComponent();
        }

        private bool CanAddPlayer()
        {
            return game.Joinable && ChipsBox.Text != string.Empty;
        }

        private bool CanStartGame()
        {
            return game.PlayerCount >= 1 && game.Joinable && game.ReadyToStart();
        }

        private void StartGame()
        {
            game.StartGame();
            AddPlayerCommand.NotifyCanExecuteChanged();
            StartGameCommand.NotifyCanExecuteChanged();
        }

        private void ContinueGame_Click(object sender, RoutedEventArgs e)
        {
            game.NextGame();
        }
      
        private void AddPlayer()
        {
            int id = game.AddPlayer(ChipsBox.Text);
            
            int column = game.PlayerCount - 1;
            handColumns[id] = column;

            ChipsBox.Text = string.Empty;
            AddPlayerCommand.NotifyCanExecuteChanged();
            StartGameCommand.NotifyCanExecuteChanged();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "[^0-9]"))
            {
                textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            StartGameCommand.NotifyCanExecuteChanged();
        }
    }
}
