using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using JustApi.Dao;

namespace JustApi.Utility
{
    public class UserAuthHttpModule
    {
        private static UserSessionDao userSessionDao = new UserSessionDao();

        public static bool VerifyUserToken()
        {
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["X-Access-Token"];
            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("bearer",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    return AuthenticateUser(authHeaderVal.Parameter);
                }
                else
                {
                    setInvalidHeader();
                    return false;
                }
            }

            setInvalidHeader();
            return false;
        }

        private static bool AuthenticateUser(string token)
        {
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                var accessToken = encoding.GetString(Convert.FromBase64String(token));

                int separator = accessToken.IndexOf(':');
                string userId = accessToken.Substring(0, separator);
                string userToken = accessToken.Substring(separator + 1);

                var userLatestToken = userSessionDao.GetLatestToken(userId);
                if (userLatestToken == null)
                {
                    setInvalidHeader();
                    return false;
                }

                if(userLatestToken.CompareTo(userToken) != 0)
                {
                    setInvalidHeader();
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
            }

            setInvalidHeader();
            return false;
        }

        private static void setInvalidHeader()
        {
            HttpContext.Current.Response.StatusCode = 401;
            HttpContext.Current.Response.Headers.Add("WWW-Authenticate",
                string.Format("Bearer realm=\"{0}\"", "Invalid user token"));
        }
    }
}