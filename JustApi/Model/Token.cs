using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using JustApi.Utility;

namespace JustApi.Model
{
    public class Token
    {
        public Model.User user { get; set; }
        public string token { get; set; }
        public string validTill { get; set; }

        public List<Model.Company> companyList { get; set; }
    }
}