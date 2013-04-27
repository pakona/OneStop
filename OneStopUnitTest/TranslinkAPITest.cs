using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OneStop;
using System.Collections.Generic;

namespace OneStopUnitTest
{
    [TestClass]
    public class TranslinkAPITest
    {
        [TestMethod]
        public async void ClosestStopToGilmoreAndSandersonWay()
        {
            const double lat = 49.249312;
            const double lng = -123.010354;
            const int radius = 100;

            List<BusStop> stops = await TranslinkAPI.GetClosestStopsAsync<List<BusStop>>(lat, lng, radius);

            CollectionAssert.AllItemsAreUnique(stops);
            Assert.AreEqual(2, stops.Count);
            Assert.AreEqual(51549, stops[0].StopNo);
        }

        [TestMethod]
        public async void NextBusSchedulesAtStop51549()
        {
            const int stopNo = 51549;
            const string routeNo = "129";
            const int count = 2;

            List<NextBus> nextBus = await TranslinkAPI.GetNextBusSchedulesAsync<List<NextBus>>(stopNo, routeNo, count);

            Assert.AreEqual("WEST", nextBus[0].Direction);
            Assert.IsNotNull(nextBus[0].Schedules);
        }
    }
}
