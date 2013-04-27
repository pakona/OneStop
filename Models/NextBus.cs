using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OneStop
{
    [DataContract]
    public class NextBus
    {
        [DataMember]
        public string Direction;

        [DataMember]
        public List<Schedule> Schedules;
    }

    [DataContract]
    public class Schedule
    {
        [DataMember]
        public string ExpectedLeaveTime;

        [DataMember]
        public string ScheduleStatus;

        [DataMember]
        public string Destination;
    }
}
