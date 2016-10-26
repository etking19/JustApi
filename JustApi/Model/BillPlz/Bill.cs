using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model.BillPlz
{
    public class Bill
    {
        /*
         *{
  "id": "8X0Iyzaw",
  "collection_id": "inbmmepb",
  "paid": false,
  "state": "overdue",
  "amount": 200 ,
  "paid_amount": 0,
  "due_at": "2015-3-9",
  "email" :"api@billplz.com",
  "mobile": null,
  "name": "MICHAEL API V3",
  "url": "https://www.billplz.com/bills/8X0Iyzaw",
  "reference_1_label": "Reference 1",
  "reference_1": null,
  "reference_2_label": "Reference 2",
  "reference_2": null,
  "redirect_url": null,
  "callback_url": "http://example.com/webhook/",
  "description": "Maecenas eu placerat ante."
}
     
         */


        public string id { get; set; }
        public string collection_id { get; set; }
        public bool paid { get; set; }
        public string state { get; set; }
        public float amount { get; set; }
        public float paid_amount { get; set; }
        public string due_at { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string reference_1_label { get; set; }
        public string reference_1 { get; set; }
        public string reference_2_label { get; set; }
        public string reference_2 { get; set; }
        public string redirect_url { get; set; }
        public string callback_url { get; set; }
        public string description { get; set; }
        public string paid_at { get; set; }
    }
}