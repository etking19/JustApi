using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class Address
    {
        public string addressId { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string stateId { get; set; }
        public string countryId { get; set; }
        public string postcode { get; set; }
        public float gpsLongitude { get; set; }
        public float gpsLatitude { get; set; }
        public string contactPerson { get; set; }
        public string contact { get; set; }

        public string buildingType { get; set; }
        public string createdBy { get; set; }
        public string creationDate { get; set; }
        public string lastModifiedDate { get; set; }
    }
}