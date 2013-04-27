using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using OneStop.Resources;
using OneStop.ViewModels;
using Microsoft.Phone.Tasks;

namespace OneStop
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void RefreshBarIconButton_Click(object sender, EventArgs e)
        {
            MainViewModel viewModel = (DataContext as MainViewModel);
            if (viewModel != null && !viewModel.IsLoadingData)
            {
                viewModel.LoadData();
            }
        }

        private void SettingsBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}