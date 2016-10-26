using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class Permission
    {
        public string permissionId;
        public string name;

        public List<Activity> activityList { get; set; }
    }
}