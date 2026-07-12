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
        public MainWindow()
        {
            game = new();
            InitializeComponent();
            AddPlayerButton();
            RowDefinition Card = new();
            RowDefinition HitButtonRow = new();
            RowDefinition StandButtonRow = new();
            RowDefinition DoubleDownButtonRow = new();
            RowDefinition SurrenderButtonRow = new();
            RowDefinition InsuranceButtonRow = new();
            RowDefinition SplitButtonRow = new();
            MainGrid.RowDefinitions.Add(Card);
            MainGrid.RowDefinitions.Add(HitButtonRow);
            MainGrid.RowDefinitions.Add(StandButtonRow);
            MainGrid.RowDefinitions.Add(DoubleDownButtonRow);
            MainGrid.RowDefinitions.Add(SurrenderButtonRow);
            MainGrid.RowDefinitions.Add(InsuranceButtonRow);
            MainGrid.RowDefinitions.Add(SplitButtonRow);
        }

        private void AddPlayerButton()
        {
            Button b = new();
            b.Content = "Add player";
            b.Command = new RelayCommand(AddPlayer, () => game.Joinable);
            MainGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetColumn(b, 0);
            Grid.SetRow(b, 0);
            MainGrid.Children.Add(b);
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
                    ++i;
                }
            }
        }
    }
}
