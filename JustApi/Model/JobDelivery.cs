using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class JobDelivery
    {
        public string jobDeliveryId;
        public string jobId;
        public string companyId;
        public string driverUserId;
        public string fleetId { get; set; }

        public List<JobOrderStatus> orderStatusList { get; set; }
        
        public float rating { get; set; }

        public string pickupErr { get; set; }

        public string deliverErr { get; set; }

        public string lastModifiedDate { get; set; }
    }
}