using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model.Delivery
{
    public class PriceDetails
    {
        public float total { get; set; }
        public float partnerTotal { get; set; }

        public float labor { get; set; }
        public float fuel { get; set; }
        public float maintenance { get; set; }
        public float partner { get; set; }
        public float justlorry { get; set; }

        public float discount { get; set; }
        public int discountRate { get; set; }
    }
}