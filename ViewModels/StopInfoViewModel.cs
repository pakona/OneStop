using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslinkAPI.CSharp;

namespace OneStop.ViewModels
{
    public class StopInfoViewModel : INotifyPropertyChanged
    {
        public int StopNo { get; set; }
        public string Routes { get; set; }
        public bool WheelchairAccess { get; set; }
        public string Location { get; set; }

        public ObservableCollection<BusInfoViewModel> Items { get; set; }

        public StopInfoViewModel()
        {
        }

        public StopInfoViewModel(Stop stop)
        {
            StopNo = stop.StopNo;
            Routes = stop.Routes;
            WheelchairAccess = stop.WheelchairAccess;
            Location = String.Empty;
            if (!String.IsNullOrWhiteSpace(stop.OnStreet) && !String.IsNullOrWhiteSpace(stop.AtStreet))
            {
                Location = String.Format("{0} AT {1}", stop.OnStreet.Trim(), stop.AtStreet.Trim());
            }
            else if(!String.IsNullOrWhiteSpace(stop.OnStreet))
            {
                Location = stop.OnStreet.Trim();
            }
            else if (!String.IsNullOrWhiteSpace(stop.AtStreet))
            {
                Location = stop.AtStreet.Trim();
            }
            Items = new ObservableCollection<BusInfoViewModel>();
        }

        public void LoadData()
        {
            foreach (string routeNo in Routes.Replace(" ", "").Split(','))
            {
                BusInfoViewModel busInfoViewModel = new BusInfoViewModel(StopNo, Location, routeNo);
                Items.Add(busInfoViewModel);
                busInfoViewModel.LoadData();
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
