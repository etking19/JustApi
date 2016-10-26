using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model
{
    public class Vouchers
    {
        public string id { get; set; }
        public string name { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public VoucherType voucherType { get; set; }
        public float discountValue { get; set; }
        public bool enabled { get; set; }
        public float minimumPurchase { get; set; }
        public float maximumDiscount { get; set; }
        public string code { get; set; }
        public int quantity { get; set; }
        public int used { get; set; }
        public string creationDate { get; set; }
    }

    public class VoucherType
    {
        public string id { get; set; }
        public string name { get; set; }
    }

}