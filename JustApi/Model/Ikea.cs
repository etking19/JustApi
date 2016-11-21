namespace JustApi.Model
{
    public class Ikea
    {
        public string id { get; set; }
        public IkeaProject project { get; set; }
        public User user { get; set; }
        public JobStatus jobStatus { get; set; }

        public string unitNumber { get; set; }
        public string itemUrl { get; set; }
    }

    public class IkeaProject
    {
        public string id { get; set; }
        public string name { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string postcode { get; set; }
        public State state { get; set; }
        public Country country { get; set; }
        public float gpsLatitude { get; set; }
        public float gpsLongitude { get; set; }
    }

}