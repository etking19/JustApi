using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model.Google
{
    public class MapsGeocode
    {
        public List<AddressComponents> results;
        public string status;
    }

    public class AddressComponents
    {
        public List<Address> address_components;
        public string formatted_address;
        public string place_id;
        public string[] types;
        public Geometry geometry;
    }

    public class Address
    {
        public string long_name;
        public string short_name;
        public string[] types;
    }

    public class Geometry
    {
        public Bounds bounds;
        public GpsLocation location;
        public string location_type;
        public Bounds viewport;
        public string place_id;
        public string[] types;
    }

    public class Bounds
    {
        public GpsLocation northeast;
        public GpsLocation southwest;
    }

    public class GpsLocation
    {
        public float lat;
        public float lng;
    }

}