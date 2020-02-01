using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ReceiveGoodsController : BaseController
    {
        public ReceiveGoodsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices) : base(orderService, propertyService, accountServices, lookupServices)
        {

        }

    }
}