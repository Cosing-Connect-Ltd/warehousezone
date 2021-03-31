using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Elmah;
using Ganedata.Core.Services;
using Newtonsoft.Json;
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
        [HttpPost]
        public IHttpActionResult Create(StripePaymentRequestModel model)
        {
            var result = _stripePaymentService.CreatePayment(model);
            return Ok(result);
        }
        //api/stripe/charge-order
        [HttpPost]
        public IHttpActionResult Charge(StripePaymentChargeCapture model)
        {
            var result = _stripePaymentService.ChargeOrder(model);
            return Ok(result);
        }

        [HttpPost]
        ///api/stripe/chargehook
        public IHttpActionResult WebhookReceive(StripeWebhookRequest model)
        {
            var result = _stripePaymentService.ProcessWebhook(model);
            return Ok(result);
        }


    }
}