using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DevExpress.CodeParser;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ApiStockTakesController : BaseApiController
    {
        public IStockTakeApiService StockTakeApiService;
        public ApiStockTakesController(IStockTakeApiService stockTakeApiService, ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices,
            IUserService userService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            StockTakeApiService = stockTakeApiService;
        }


        [ResponseType(typeof(StockTakeProductCodeScanResponse))]
        public async Task<IHttpActionResult> RecordScannedProducts(StockTakeProductCodeScanRequest request)
        {
            if (!string.IsNullOrEmpty(request.SerialNo))
            {
                var terminal = TerminalServices.GetTerminalBySerial(request.SerialNo);

                if (terminal == null)
                {
                    return Unauthorized();
                }

                var TransactionLog = TerminalServices.CheckTransactionLog(request.TransactionLogId, terminal.TerminalId);

                if (TransactionLog == true)
                {
                    return Conflict();
                }
            }

            if (request.ScannedQuantity == 0) { request.ScannedQuantity = 1; }

            var result = await StockTakeApiService.RecordScannedProducts(request);

            return Ok(result);
        }

        [ResponseType(typeof(ResponseObject))]
        public async Task<IHttpActionResult> UpdateStockTakeDetailQuantity(StockDetailQuantityUpdateRequest request)
        {

            if (!string.IsNullOrEmpty(request.SerialNo))
            {
                var terminal = TerminalServices.GetTerminalBySerial(request.SerialNo);

                if (terminal == null)
                {
                    return Unauthorized();
                }

                var TransactionLog = TerminalServices.CheckTransactionLog(request.TransactionLogId, terminal.TerminalId);

                if (TransactionLog == true)
                {
                    return Conflict();
                }
            }


            var result = await StockTakeApiService.UpdateStockTakeDetailQuantity(request);

            return Ok(result);
        }

        public async Task<IHttpActionResult> ArchiveStockTakeDetail(StockDetailDeleteRequest request)
        {
            var result = await StockTakeApiService.DeleteStockTakeDetail(request);

            return Ok(result);
        }

        public async Task<IHttpActionResult> CreateProductOnStockTake(ProductDetailRequest request)
        {
            var result = await StockTakeApiService.CreateProductOnStockTake(request);

            return Ok(result);
        }

    }
}