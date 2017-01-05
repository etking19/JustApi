using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace JustApi.Utility
{
    public class UtilNotification
    {
        public static void SendMessage(string identifier, object extraData, string title, string message)
        {
            var obj = new
            {
                app_id = System.Configuration.ConfigurationManager.AppSettings["OneSignalAppId"],
                contents = new { en = message },
                headings = new { en = title },
                include_player_ids = new string[] { identifier },
                data = extraData
            };

            var param = new JavaScriptSerializer().Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            sendMsg(byteArray);
        }

        public static void BroadCastMessage(string[] identifiers, object extraData, string title, string message)
        {
            var obj = new
            {
                app_id = System.Configuration.ConfigurationManager.AppSettings["OneSignalAppId"],
                contents = new { en = message },
                headings = new { en = title },
                include_player_ids = identifiers,
                data = extraData
            };

            var param = new JavaScriptSerializer().Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            sendMsg(byteArray);
        }

        private static void sendMsg(byte[] byteArray)
        {
            Task.Run(async () =>
            {
                // send push notification if location was within boundary 
                var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json";

                request.Headers.Add("authorization", "Basic " + System.Configuration.ConfigurationManager.AppSettings["OneSignalAppAPI"]);

                string responseContent = null;
                try
                {
                    using (var writer = await request.GetRequestStreamAsync())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = await request.GetResponseAsync())
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                }
            });
        }
    }
}