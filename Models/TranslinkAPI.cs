using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinPhoneExtensions;

namespace OneStop
{
    public class APIAsyncResult<T>
    {
        public HttpWebRequest request = null;
        public HttpWebResponse response = null;
        public T data;
    }

    public static class TranslinkAPI
    {
        const string API_KEY = "On7sr8TSCW6a6g7qub2d";
        const string API_URL = "http://api.translink.ca/RTTIAPI/V1/";

        public async static Task<T> GetClosestStopsAsync<T>(double lat, double lng, int radius = 100) where T : new()
        {
            string requestString = String.Format("{0}stops?lat={1:0.####}&long={2:0.####}&radius={3}&apiKey={4}", API_URL, lat, lng, radius, API_KEY);
            return await APIRequestAsync<T>(requestString);
        }

        public async static Task<T> GetNextBusSchedulesAsync<T>(int stopNo, string routeNo, int count = 2) where T : new()
        {
            string requestString = String.Format("{0}stops/{1}/estimates?count={2}&routeNo={3}&apikey={4}", API_URL, stopNo, count, routeNo, API_KEY);
            return await APIRequestAsync<T>(requestString);
        }

        async static Task<T> APIRequestAsync<T>(string requestString) where T : new()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestString);
            webRequest.Accept = "application/json";
            HttpWebResponse response = (HttpWebResponse)await HttpExtensions.GetResponseAsync(webRequest).ConfigureAwait(false);

            T data;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                data = (T)ser.ReadObject(response.GetResponseStream());
            }
            else
            {
                data = new T();
            }
            return data;
        }
    }
}
