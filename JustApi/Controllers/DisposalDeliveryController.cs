using JustApi.Model;
using JustApi.Model.Delivery;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class DisposalDeliveryController : BaseController
    {
        [NonAction]
        public PriceDetails GetPrice(string lorryType, string fromBuildingType, string promoCode)
        {
            var transportCost = deliveryPriceDao.GetPrice("0", lorryType);
            if (transportCost == null)
            {
                return null;
            }

            PriceDetails priceDetails = new PriceDetails();
            priceDetails.total = (transportCost.price * 0.8f);
            priceDetails.partnerTotal = (transportCost.partner_price * 0.8f);

            // breakdown cost
            priceDetails.fuel = 0.4f * priceDetails.partnerTotal;
            priceDetails.maintenance = 0.35f * priceDetails.partnerTotal;
            priceDetails.labor = 0.25f * priceDetails.partnerTotal;

            // calculate for labors
            var laborCount = 1;
            switch (lorryType)
            {
                case "1":
                    laborCount = 2;
                    break;
                case "2":
                case "3":
                    laborCount = 3;
                    break;
            }

            var additionalService = deliveryAdditionalDao.Get();
            DeliveryExtraService laborCost = null;

            // extra labor cost
            if (fromBuildingType == null)
            {
                fromBuildingType = "1";
            }
            if (int.Parse(fromBuildingType) == (int)Constants.Configuration.BuildingType.HighRise_nolift)
            {
                laborCost = additionalService.Find(t => t.name.CompareTo("labor-highrise-nolift") == 0);
            }
            else if (int.Parse(fromBuildingType) == (int)Constants.Configuration.BuildingType.HighRise_lift)
            {
                laborCost = additionalService.Find(t => t.name.CompareTo("labor-highrise-lift") == 0);
            }
            else
            {
                laborCost = additionalService.Find(t => t.name.CompareTo("labor-landed") == 0);
            }

            if (laborCost == null)
            {
                return null;
            }

            var laborCosting = laborCount * laborCost.value;
            var partnerLaborCosting = laborCount * laborCost.partnerValue;

            priceDetails.total += laborCosting;
            priceDetails.partnerTotal += partnerLaborCosting;
            priceDetails.labor += partnerLaborCosting;

            priceDetails.partner = priceDetails.partnerTotal;
            priceDetails.justlorry = (priceDetails.total - priceDetails.partnerTotal);

            // discount voucher
            if (promoCode != null)
            {
                var voucherResult = new Vouchers();
                var responseCode = validateVoucher(promoCode, priceDetails.total, out voucherResult);
                if (responseCode == Constant.ErrorCode.ESuccess)
                {
                    // first set the used voucher count
                    // 20161118 - increase count during confirm (add job) instead
                    //if (voucherDao.IncreaseUsedCount(promoCode) == false)
                    //{
                    //    DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, "voucherDao.IncreaseUsedCount(promoCode) in Common controller: " + promoCode);
                    //}

                    if (int.Parse(voucherResult.voucherType.id) == (int)Constants.Configuration.VoucherType.Percentage)
                    {
                        var discountedValue = voucherResult.discountValue * priceDetails.total;
                        if (discountedValue > voucherResult.maximumDiscount)
                        {
                            discountedValue = voucherResult.maximumDiscount;
                        }

                        priceDetails.total -= discountedValue;
                        priceDetails.discount = discountedValue;
                        priceDetails.discountRate = (int)voucherResult.discountValue;
                    }
                    else if (int.Parse(voucherResult.voucherType.id) == (int)Constants.Configuration.VoucherType.Value)
                    {
                        var discountedValue = voucherResult.discountValue;
                        if (discountedValue > voucherResult.maximumDiscount)
                        {
                            discountedValue = voucherResult.maximumDiscount;
                        }

                        priceDetails.discountRate = (int)(voucherResult.discountValue / priceDetails.total);
                        priceDetails.total -= discountedValue;
                        priceDetails.discount = discountedValue;
                    }
                }
            }

            return priceDetails;
        }

        public Response Get(string lorryType, string fromBuildingType, string promoCode = null)
        {
            var priceDetails = GetPrice(lorryType, fromBuildingType, promoCode);

            response.payload = javaScriptSerializer.Serialize(priceDetails);
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
