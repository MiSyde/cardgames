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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cards
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        readonly Blackjack game;
        RelayCommand AddPlayerCommand;
        RelayCommand StartGameCommand;
        Thickness buttonThickness;
        public MainWindow()
        {
            game = new();
            InitializeComponent();
            AddPlayerCommand = new RelayCommand(AddPlayer, () => game.Joinable);
            StartGameCommand = new RelayCommand(StartGame, () => game.PlayerCount >= 1 && game.Joinable);
            buttonThickness = new Thickness(5, 5, 5, 5);
        }

        private void StartGame()
        {
            game.StartGame();
            AddPlayerCommand.NotifyCanExecuteChanged();
            StartGameCommand.NotifyCanExecuteChanged();
        }

        private void AddPlayer()
        {
            ColumnDefinition Column = new();
            MainGrid.ColumnDefinitions.Add(Column);
            int id = game.AddPlayer();
            List<Button>? buttons = game.Buttons.GetValueOrDefault(id);
            if(buttons != null)
            {
                int playerCount = game.PlayerCount;
                int i = 1;
                foreach(Button b in buttons)
                {
                    Grid.SetColumn(b, playerCount - 1);
                    Grid.SetRow(b, i+1);
                    MainGrid.Children.Add(b);
                    b.Margin = buttonThickness;
                    b.HorizontalAlignment = HorizontalAlignment.Center;
                    b.VerticalAlignment = VerticalAlignment.Center;
                    ++i;
                }
            }
            StartGameCommand.NotifyCanExecuteChanged();
        }
    }
}
