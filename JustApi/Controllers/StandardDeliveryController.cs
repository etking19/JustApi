using JustApi.Model;
using JustApi.Model.Delivery;
using JustApi.Utility;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class StandardDeliveryController : BaseController
    {
        public Response Get(string distance, string lorryType, string fromBuildingType, string toBuildingType, string labor,
            string assembleBed, string assemblyDining, string assemblyWardrobe, string assemblyTable, string promoCode)
        {
            // modify the tonne pass in to type id accordingly
            var lorryId = 0;
            switch (int.Parse(lorryType))
            {
                case (int)Constants.Configuration.LorryType.Lorry_1tonne:
                    lorryId = 1;
                    break;
                case (int)Constants.Configuration.LorryType.Lorry_3tonne:
                    lorryId = 2;
                    break;
                case (int)Constants.Configuration.LorryType.Lorry_5tonne:
                    lorryId = 3;
                    break;
                default:
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                    return response;
            }

            var transportCost = deliveryPriceDao.GetPrice(distance, lorryId.ToString());
            if (transportCost == 0)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                return response;
            }

            PriceDetails priceDetails = new PriceDetails();
            priceDetails.total = transportCost;

            var additionalService = deliveryAdditionalDao.Get();
            if (fromBuildingType != null &&
                toBuildingType != null &&
                labor != null)
            {
                DeliveryExtraService laborCost = null;

                // extra labor cost
                if (int.Parse(fromBuildingType) == (int)Constants.Configuration.BuildingType.HighRise_nolift ||
                    int.Parse(toBuildingType) == (int)Constants.Configuration.BuildingType.HighRise_nolift)
                {
                    laborCost = additionalService.Find(t => t.name.CompareTo("labor-highrise-nolift") == 0);
                }
                else if (int.Parse(fromBuildingType) == (int)Constants.Configuration.BuildingType.HighRise_lift ||
                    int.Parse(toBuildingType) == (int)Constants.Configuration.BuildingType.HighRise_lift)
                {
                    laborCost = additionalService.Find(t => t.name.CompareTo("labor-highrise-lift") == 0);
                }
                else
                {
                    laborCost = additionalService.Find(t => t.name.CompareTo("labor-landed") == 0);
                }

                if (laborCost == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                var addCost = int.Parse(labor) * laborCost.value;
                priceDetails.total += addCost;
                priceDetails.labor = addCost;
            }

            if (assembleBed != null)
            {
                var cost = additionalService.Find(t => t.name.CompareTo("assemble-bed") == 0);
                if (cost == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                var addCost = int.Parse(assembleBed) * cost.value;
                priceDetails.total += addCost;
                priceDetails.labor += addCost;
            }

            if (assemblyDining != null)
            {
                var cost = additionalService.Find(t => t.name.CompareTo("assemble-diningtable") == 0);
                if (cost == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                var addCost = int.Parse(assemblyDining) * cost.value;
                priceDetails.total += addCost;
                priceDetails.labor += addCost;
            }


            if (assemblyTable != null)
            {
                var cost = additionalService.Find(t => t.name.CompareTo("assemble-table") == 0);
                if (cost == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                var addCost = int.Parse(assemblyTable) * cost.value;
                priceDetails.total += addCost;
                priceDetails.labor += addCost;
            }

            if (assemblyWardrobe != null)
            {
                var cost = additionalService.Find(t => t.name.CompareTo("assemble-wardrobe") == 0);
                if (cost == null)
                {
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                    return response;
                }

                var addCost = int.Parse(assemblyTable) * cost.value;
                priceDetails.total += addCost;
                priceDetails.labor += addCost;
            }

            // break down the cost
            priceDetails.fuel = 0.35f * transportCost;
            priceDetails.maintenance = 0.25f * transportCost;
            priceDetails.labor += 0.15f * transportCost;
            priceDetails.partner = 0.15f * transportCost;
            priceDetails.justlorry = 0.10f * transportCost;

            // discount voucher
            if (promoCode != null)
            {
                var voucherResult = new Vouchers();
                var responseCode = validateVoucher(promoCode, priceDetails.total, out voucherResult);
                if (responseCode == Constant.ErrorCode.ESuccess)
                {
                    // first set the used voucher count
                    if (voucherDao.IncreaseUsedCount(promoCode) == false)
                    {
                        DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, "voucherDao.IncreaseUsedCount(promoCode) in Common controller: " + promoCode);
                    }

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

            if (priceDetails.total < 0)
            {
                priceDetails.total = 0;
            }

            response.payload = javaScriptSerializer.Serialize(priceDetails);
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
