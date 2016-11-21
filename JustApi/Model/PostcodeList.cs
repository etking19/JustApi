using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model
{
    public class PostcodeList
    {
        public string postCodeAddFrom { get; set; }
        public Google.Bounds fromBound { get; set; }

        public string postCodeAddTo { get; set; }
        public Google.Bounds toBound { get; set; }

        public int distance { get; set; }
        public int duration { get; set; }
    }
}