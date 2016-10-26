using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class JobDetails
    {
        private float _amount = 0;
        public string jobId { get; set; }
        public string ownerUserId { get; set; }
        public string jobTypeId { get; set; }
        public string fleetTypeId { get; set; }
        public float amount
        {
            get
            {
                return _amount;
            }

            set
            {
                this._amount = value;
            }
        }

        public float amountPartner
        {
            get
            {
                return _amount * 0.9f;
            }
            set
            {

            }
        }

        public float amountPaid { get; set; }
        public bool cashOnDelivery { get; set; }
        public int workerAssistant { get; set; }
        public string deliveryDate { get; set; }
        public string remarks { get; set; }
        public bool enabled { get; set; }
        public string createdBy { get; set; }
        public List<Address> addressFrom { get; set; }
        public List<Address> addressTo { get; set; }
        public string creationDate { get; set; }
        public string jobStatusId { get; set; }
        public string modifiedBy { get; set; }
        public string lastModifiedDate { get; set; }

        public bool deleted { get; set; }
    }
}