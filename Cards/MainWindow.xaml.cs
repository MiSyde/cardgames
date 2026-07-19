using Cards.Models;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cards
{
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Visibility _gameButtonsVisibility;
        public Visibility GameButtonsVisibility
        {
            get => _gameButtonsVisibility;
            set
            {
                if(_gameButtonsVisibility != value)
                {
                    _gameButtonsVisibility = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameButtonsVisibility)));
                }
            }
        }

        private Visibility _backButtonVisibility;
        public Visibility BackButtonVisibility
        {
            get => _backButtonVisibility;
            set
            {
                if (_backButtonVisibility != value)
                {
                    _backButtonVisibility = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackButtonVisibility)));
                }
            }
        }
        public MainWindow()
        {
            GameButtonsVisibility = Visibility.Visible;
            BackButtonVisibility = Visibility.Collapsed;
            InitializeComponent();
            //_ = SetIconAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async Task SetIconAsync()
        {
            Uri uri = new Uri("ms-appx:///Assets/logo.ico");
            StorageFile? storageFile = null;
            try
            {
                storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            }
            catch (Exception ex)
            {
                // Use default icon.
            }

            if (storageFile is not null)
            {
                this?.AppWindow.SetIcon(storageFile.Path);
            }
        }


        private void ShowBlackjack(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(BlackjackPage));
            GameButtonsVisibility = Visibility.Collapsed;
            BackButtonVisibility = Visibility.Visible;
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = null;
            GameButtonsVisibility = Visibility.Visible;
            BackButtonVisibility = Visibility.Collapsed;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}
