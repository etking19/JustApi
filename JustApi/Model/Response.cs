using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace JustApi.Model
{
    public class Response
    {
        public bool success;
        public int errorCode;
        public string errorMessage;
        public object payload;
    }
}