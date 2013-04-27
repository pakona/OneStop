using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OneStop.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.Windows;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Net.NetworkInformation;

namespace OneStop.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Items = new ObservableCollection<StopInfoViewModel>();
            Settings = new SettingsViewModel();
        }

        public ObservableCollection<StopInfoViewModel> Items { get; private set; }
        public SettingsViewModel Settings { get; private set; }

        public bool IsDataLoaded { get; set; }

        public bool IsLoadingData { get; private set; }
        public float LoadingDataProgressValue { get; private set; }
        public string LoadingDataProgressMessage { get; private set; }

        public string ErrorMessage { get; private set; }
        public bool EncounteredError { get; private set; }

        public void LoadData()
        {
            RequestUserAuthorization();
            BeginLoadingData();
            FillInStopsInfo();
        }

        private void BeginLoadingData()
        {
            Items.Clear();
            IsLoadingData = true;
            EncounteredError = false;
            NotifyPropertyChanged("IsLoadingData");
            NotifyPropertyChanged("EncounteredError");
        }

        private void EndLoadingData()
        {
            IsLoadingData = false;
            NotifyPropertyChanged("IsLoadingData");
        }

        private void NotifyLoadingDataProgress(float value, string message)
        {
            LoadingDataProgressValue += value;
            LoadingDataProgressMessage = message;
            NotifyPropertyChanged("LoadingDataProgressValue");
            NotifyPropertyChanged("LoadingDataProgressMessage");

            if (100.0f - LoadingDataProgressValue < 1.0f)
            {
                EndLoadingData();
                IsDataLoaded = true;
                NotifyPropertyChanged("IsDataLoaded");
            }
        }

        private void NotifyEncounteredError(string message)
        {
            ErrorMessage = message;
            EncounteredError = true;
            EndLoadingData();
            NotifyPropertyChanged("ErrorMessage");
            NotifyPropertyChanged("EncounteredError");
        }

        private async void FillInStopsInfo()
        {
            if (!Settings.IsLocationAccessEnabled)
            {
                // The user has opted out of Location.
                NotifyEncounteredError(":(\nWe do not have your approval for location access.");
                return;
            }

            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                NotifyEncounteredError(":(\nNo network connection.");
                return;
            }

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 100;

            try
            {
                NotifyLoadingDataProgress(0.0f, "Getting current location");
                Geoposition geoposition = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(3), timeout: TimeSpan.FromSeconds(10));

                const int MAX_TENTATIVES = 3;
                int numTentatives = 0;
                bool noStopAround = true;
                while (noStopAround && numTentatives < MAX_TENTATIVES)
                {
                    ++numTentatives;

                    //UnitTest values lat=49.249312, long=-123.010354
                    NotifyLoadingDataProgress(25.0f, "Getting closest stops");
                    List<BusStop> stops = await TranslinkAPI.GetClosestStopsAsync<List<BusStop>>(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude, Settings.DefaultSearchRadius * numTentatives);

                    foreach (BusStop stop in stops)
                    {
                        NotifyLoadingDataProgress(75.0f / stops.Count, String.Format("Getting info for stop #{0}", stop.StopNo));
                        StopInfoViewModel stopInfoViewModel = new StopInfoViewModel(stop);
                        Items.Add(stopInfoViewModel);
                        stopInfoViewModel.LoadData();
                    }

                    noStopAround = stops.Count == 0;
                }

                if (noStopAround)
                {
                    NotifyEncounteredError("No bus stop around.");
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    System.Diagnostics.Debug.WriteLine("location is disabled in phone settings.");

                    NotifyEncounteredError(":(\nLocation disabled in phone settings.");
                }
                else
                {
                    NotifyEncounteredError(":(\nSomething wrong happened, but it's not your fault :).");
                }
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void RequestUserAuthorization()
        {
            if (!Settings.IsLocationAccessKnown)
            {
                try
                {
                    MessageBoxResult result = MessageBox.Show(Settings.PrivacyPolicy, "Allow access to location?", MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        Settings.EnableLocationAccess();
                    }
                    else
                    {
                        Settings.DisableLocationAccess();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}