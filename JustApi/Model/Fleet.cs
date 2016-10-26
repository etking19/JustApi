using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class Fleet
    {
        public string fleetId;
        public string registrationNumber;
        public string fleetTypeId;
        public string roadTaxExpiry;
        public string serviceDueDate;
        public int serviceDueMileage;
        public string companyId;
        public string remarks;

        public bool enabled;

        public bool deleted;

        public string creationDate;

        public string lastModifiedDate;
    }
}