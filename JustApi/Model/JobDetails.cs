﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class JobDetails
    {
        public string jobId { get; set; }
        public string ownerUserId { get; set; }
        public string jobTypeId { get; set; }
        public string fleetTypeId { get; set; }

        public float amount { get; set; }
        public float amountPartner { get; set; }
        public int distance { get; set; }

        public float amountPaid { get; set; }
        public bool cashOnDelivery { get; set; }

        // labor charges
        public int workerAssistant { get; set; }
        public int assembleBed { get; set; }
        public int assembleDiningTable { get; set; }
        public int assembleWardrobe { get; set; }
        public int assembleOfficeTable { get; set; }
        public int bubbleWrapping { get; set; }
        public int shrinkWrapping { get; set; }

        public string deliveryDate { get; set; }
        public string remarks { get; set; }
        public bool enabled { get; set; }

        public List<Address> addressFrom { get; set; }
        public List<Address> addressTo { get; set; }

        public string createdBy { get; set; }
        public string creationDate { get; set; }
        public string modifiedBy { get; set; }
        public string lastModifiedDate { get; set; }

        public string jobStatusId { get; set; }

        public bool deleted { get; set; }
    }
}