using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Constants
{
    public class Configuration
    {
        public static int TOKEN_VALID_HOURS = 10;
        public static int JOB_STATUS_PAID = 3;


        public string CONNECTION_STRING
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["JustGoNetworkDB"].ConnectionString;
            }
        }

        public enum BuildingType
        {
            HighRise_lift = 1,
            HighRise_nolift = 2,
            Landed = 3
        };

        public enum VoucherType
        {
            Percentage = 1,
            Value = 2
        };

        public enum Role
        {
            MasterAdmin = 1,
            CompanyAdmin = 2,
            CompanyDriver = 3,
            CorporateAdmin = 4,
            CorporateClerk = 5,
            Finance = 6,
            PublicUsers = 7,
            Clerk = 8
        }

        public enum DeliveryJobType
        {
            Standard = 1,
            Disposal = 2
        }

        public enum JobStatus
        {
            PendingPayment = 1,
            PaymentVerifying = 2,
            OrderReceived = 3,
            Ongoing_Pickup = 4,
            Ongoing_Delivery = 5,
            Delivered = 6,
            Error_Pickup = 7,
            Error_Delivery = 8
        }
    }
}