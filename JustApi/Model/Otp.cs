using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustApi.Model
{
    public class Otp
    {
        public string otpId;

        public string userId;

        public string otpCode;

        public string refNumber;

        public bool isUsed;

        public string creationDate;
    }
}