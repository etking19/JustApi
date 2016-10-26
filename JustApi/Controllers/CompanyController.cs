using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class CompanyController : BaseController
    {
        public Response Get(string companyId = null, string limit = null, string skip = null)
        {
            if (companyId != null)
            {
                var result = companyDao.GetCompanyById(companyId);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.ECompanyNotFound);
                    return response;
                }

                if (result.deleted)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.ECompanyDeleted);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }
            else
            {
                var result = companyDao.GetCompanies(limit, skip);
                if (result == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EResourceNotFoundError);
                    return response;
                }

                response.payload = javaScriptSerializer.Serialize(result);
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Put(string companyId, [FromBody]Model.Company company)
        {
            var result = companyDao.UpdateCompany(companyId, company);
            if (false == result)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Delete(string companyId)
        {
            var result = companyDao.DeleteCompany(companyId);
            if (false == result)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }

        public Response Post([FromBody]Company company)
        {
            var result = companyDao.AddCompany(company);
            if (result == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            response.payload = javaScriptSerializer.Serialize(result);
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
