using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using OneStop.ViewModels;

namespace OneStop
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            DataContext = App.ViewModel.Settings;
        }

        private void mBtnShowPrivacyPolicy_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SettingsViewModel).ShowPrivacyPolicy = !(DataContext as SettingsViewModel).ShowPrivacyPolicy;
        }

        private void mBtnRateAndReview_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
    }
}