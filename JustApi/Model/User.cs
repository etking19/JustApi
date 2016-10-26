using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class User
    {
        public string userId;
        public string username;
        public string password;
        public string displayName;
        public string identityCard;
        public string image;
        public string contactNumber;
        public string email;
        public bool enabled;

        public bool deleted;
        public string creationDate;

        public string lastModifiedDate;

        // only use when add, possibloe have multiple company
        public string companyId;

        // only use when add, possible have multiple role
        public string roleId;
    }
}