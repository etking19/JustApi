using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model
{
    public class DriverLocation
    {
        public float longitude { get; set; }
        public float latitude { get; set; }
        public string lastSeen { get; set; }

        public List<DriverCompany> companies { get; set; }
        public Driver driver { get; set; }
        public List<OnGoingJob> jobs { get; set; }
    }

    public class OnGoingJob
    {
        public Fleet fleet;
        public string jobId;
    }

    public class DriverCompany
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Driver
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}