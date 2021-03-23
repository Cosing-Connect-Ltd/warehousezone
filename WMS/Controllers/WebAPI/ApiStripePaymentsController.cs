using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Ganedata.Core.Services;
using Stripe;
using Stripe.Checkout;

namespace WMS.Controllers.WebAPI
{
    public class ApiStripePaymentsController : BaseApiController
    {
        private readonly IStripePaymentService _stripePaymentService;

        public ApiStripePaymentsController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IStripePaymentService stripePaymentService) : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _stripePaymentService = stripePaymentService;
        }

        //api/stripe/create-intent  
        //[System.Web.Mvc.HttpPost]
        public IHttpActionResult Create()
        {
            var model = new StripePaymentRequestModel()
            {
                UserId = 25,
                PersonalReferralCode = "riyaz@ganedata.co.uk",
                Amount = 25.2m,
                SerialNumber = "123456789",
                ChargeToken = "tok_1IYCb8HE8E2m6CoU6ukzLvpf"
            };
            var result = _stripePaymentService.CreatePayment(model);
            return Ok(result);
        }
    }
}