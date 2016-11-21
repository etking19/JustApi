using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace JustApi.Controllers
{
    public class TestController : ApiController
    {
        public HttpResponseMessage Get(string jobId)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(Utility.Utils.EncodeUniqueId(jobId))
            };
        }

        public HttpResponseMessage Post()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent("POST: Test message")
            };
        }

        public HttpResponseMessage Put()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent("PUT: Test message")
            };
        }

        public HttpResponseMessage Delete()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent("DELETE: Test message")
            };
        }
    }
}
