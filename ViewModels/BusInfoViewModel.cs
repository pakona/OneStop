using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneStop.ViewModels
{
    public class BusInfoViewModel : INotifyPropertyChanged
    {
        public int StopNo { get; set; }
        public string RouteNo { get; set; }
        public string Direction { get; set; }
        public string Location { get; set; }
        public string Destination { get; set; }
        public ObservableCollection<string> ExpectedLeaveTimes { get; set; }
        public bool StopsAtSkytrainStation { get; set; }
        public string NextLeaveTime { get; set; }
        public string LeaveTimeAfterNext { get; set; }

        public BusInfoViewModel()
        {
        }

        public BusInfoViewModel(int stopNo, string location, string routeNo)
        {
            StopNo = stopNo;
            RouteNo = routeNo;
            Location = location;
            ExpectedLeaveTimes = new ObservableCollection<string>();
        }

        public async void LoadData()
        {
            List<NextBus> busses = await TranslinkAPI.GetNextBusSchedulesAsync<List<NextBus>>(StopNo, RouteNo);

            if (busses.Count > 0)
            {
                NextBus nextBus = busses[0];
                Direction = nextBus.Direction;

                ExpectedLeaveTimes.Clear();
                foreach (Schedule schedule in nextBus.Schedules)
                {
                    ExpectedLeaveTimes.Add(DateTime.Parse(schedule.ExpectedLeaveTime).ToShortTimeString());
                    Destination = schedule.Destination.Trim();
                }
                NextLeaveTime = ExpectedLeaveTimes.Count > 0 ? ExpectedLeaveTimes[0] : String.Empty;
                LeaveTimeAfterNext = ExpectedLeaveTimes.Count > 1 ? ExpectedLeaveTimes[1] : String.Empty;

                if (!String.IsNullOrWhiteSpace(Destination) && !Destination.StartsWith("TO"))
                {
                    Destination = "to " + Destination;
                }
            }

            NotifyPropertyChanged("Direction");
            NotifyPropertyChanged("Destination");
            NotifyPropertyChanged("NextLeaveTime");
            NotifyPropertyChanged("LeaveTimeAfterNext");
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
