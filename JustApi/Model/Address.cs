using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class Address
    {
        public string addressId;
        public string address1;
        public string address2;
        public string address3;
        public string stateId;
        public string countryId;
        public string postcode;
        public float gpsLongitude;
        public float gpsLatitude;
        public string contactPerson;
        public string contact;

        public string createdBy;

        public string creationDate;

        public string lastModifiedDate;
    }
}