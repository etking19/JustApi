using JustApi.Dao;
using JustApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace JustApi.Controllers
{
    public class BaseController : ApiController
    {
        protected Response response = new Response();

        protected JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

        // Dao classes
        protected static CommonDao commonDao = new CommonDao();

        protected static CountryDao countryDao = new CountryDao();
        protected static StateDao stateDao = new StateDao();
        protected static FleetTypeDao fleetTypeDao = new FleetTypeDao();
        protected static PickUpErrorDao pickupErrDao = new PickUpErrorDao();
        protected static DeliveryErrorDao deliveryErrDao = new DeliveryErrorDao();
        protected static JobStatusDao jobStatusDao = new JobStatusDao();
        protected static JobTypeDao jobTypeDao = new JobTypeDao();

        protected static PermissionDao permissionDao = new PermissionDao();
        protected static ActivityDao activityDao = new ActivityDao();
        protected static RoleDao roleDao = new RoleDao();

        protected static UsersDao userDao = new UsersDao();
        protected static OtpDao otpDao = new OtpDao();
        protected static CompanyDao companyDao = new CompanyDao();
        protected static FleetDao fleetDao = new FleetDao();
        protected static JobDetailsDao jobDetailsDao = new JobDetailsDao();
        protected static JobDeliveryDao jobDeliveryDao = new JobDeliveryDao();
        protected static AddressDao addressDao = new AddressDao();
        protected static JobDeliveryDeclinedDao jobDeclineDao = new JobDeliveryDeclinedDao();

        protected static SupportedAreaDao supportedAreaDao = new SupportedAreaDao();
        protected static DeliveryPriceDao deliveryPriceDao = new DeliveryPriceDao();
        protected static DeliveryAdditionalPriceDao deliveryAdditionalDao = new DeliveryAdditionalPriceDao();
        protected static VoucherDao voucherDao = new VoucherDao();

        protected static PaymentsDao paymentsDao = new PaymentsDao();
        protected static StatisticDao statisticDao = new StatisticDao();

        protected int validateVoucher(string promoCode, float totalSpending, out Vouchers result)
        {
            result = voucherDao.GetByVoucherCode(promoCode);
            if (result == null ||
                result.enabled == false)
            {
                return Constant.ErrorCode.EVoucherNotValid;
            }

            DateTime startDate = DateTime.Parse(result.startDate);
            DateTime endDate = DateTime.Parse(result.endDate);

            if (DateTime.Now.CompareTo(startDate) < 0 ||
                DateTime.Now.CompareTo(endDate) > 0)
            {
                return Constant.ErrorCode.EVoucherExpired;
            }

            if (result.minimumPurchase != -1 &&
                totalSpending < result.minimumPurchase)
            {
                return Constant.ErrorCode.EVoucherMinimumRequired;
            }

            if (result.used >= result.quantity)
            {
                return Constant.ErrorCode.EVoucherRedemptionLimit;
            }

            return Constant.ErrorCode.ESuccess;
        }

        protected bool updateUserToken(string userId)
        {
            string newToken = Guid.NewGuid().ToString();
            string newValidity = commonDao.GetCurrentUtcTime(Constants.Configuration.TOKEN_VALID_HOURS);

            return userDao.InsertOrUpdateToken(userId, newToken, newValidity);
        }
    }
}
