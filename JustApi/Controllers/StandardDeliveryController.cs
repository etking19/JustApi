using JustApi.Model;
using JustApi.Model.Delivery;
using JustApi.Utility;
using System.Web.Http;

namespace JustApi.Controllers
{
    public class StandardDeliveryController : BaseController
    {
        [NonAction]
        public PriceDetails GetPrice(string distance, string lorryType, string fromBuildingType = null, string toBuildingType = null, string labor = null,
            string assembleBed = null, string assemblyDining = null, string assemblyWardrobe = null, string assemblyTable = null, 
            string bubbleWrapping = null, string shrinkWrapping = null, string promoCode = null)
        {
            // calculate the transport cost
            var transportCost = deliveryPriceDao.GetPrice(distance, lorryType);
            if (transportCost == null)
            {
                return null;
            }

            PriceDetails priceDetails = new PriceDetails();
            priceDetails.total = transportCost.price;
            priceDetails.partnerTotal = transportCost.partner_price;

            // break down the cost
            priceDetails.fuel = 0.4f * priceDetails.partnerTotal;
            priceDetails.maintenance = 0.35f * priceDetails.partnerTotal;
            priceDetails.labor = 0.25f * priceDetails.partnerTotal;

            var additionalService = deliveryAdditionalDao.Get();
            int laborCount = 0;
            if (fromBuildingType != null &&
                toBuildingType != null &&
                labor != null &&
                int.TryParse(labor, out laborCount) &&
                laborCount != 0)
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
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, fromBuildingType + " " + toBuildingType);
                    return null;
                }

                var addCost = laborCount * laborCost.value;
                var addPartnerCost = laborCount * laborCost.partnerValue;

                priceDetails.total += addCost;
                priceDetails.partnerTotal += addPartnerCost;

                priceDetails.labor += addPartnerCost;
            }

            int assembleBedCount = 0;
            if (assembleBed != null &&
                int.TryParse(assembleBed, out assembleBedCount) &&
                assembleBedCount != 0)
            {
                var cost = additionalService.Find(t => t.name.CompareTo("assemble-bed") == 0);
                if (cost == null)
                {
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, "assembleBed: " + assembleBed);
                    return null;
                }

                var addCost = assembleBedCount * cost.value;
                var addPartnerCost = assembleBedCount * cost.partnerValue;

                priceDetails.total += addCost;
                priceDetails.partnerTotal += addPartnerCost;

                priceDetails.labor += addPartnerCost;
            }

            int assembleDiningCount = 0;
            if (assemblyDining != null &&
                int.TryParse(assemblyDining, out assembleDiningCount) &&
                assembleDiningCount != 0)
            {
                var cost = additionalService.Find(t => t.name.CompareTo("assemble-diningtable") == 0);
                if (cost == null)
                {
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, "assembleBed: " + assemblyDining);
                    return null;
                }

                var addCost = assembleDiningCount * cost.value;
                var addPartnerCost = assembleDiningCount * cost.partnerValue;

                priceDetails.total += addCost;
                priceDetails.partnerTotal += addPartnerCost;

                priceDetails.labor += addPartnerCost;
            }

            int assembleTableCount = 0;
            if (assemblyTable != null &&
                int.TryParse(assemblyTable, out assembleTableCount) &&
                assembleTableCount != 0)
            {
                var cost = additionalService.Find(t => t.name.CompareTo("assemble-table") == 0);
                if (cost == null)
                {
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, "assemblyTable: " + assemblyTable);
                    return null;
                }

                var addCost = assembleTableCount * cost.value;
                var addPartnerCost = assembleTableCount * cost.partnerValue;

                priceDetails.total += addCost;
                priceDetails.partnerTotal += addPartnerCost;

                priceDetails.labor += addPartnerCost;
            }

            int assemblyWardrobeCount = 0;
            if (assemblyWardrobe != null &&
                int.TryParse(assemblyWardrobe, out assemblyWardrobeCount) &&
                assemblyWardrobeCount != 0)
            {
                var cost = additionalService.Find(t => t.name.CompareTo("assemble-wardrobe") == 0);
                if (cost == null)
                {
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, "assemblyWardrobe: " + assemblyWardrobe);
                    return null;
                }

                var addCost = assemblyWardrobeCount * cost.value;
                var addPartnerCost = assemblyWardrobeCount * cost.partnerValue;

                priceDetails.total += addCost;
                priceDetails.partnerTotal += addPartnerCost;

                priceDetails.labor += addPartnerCost;
            }

            int bubbleWrappingCount = 0;
            if (bubbleWrapping != null &&
                int.TryParse(bubbleWrapping, out bubbleWrappingCount) &&
                bubbleWrappingCount != 0)
            {
                var cost = additionalService.Find(t => t.name.CompareTo("bubble-wrap") == 0);
                if (cost == null)
                {
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, "bubbleWrapping: " + bubbleWrapping);
                    return null;
                }

                var addCost = bubbleWrappingCount * cost.value;
                var addPartnerCost = bubbleWrappingCount * cost.partnerValue;

                priceDetails.total += addCost;
                priceDetails.partnerTotal += addPartnerCost;
            }

            int shrinkWrappingCount = 0;
            if (shrinkWrapping != null &&
                int.TryParse(shrinkWrapping, out shrinkWrappingCount) &&
                shrinkWrappingCount != 0)
            {
                var cost = additionalService.Find(t => t.name.CompareTo("shrink-wrap") == 0);
                if (cost == null)
                {
                    DBLogger.GetInstance().Log(DBLogger.ESeverity.Warning, "shrinkWrapping: " + shrinkWrapping);
                    return null;
                }

                var addCost = shrinkWrappingCount * cost.value;
                var addPartnerCost = shrinkWrappingCount * cost.partnerValue;

                priceDetails.total += addCost;
                priceDetails.partnerTotal += addPartnerCost;
            }

            priceDetails.partner = priceDetails.partnerTotal;
            priceDetails.justlorry = priceDetails.total - priceDetails.partnerTotal;

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

            if (priceDetails.total < 0)
            {
                priceDetails.total = 0;
            }

            return priceDetails;
        }

        public Response Get(string distance, string lorryType, string fromBuildingType = null, string toBuildingType = null, string labor = null,
            string assembleBed = null, string assemblyDining = null, string assemblyWardrobe = null, string assemblyTable = null,
            string bubbleWrapping = null, string shrinkWrapping = null, string promoCode = null)
        {
            // compulsory field
            if (lorryType == null ||
                distance == null)
            {
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                return response;
            }

            var priceDetails = GetPrice(distance, lorryType, fromBuildingType, toBuildingType, labor, assembleBed,
                assemblyDining, assemblyWardrobe, assemblyTable, bubbleWrapping, shrinkWrapping, promoCode);

            response.payload = javaScriptSerializer.Serialize(priceDetails);
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
