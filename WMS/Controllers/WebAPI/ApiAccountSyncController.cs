using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Web.Mvc;
using AutoMapper;

namespace WMS.Controllers.WebAPI
{
    public class ApiAccountSyncController : BaseApiController
    {
        private readonly IAccountServices _accountServices;
        private readonly IProductPriceService _productPriceService;
        private readonly IMapper _mapper;

        public ApiAccountSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, IAccountServices accountServices, IProductPriceService productPriceService, IMapper mapper)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _accountServices = accountServices;
            _productPriceService = productPriceService;
            _mapper = mapper;
        }
        // GET http://localhost:8005/api/sync/accounts/{reqDate}/{serialNo}
        // GET http://localhost:8005/api/sync/accounts/2014-11-23/920013c000814
        [ValidateInput(false)]
        public IHttpActionResult GetAccounts(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new AccountSyncCollection();

            var allAccounts = _accountServices.GetAllValidAccounts(terminal.TenantId, EnumAccountType.All, null, reqDate, true);

            var accounts = new List<AccountSync>();

            foreach (var p in allAccounts)
            {
                var account = new AccountSync();
                var mappedAccount = _mapper.Map(p, account);
                mappedAccount.CountryName = p.GlobalCountry.CountryName;
                mappedAccount.CurrencyName = p.GlobalCurrency.CurrencyName;
                mappedAccount.PriceGroupName = p.TenantPriceGroups.Name;
                mappedAccount.PriceGroupID = p.PriceGroupID;
                mappedAccount.FullAddress = p.FullAddress;
                mappedAccount.TaxPercent = p.GlobalTax.PercentageOfAmount;
                accounts.Add(mappedAccount);
            }

            result.Count = accounts.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, accounts.Count(), terminal.TerminalId, TerminalLogTypeEnum.AccountsSync).TerminalLogId;
            result.Accounts = accounts;
            return Ok(result);
        }


        public IHttpActionResult GetTenantPriceGroups(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new TenantPriceGroupsSyncCollection();

            var allGroups = _productPriceService.GetAllTenantPriceGroups(terminal.TenantId, true).Where(x => (x.DateUpdated ?? x.DateCreated) >= reqDate).ToList();

            var groups = new List<TenantPriceGroupsSync>();

            foreach (var p in allGroups)
            {
                var group = new TenantPriceGroupsSync();
                _mapper.Map(p, group);
                groups.Add(group);
            }

            result.Count = groups.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, groups.Count(), terminal.TerminalId, TerminalLogTypeEnum.TenantPriceGroupsSync).TerminalLogId;
            result.TenantPriceGroupsSync = groups;
            return Ok(result);
        }

        public IHttpActionResult GetTenantPriceGroupDetails(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new TenantPriceGroupDetailSyncCollection();

            var allGroupDetails = _productPriceService.GetAllTenantPriceGroupDetails(terminal.TenantId, true).Where(x => (x.DateUpdated ?? x.DateCreated) >= reqDate).ToList();

            var groupDetails = new List<TenantPriceGroupDetailSync>();

            foreach (var p in allGroupDetails)
            {
                var detail = new TenantPriceGroupDetailSync();
                _mapper.Map(p, detail);
                groupDetails.Add(detail);
            }

            result.Count = groupDetails.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, groupDetails.Count(), terminal.TerminalId, TerminalLogTypeEnum.TenantPriceGroupDetailsSync).TerminalLogId;
            result.TenantPriceGroupDetailSync = groupDetails;
            return Ok(result);
        }

    }
}