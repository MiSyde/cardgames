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
        Thickness defaultThickness;
        public MainWindow()
        {
            game = new();
            InitializeComponent();
            AddPlayerCommand = new RelayCommand(AddPlayer, () => game.Joinable);
            StartGameCommand = new RelayCommand(StartGame, () => game.PlayerCount >= 1 && game.Joinable);
            defaultThickness = new Thickness(5, 5, 5, 5);
        }

        private void StartGame()
        {
            game.StartGame();
            DealerCardBlock(game.Dealer.CardVisualBlock);
            AddPlayerCommand.NotifyCanExecuteChanged();
            StartGameCommand.NotifyCanExecuteChanged();
        }

        private void ContinueGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddPlayer()
        {
            ColumnDefinition Column = new();
            MainGrid.ColumnDefinitions.Add(Column);
            int id = game.AddPlayer();
            List<Button>? buttons = game.Buttons.GetValueOrDefault(id);
            int playerCount = game.PlayerCount;
            game.Hands.TryGetValue(id, out IHand? hand);
            Generic? g = hand as Generic;
            TextBlock textBlock = g.CardVisualBlock;
            DefaultObject(textBlock, playerCount - 1, 1);
            if (buttons != null)
            {
                int i = 2;
                foreach(Button b in buttons)
                {
                    DefaultObject(b, playerCount - 1, i++);
                }
            }
            StartGameCommand.NotifyCanExecuteChanged();
        }
        // Split gombjait hozzáadni!!
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
