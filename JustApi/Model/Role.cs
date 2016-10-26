using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using JustApi.Utility;

namespace JustApi.Model
{
    public class Role
    {
        public string roleId;
        public string name;
        public List<Permission> permissionList { get; set; }
    }
}