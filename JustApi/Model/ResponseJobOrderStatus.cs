using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class ResponseJobOrderStatus
    {
        public string job_id { get; set; }
        public bool enabled { get; set; }
        public bool deleted { get; set; }
        public List<JobOrderStatus> orderStatus { get; set; }
    }
}