using JustApi.Model.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model.Delivery
{
    public class RouteDetails
    {
        public GpsLocation gpsLocation { get; set; }

        public float distanceFromLastLoc { get; set; }
        public DateTime estimateArrivalTime { get; set; }
    }
}