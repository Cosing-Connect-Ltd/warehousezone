using System.Web.Http;
using System.Web.Mvc;
using Braintree;
using Ganedata.Core.Models.PaypalPayments;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiPaypalBraintreeController : BaseApiController
    {
        private readonly IPaypalPaymentServices _paypalService;

        public ApiPaypalBraintreeController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IPaypalPaymentServices paypalService) : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _paypalService = paypalService;
        }

        public IHttpActionResult AuthorisePayments()
        {
            var sampleRequest = new PaypalPaymentAuthorisationRequest()
            {
                AuthorisationNonceCode = "819fa56a-ffe9-0606-7f33-0fe4c96275d0",
                PaymentAmount = 29.94m,
                PaymentReference = "RIYAZ TEST",
                UserId = 51,
                OrderID = 990
            };
            var status = _paypalService.SubmitPaypalAuthorisation(sampleRequest);
            return Ok(status);
        }

        public void PaymentWebhook(PaypalPaymentWebhookRequest hook)
        {

            //_paypalService.SubmitPaypalAuthorisation();
            //return new HttpStatusCodeResult(200);
        }
    }
}