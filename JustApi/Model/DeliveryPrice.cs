using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model
{
    public class DeliveryPrice
    {
        public string id { get; set; }

        public int min { get; set; }
        public int max { get; set; }

        public int fleet_type_id { get; set; }

        public float price { get; set; }
        public float partner_price { get; set; }
        public float partner_bonus { get; set; }

        public string last_modified { get; set; }
    }
}