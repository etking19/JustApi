using Newtonsoft.Json;
using System.Net.Http;
using JustApi.Model;
using JustApi.Model.Google;
using System;
using JustApi.Model.Delivery;
using System.Collections.Generic;

namespace JustApi.Utility
{
    public class Utils
    {
        public static Response SetResponse(Response response, bool success, int errorCode)
        {
            response.success = success;
            response.errorCode = errorCode;
            response.errorMessage = Constant.ErrorMsg.GetInstance().GetErrorMsg(errorCode);

            if(success == false)
            {
                response.payload = null;
            }
            return response;
        }

        public static bool ContainsAny(string haystack, string[] needles)
        {
            foreach (string needle in needles)
            {
                if (haystack.Contains(needle))
                    return true;
            }

            return false;
        }

        public static AddressComponents GetGpsCoordinate(string address)
        {
            string result = "";
            address = address.Replace(" ", "+");
            string url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?region=my&address={0}", address);
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    result = responseContent.ReadAsStringAsync().Result;
                }
            }

            MapsGeocode jsonObj = JsonConvert.DeserializeObject<MapsGeocode>(result);
            if (jsonObj.status.CompareTo("OK") != 0)
            {
                return null;
            }

            var representAddFrom = jsonObj.results.Find(t => t.formatted_address.Contains("Malaysia"));
            if (representAddFrom == null)
            {
                return null;
            }

            return representAddFrom;
        }

        public static AddressComponents GetGpsCoordinate(string add1, string add2, string add3, string poscode)
        {
            string result = "";
            string address = add1 + ", " + add2 + ", " + add3;
            int retryCount = 3;
            do {
                address = address.Replace(" ", "+");
                string url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?region=my&address={0}", address);
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // by calling .Result you are performing a synchronous call
                        var responseContent = response.Content;

                        // by calling .Result you are synchronously reading the result
                        result = responseContent.ReadAsStringAsync().Result;
                    }
                }

                MapsGeocode jsonObj = JsonConvert.DeserializeObject<MapsGeocode>(result);
                do
                {
                    if (jsonObj.status.CompareTo("OK") != 0)
                    {
                        break;
                    }

                    var representAddFrom = jsonObj.results.Find(t => t.formatted_address.Contains("Malaysia"));
                    if (representAddFrom == null)
                    {
                        break;
                    }

                    return representAddFrom;

                } while (true);

                switch (retryCount)
                {
                    case 3:
                        address = add2 + ", " + add3;
                        break;
                    case 2:
                        address = add3;
                        break;
                    case 1:
                        address = poscode;
                        break;
                }
                retryCount--;

            } while (retryCount >= 0) ;

            return null;
        }

        public static DistanceMatrix GetDistance(GpsLocation from, GpsLocation[] to, int departureTime = 0, int arrivalTime = 0)
        {
            // https://maps.googleapis.com/maps/api/distancematrix/json?
            // units =imperial&origins=40.6655101,-73.89188969999998&
            // destinations =40.6905615%2C-73.9976592%7C40.6905615%2C-73.9976592%7C40.6905615%2C-73.9976592%7C40.6905615%2C-73.9976592%7C40.6905615%2C-73.9976592%7C40.6905615%2C-73.9976592%7C40.659569%2C-73.933783%7C40.729029%2C-73.851524%7C40.6860072%2C-73.6334271%7C40.598566%2C-73.7527626%7C40.659569%2C-73.933783%7C40.729029%2C-73.851524%7C40.6860072%2C-73.6334271%7C40.598566%2C-73.7527626&
            // key =YOUR_API_KEY

            string origin = string.Format("{0},{1}", from.lat, from.lng);

            string destinationLoc = "";
            foreach (GpsLocation gpsLoc in to)
            {
                destinationLoc += string.Format("{0},{1}|", gpsLoc.lat, gpsLoc.lng);
            }
            destinationLoc.Remove(destinationLoc.Length - 1);

            var fullUrl = string.Format("https://maps.googleapis.com/maps/api/distancematrix/json?origins={0}&destinations={1}&key={2}",
                origin, destinationLoc, System.Configuration.ConfigurationManager.AppSettings["GoogleApiKey"]);

            if (departureTime != 0)
            {
                fullUrl += string.Format("&departure_time={0}", departureTime);
            }

            if (arrivalTime != 0)
            {
                fullUrl += string.Format("&arrival_time={0}", arrivalTime);
            }

            var result = "";
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(fullUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    result = responseContent.ReadAsStringAsync().Result;
                }
            }

            return JsonConvert.DeserializeObject<DistanceMatrix>(result);
        }

        public static int GetEpochTime(DateTime time)
        {
            TimeSpan t = time - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }

        public static RouteDetails[] GetSimpleRoute(GpsLocation from, GpsLocation to, GpsLocation[] location, DateTime departureTime)
        {
            // https://maps.googleapis.com/maps/api/directions/json?origin=3.138501,%20101.595983&
            // destination =3.138501,101.595983&waypoints=optimize:true|3.083020,101.720428|3.070053,101.606896|3.131532,101.625198|3.101207,101.612382
            // &key=AIzaSyArNVgfBbl4WUqHdu-ycAE-pL0TGvok8Us

            string waypointLoc = "waypoints=optimize:true";
            foreach (GpsLocation gpsLoc in location)
            {
                waypointLoc += string.Format("|{0},{1}", gpsLoc.lat, gpsLoc.lng);
            }

            string fullUrl = string.Format("https://maps.googleapis.com/maps/api/directions/json?origin={0},{1}&destination={2},{3}&{4}&key={5}", 
                from.lat, from.lng, to.lat, to.lng, waypointLoc, System.Configuration.ConfigurationManager.AppSettings["GoogleApiKey"]);

            var result = "";
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(fullUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    result = responseContent.ReadAsStringAsync().Result;
                }
            }

            Direction jsonObj = JsonConvert.DeserializeObject<Direction>(result);

            List<RouteDetails> routeDetailsList = new List<RouteDetails>();
            foreach (int orderId in jsonObj.routes[0].waypoint_order)
            {
                routeDetailsList.Add(new RouteDetails() { gpsLocation = location[orderId] });
            }

            return routeDetailsList.ToArray();
        }

        public static int GetSimpleRouteDistance(GpsLocation from, GpsLocation to, GpsLocation[] location)
        {
            // https://maps.googleapis.com/maps/api/directions/json?origin=3.138501,%20101.595983&
            // destination =3.138501,101.595983&waypoints=optimize:true|3.083020,101.720428|3.070053,101.606896|3.131532,101.625198|3.101207,101.612382
            // &key=AIzaSyArNVgfBbl4WUqHdu-ycAE-pL0TGvok8Us

            string waypointLoc = "waypoints=optimize:true";
            foreach (GpsLocation gpsLoc in location)
            {
                waypointLoc += string.Format("|{0},{1}", gpsLoc.lat, gpsLoc.lng);
            }

            string fullUrl = string.Format("https://maps.googleapis.com/maps/api/directions/json?origin={0},{1}&destination={2},{3}&{4}&key={5}",
                from.lat, from.lng, to.lat, to.lng, waypointLoc, System.Configuration.ConfigurationManager.AppSettings["GoogleApiKey"]);

            var result = "";
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(fullUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    result = responseContent.ReadAsStringAsync().Result;
                }
            }

            Direction jsonObj = JsonConvert.DeserializeObject<Direction>(result);

            var distance = 0;
            foreach (Route route in jsonObj.routes)
            {
                foreach (Leg leg in route.legs)
                {
                    distance += leg.duration.value;
                }
            }

            return distance;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string DecodeUniqueId(string uniqueId)
        {
            return (Utility.IdGenerator.Decode(uniqueId) - 999999).ToString();
        }

        public static string EncodeUniqueId(string jobId)
        {
            return Utility.IdGenerator.Encode(ulong.Parse(jobId) + 999999);
        }

    }
}