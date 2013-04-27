using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OneStop
{
    [DataContract]
    public class BusStop
    {
        [DataMember]
        public int StopNo;

        [DataMember]
        public string Name;

        [DataMember]
        public bool WheelchairAccess;

        [DataMember]
        public String Routes;

        [DataMember]
        public string OnStreet;

        [DataMember]
        public string AtStreet;
    }
}