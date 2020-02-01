using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace WMS.Controllers
{
    public class ApiMarketsController : BaseApiController
    {
        private readonly IMarketServices _marketServices;
        private readonly ITransferOrderService _transferOrderService;
        private readonly IMapper _mapper;

        public ApiMarketsController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, 
            IMarketServices marketServices, ITransferOrderService transferOrderService, IMapper mapper) 
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _marketServices = marketServices;
            _transferOrderService = transferOrderService;
            _mapper = mapper;
        }

        [HttpGet]
        public IHttpActionResult GetMyMarketJobs(DateTime reqDate, string serialNo, int userId)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            reqDate = TerminalServices.GetTerminalSyncDate(reqDate, terminal.TenantId);

            var jobs = _marketServices.GetAllResourceJobs(userId, reqDate, true);
            var result = new MarketJobsSyncCollection { Jobs = jobs.Select(m => _mapper.Map<MarketJobAllocationModel, MyJobSync>(m)).ToList(), Count = jobs.Count };
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, jobs.Count, terminal.TerminalId, TerminalLogTypeEnum.MarketJobSync).TerminalLogId;

            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeclineMarketJobRequest(MarketJobSync request)
        {
            request.SerialNumber = request.SerialNumber.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(request.SerialNumber);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var TransactionLog = TerminalServices.CheckTransactionLog(request.TransactionLogId, terminal.TerminalId);

            if (TransactionLog == true)
            {
                return Conflict();
            }

            var result = await _marketServices.DeclineMarketJob(request.MarketJobId, request.UserId, terminal.TenantId, request.Comment, terminal.TermainlSerial, request.Latitude, request.Longitude);

            request.LatestStatusID = result.MarketJobStatusId ?? 0;

            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> AcceptMarketJobRequest(MarketJobSync request)
        {
            request.SerialNumber = request.SerialNumber.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(request.SerialNumber);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var TransactionLog = TerminalServices.CheckTransactionLog(request.TransactionLogId, terminal.TerminalId);

            if (TransactionLog == true)
            {
                return Conflict();
            }

            var marketJob = _marketServices.GetMarketJobById(request.MarketJobId);

            if (marketJob.Id == 0)
            {
                return NotFound();
            }

            var result = await _marketServices.AcceptMarketJob(request.MarketJobId, request.UserId, terminal.TenantId, terminal.TermainlSerial, request.Latitude, request.Longitude);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> CompleteMarketJobRequest(MarketJobSync request)
        {
            request.SerialNumber = request.SerialNumber.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(request.SerialNumber);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var TransactionLog = TerminalServices.CheckTransactionLog(request.TransactionLogId, terminal.TerminalId);

            if (TransactionLog == true)
            {
                return Conflict();
            }

            var result = await _marketServices.CompleteMarketJob(request.MarketJobId, request.UserId, terminal.TenantId, request.Comment, terminal.TermainlSerial, request.Latitude, request.Longitude);

            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult AutoTransferVanStocks(int warehouselocationId)
        {
            _transferOrderService.AutoTransferOrdersForMobileLocations(warehouselocationId);

            return Ok(true);
        }


    }
}