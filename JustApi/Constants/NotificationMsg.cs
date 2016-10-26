using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Constant
{
    public class NotificationMsg
    {
        public static string NewJob_Desc = "Thanks for using JustLorry.com.\nWe have received your order.\nYou may check your order status using order id below: ";
        public static string NewJob_Title = "JustLorry.com - New order";

        public static string NewOpenJob_Title = "JustLorry.com - New open job!";
        public static string NewOpenJob_Desc = "";

        public static string JobStatusUpdate_Title = "JustLorry.com - Order status update";
        public static string JobStatusUpdate_Desc = "Your oder status has been update.\nPlease check latest status using order id: ";

        public static string JobRating_Title = "JustLorry.com - Job rating updated!";
        public static string JobRating_Desc = "Customer has rate @rating for job id: @jobId, deliver from @from. Good job!";
    }
}