using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class State
    {
        public string stateId { get; set; }
        public string name { get; set; }
        public string countryId { get; set; }
    }
}