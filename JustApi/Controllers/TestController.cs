using JustApi.Model.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace JustApi.Controllers
{
    public class TestController : ApiController
    {
        public HttpResponseMessage Get(string jobId)
        {
            List<GpsLocation> location = new List<GpsLocation>();
            location.Add(new GpsLocation() { lat = 3.133160f, lng = 101.722306f });
            location.Add(new GpsLocation() { lat = 3.114616f, lng = 101.677515f });
            location.Add(new GpsLocation() { lat = 3.074861f, lng = 101.614154f });
            location.Add(new GpsLocation() { lat = 3.028194f, lng = 101.534769f });

            GpsLocation from = new GpsLocation() { lat = 3.114159f, lng = 101.737470f };

            // var result = Utility.Utils.GetSimpleRoute(from, from, location.ToArray(), new DateTime());
            var result = Utility.Utils.GetDistance(from, location.ToArray(), 0, Utility.Utils.GetEpochTime(new DateTime(2006, 12, 5, 12, 00, 00)));

            return new HttpResponseMessage()
            {
                Content = new StringContent(Utility.Utils.EncodeUniqueId(jobId))
            };
        }

        public HttpResponseMessage Post()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent("POST: Test message")
            };
        }

        public HttpResponseMessage Put()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent("PUT: Test message")
            };
        }

        public HttpResponseMessage Delete()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent("DELETE: Test message")
            };
        }
    }
}
