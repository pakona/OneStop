using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneStop.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        const string mPrivacyPolicy = "OneStop uses your phone location to automatically find public transit stops around you." +
            "\n\nYour phone location is sent to Translink (Vancouver, BC transportation) to retrieve data about stops around your location." +
            "\n\nOneStop does not store your location. You can disable location access from OneStop at anytime by turning Off location in settings.";
        public string PrivacyPolicy { get { return mPrivacyPolicy; } }

        bool mShowPrivacyPolicy = false;
        public bool ShowPrivacyPolicy 
        {
            get { return mShowPrivacyPolicy; }
            set
            {
                mShowPrivacyPolicy = value;
                NotifyPropertyChanged("ShowPrivacyPolicy");
            }
        }

        public const int MIN_SEARCH_RADIUS = 100;
        public const int MAX_SEARCH_RADIUS = 1000;

        public int DefaultSearchRadius { get; private set; }

        bool mIsLocationAccessEnabled;
        public bool IsLocationAccessEnabled
        {
            get { return mIsLocationAccessEnabled; }
            set
            {
                mIsLocationAccessEnabled = value;
                IsLocationAccessKnown = true;
                IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
        public bool IsLocationAccessKnown { get; private set; }

        public SettingsViewModel()
        {
            Load();
        }

        void Load()
        {
            IsLocationAccessKnown = IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent");
            mIsLocationAccessEnabled = IsLocationAccessKnown ? (bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] : false;
            DefaultSearchRadius = MIN_SEARCH_RADIUS; //no support for changing the radius
            VerifySettings();
        }

        public void VerifySettings()
        {
            DefaultSearchRadius = Math.Max(MIN_SEARCH_RADIUS, DefaultSearchRadius);
            DefaultSearchRadius = Math.Min(MAX_SEARCH_RADIUS, DefaultSearchRadius);
        }

        public void EnableLocationAccess()
        {
            IsLocationAccessEnabled = true;
        }

        public void DisableLocationAccess()
        {
            IsLocationAccessEnabled = false;
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
