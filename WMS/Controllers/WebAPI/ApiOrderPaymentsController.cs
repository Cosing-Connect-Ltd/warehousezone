using System.Web.Http;
using AutoMapper;
using Ganedata.Core.Models;
using Ganedata.Core.Models.PaypalPayments;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiOrderPaymentsController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IPaypalPaymentServices _paypalPaymentServices;
        private readonly IMapper _mapper;

        public ApiOrderPaymentsController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IPaypalPaymentServices paypalPaymentServices, IMapper mapper) : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _orderService = orderService;
            _paypalPaymentServices = paypalPaymentServices;
            _mapper = mapper;
        }

        [HttpPost]
        public IHttpActionResult AuthorizePaypalPayment(PaypalPaymentAuthorisationRequest model)
        {
            var result = _paypalPaymentServices.SubmitPaypalAuthorisation(model);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult ReceivePaypalWebhook(PaypalPaymentWebhookRequest model)
        {
            var result = _paypalPaymentServices.ReceiveWebHook(model);

            return Ok(result);
        }

        [HttpGet]
      public IHttpActionResult GetOrderStatus(int id)
      {
          var result = _orderService.GetOrderById(id);
          var order = new OrdersSync();
          _mapper.Map(result, order);
            return Ok(order);
        }

    }
}