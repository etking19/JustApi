using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace JustApi.Helper
{
    public class PushNotification
    {
        public enum ECategories
        {
            NewOpenJob = 1,
            OrderCreated = 2,
            OrderStatusUpdate = 3,
            RatingUpdate = 4,
            JobAssigned = 5
        }

        public struct ExtraData
        {
            public string category { get; set; }
            public string data { get; set; }
        }

        /// <summary>
        /// JSON data format of {"category" : "1", "data" : "xxx"}
        /// where xxx depends of category:
        /// NewOpenJob - job id, app should regconize and go to the open job page (partner)
        /// OrderCreated - order id, app should regconize and go to order status page (consumer)
        /// OrderStatusUpdate - order id, app should regconize and go to order status page (consumer)
        /// RatingUpdate - job id, app should open the job details page on the delivery (partner)
        /// JobAssigned - job id, app should open the job details page on the delivery (driver)
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static object ConstructExtraData(ECategories category, string data)
        {
            return new ExtraData() { category = ((int)category).ToString(), data = data };
        }
    }
}