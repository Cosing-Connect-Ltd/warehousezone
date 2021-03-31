using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
        public async Task<IHttpActionResult> WebhookReceive()
        {
            var requestJson = await Request.Content.ReadAsStringAsync();

            try
            {
                ErrorSignal.FromCurrentContext().Raise(new Exception(requestJson));

                var stripeEvent = EventUtility.ParseEvent(requestJson);
                 
                if (stripeEvent.Type == Events.ChargePending || stripeEvent.Type == Events.ChargeCaptured ||
                    stripeEvent.Type == Events.ChargeSucceeded || stripeEvent.Type == Events.ChargeRefunded ||
                    stripeEvent.Type == Events.ChargeFailed)
                {
                    var charge = stripeEvent.Data.Object as Charge;
                    if (charge != null)
                    {

                        _stripePaymentService.UpdateChargeInformation(stripeEvent.Type, charge.Id);
                    }
                    return Ok("Success");
                }
                else  
                {
                    ErrorSignal.FromCurrentContext().Raise(new Exception("Unhandled Event : "+ stripeEvent.Type + "; Content: " + requestJson));
                }
                return Ok("Success - Event not handled");
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

    }
}