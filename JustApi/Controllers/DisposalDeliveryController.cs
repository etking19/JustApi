using JustApi.Model;
using JustApi.Model.Delivery;

namespace JustApi.Controllers
{
    public class DisposalDeliveryController : BaseController
    {
        public Response Get(string lorryType, string fromBuildingType)
        {
            var labor = 1;
            float transportCost = 0;
            switch (int.Parse(lorryType))
            {
                case (int)Constants.Configuration.LorryType.Lorry_1tonne:
                    labor = 2;
                    transportCost = 120;
                    break;
                case (int)Constants.Configuration.LorryType.Lorry_3tonne:
                    labor = 3;
                    transportCost = 230;
                    break;
                case (int)Constants.Configuration.LorryType.Lorry_5tonne:
                    labor = 3;
                    transportCost = 300;
                    break;
                default:
                    response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EParameterError);
                    return response;
            }

            PriceDetails priceDetails = new PriceDetails();
            priceDetails.total = transportCost;

            // calculate for labors
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
                response = Utility.Utils.SetResponse(response, false, Constant.ErrorCode.EGeneralError);
                return response;
            }

            var addCost = labor * laborCost.value;
            priceDetails.total += addCost;
            priceDetails.labor = addCost;

            // breakdown cost
            priceDetails.fuel = 0.35f * transportCost;
            priceDetails.maintenance = 0.25f * transportCost;
            priceDetails.labor += 0.15f * transportCost;
            priceDetails.partner = 0.15f * transportCost;
            priceDetails.justlorry = 0.10f * transportCost;

            response.payload = javaScriptSerializer.Serialize(priceDetails);
            response = Utility.Utils.SetResponse(response, true, Constant.ErrorCode.ESuccess);
            return response;
        }
    }
}
