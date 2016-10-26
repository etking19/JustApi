using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{

    public class Company
    {
        public string name;
        public string address1;
        public string address2;
        public string postcode;
        public string stateId;
        public string countryId;
        public string registrationNumber;

        public string companyId;

        public bool enabled;

        public bool deleted;

        public string creationDate;

        public string lastModifiedDate;

        public float rating;

        public User[] admin;

        public List<Model.Role> rolePermissionList { get; set; }
    }
}