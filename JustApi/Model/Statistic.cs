using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model
{
    public class AdminStatistics
    {
        public Statistic total { get; set; }
        public Dictionary<string, Statistic> statistics { get; set; }
    }

    public class Statistic
    {
        public int numUniqueUsers { get; set; }

        public int numPartners { get; set; }
        public int numFleets { get; set; }

        public int numJobs { get; set; }
        public float numRevenues { get; set; }
        public float numProfits { get; set; }
    }
}