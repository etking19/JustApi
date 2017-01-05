using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model.Google
{
    public class DistanceMatrix
    {
        public string status { get; set; }
        public List<string> origin_addresses { get; set; }
        public List<string> destination_addresses { get; set; }
        public List<DistanceMatrixRow> rows { get; set; }
    }

    public class DistanceMatrixRow
    {
        public List<DistanceMatrixElement> elements { get; set; }
    }

    public class DistanceMatrixElement
    {
        public string status { get; set; }
        public DistanceMatEleDuration duration { get; set; }
        public DistanceMatEleDistance distance { get; set; }
    }

    public class DistanceMatEleDuration
    {
        public string value { get; set; }
        public string text { get; set; }
    }

    public class DistanceMatEleDistance
    {
        public string value { get; set; }
        public string text { get; set; }
    }
}