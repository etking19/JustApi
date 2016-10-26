using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace JustApi.Utility
{
    public class UtilSms
    {
        public static void SendSms(string phoneNumber, string message)
        {
            sendSMSToURL("http://www.isms.com.my/isms_send.php?un=" + ConfigurationManager.AppSettings.Get("ismsUsername")
                        + "&pwd=" + ConfigurationManager.AppSettings.Get("ismsPassword")
                        + "&dstno=" + phoneNumber + "&msg=" + message + "&type=1");
        }

        private static string sendSMSToURL(string getUri)
        {
            string SentResult = String.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUri);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader responseReader = new StreamReader(response.GetResponseStream());

            String resultmsg = responseReader.ReadToEnd();
            responseReader.Close();

            int StartIndex = 0;
            int LastIndex = resultmsg.Length;

            if (LastIndex > 0)
                SentResult = resultmsg.Substring(StartIndex, LastIndex);

            responseReader.Dispose();

            return SentResult;
        }
    }
}