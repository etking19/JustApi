using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Dao
{
    public class OtpDao : BaseDao
    {
        public bool Add(string userId, string code, string referenceNum)
        {
            return true;
        }

        public bool DisableAll(string userId)
        {
            return true;
        }

        public Model.Otp Get(string userId)
        {
            Model.Otp otp = new Model.Otp();
            return otp;
        }
    }
}