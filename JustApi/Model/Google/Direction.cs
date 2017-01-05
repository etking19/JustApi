using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model.Google
{
    public class Direction
    {
        public List<GeocodedWaypoint> geocoded_waypoints { get; set; }
        public List<Route> routes { get; set; }
        public string status { get; set; }
    }

    public class GeocodedWaypoint
    {
    }

    public class Route
    {
        public Bound bounds { get; set; }
        public List<Leg> legs { get; set; }
        public List<int> waypoint_order;
    }

    public class Bound
    {
        public Coordinate northeast { get; set; }
        public Coordinate southwest { get; set; }
    }

    public class Coordinate
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Leg
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public List<Steps> steps { get; set; }
    }

    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Steps
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }

    }









}