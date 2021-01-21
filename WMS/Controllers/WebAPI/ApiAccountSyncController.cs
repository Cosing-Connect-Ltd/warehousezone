using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using AutoMapper;
using Ganedata.Core.Entities.Domain;

namespace WMS.Controllers.WebAPI
{
    public class ApiAccountSyncController : BaseApiController
    {
        private readonly IAccountServices _accountServices;
        private readonly IProductPriceService _productPriceService;
        private readonly IMapper _mapper;
        private readonly IEmailServices _emailServices;

        public ApiAccountSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, IAccountServices accountServices, IProductPriceService productPriceService, IMapper mapper, IEmailServices emailServices)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _accountServices = accountServices;
            _productPriceService = productPriceService;
            _mapper = mapper;
            _emailServices = emailServices;
        }
        // GET http://localhost:8005/api/sync/accounts/{reqDate}/{serialNo}
        // GET http://localhost:8005/api/sync/accounts/2014-11-23/920013c000814
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
                mappedAccount.PointsToNextReward = p.AccountLoyaltyPoints<500? 500 - p.AccountLoyaltyPoints: 0; //This will need to be changed based on Najum's input
                mappedAccount.RecentRewardPoints = p.AccountRewardPoints.Select(m => new RecentRewardPointSync()
                {
                    OrderDateTime = m.OrderDateTime,
                    PointsEarned = m.PointsEarned,
                    OrderID = m.OrderID
                }).ToList();
                accounts.Add(mappedAccount);
            }

            result.Count = accounts.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, accounts.Count(), terminal.TerminalId, TerminalLogTypeEnum.AccountsSync).TerminalLogId;
            result.Accounts = accounts;
            return Ok(result);
        }

        // GET http://localhost:8005/api/sync/account/{accountId}/{serialNo}
        // GET http://localhost:8005/api/sync/account/2/920013c000814
        public IHttpActionResult GetAccount(int accountId, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var acc = _accountServices.GetAccountsById(accountId);

            if (acc != null)
            {
                var result = new UserAccountSyncCollection();
                var account = new AccountSync();
                var mappedAccount = _mapper.Map(acc, account);
                mappedAccount.CountryName = acc.GlobalCountry.CountryName;
                mappedAccount.CurrencyName = acc.GlobalCurrency.CurrencyName;
                mappedAccount.PriceGroupName = acc.TenantPriceGroups.Name;
                mappedAccount.PriceGroupID = acc.PriceGroupID;
                mappedAccount.FullAddress = acc.FullAddress;
                mappedAccount.TaxPercent = acc.GlobalTax.PercentageOfAmount;
                result.Account = mappedAccount;
                mappedAccount.PointsToNextReward = acc.AccountLoyaltyPoints < 500 ? 500 - acc.AccountLoyaltyPoints : 0; //This will need to be changed based on Najum's input
                mappedAccount.RecentRewardPoints = acc.AccountRewardPoints.Select(m => new RecentRewardPointSync()
                {
                    OrderDateTime = m.OrderDateTime,
                    PointsEarned = m.PointsEarned,
                    OrderID = m.OrderID
                }).ToList();
                result.TerminalLogId = TerminalServices.CreateTerminalLog(DateTime.UtcNow, terminal.TenantId, 1, terminal.TerminalId, TerminalLogTypeEnum.UserAccountsSync).TerminalLogId;
                return Ok(result);

            }
            else
            {
                return NotFound();
            }

        }


        public IHttpActionResult GetAccountAddresses(DateTime reqDate, string serialNo, int accountId)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var acc = _accountServices.GetAccountsById(accountId);

            if (acc != null)
            {

                var allAddresses = _accountServices.GetAllValidAccountAddressesByAccountId(accountId, reqDate, true);
                var result = new AccountAddressesSyncCollection();
                var addresses = new List<AccountAddressSync>();

                foreach (var item in allAddresses)
                {
                    var address = new AccountAddressSync();
                    var mappedAddress = _mapper.Map(item, address);
                    addresses.Add(mappedAddress);
                }

                result.Count = allAddresses.Count();
                result.Addresses = addresses;
                result.TerminalLogId = TerminalServices.CreateTerminalLog(DateTime.UtcNow, terminal.TenantId, 1, terminal.TerminalId, TerminalLogTypeEnum.AccountAddressesSync).TerminalLogId;
                return Ok(result);

            }
            else
            {
                return NotFound();
            }
        }

       

        [HttpPost]
        public IHttpActionResult PostAccountAddress(AccountAddressSync address)
        {
            string serialNo = address.SerialNo.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            AccountAddresses newAddress = new AccountAddresses();
            _mapper.Map(address, newAddress);

            var res = _accountServices.SaveAccountAddress(newAddress, 0);

            if (res.AddressID > 0)
            {
                var savedAddress = new AccountAddressSync();
                _mapper.Map(res, savedAddress);
                return Ok(savedAddress);
            }
            else
            {
                return BadRequest("Unable to save records");
            }
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
        [HttpPost]
        public IHttpActionResult AccountResetPassword(AccountPasswordResetSync model)
        {
            var (token, expiryDate) = _accountServices.PasswordResetCode(model.EmailAddress);
            if (token == null)
            {
                var response = model.EmailAddress + " is not a valid registered email address";
                var result = new {SentResetLinkSuccessfully= false, FailureMessage= response, EmailRegistered= false};
                return Ok(result);//Other type of responses will require change in response handling 
            }
            else
            {
                var lnkHref = "<a href='" + Url.Link("AccountsSyncResetPasswordGet", new { Controller= "Account", Action= "ResetPassword", email = model.EmailAddress, code = token }) + "'>Reset Password</a>";
                string subject = "Your changed password - " + Request.RequestUri.Host.ToUpper();
                string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;
                body += "<br>Link will expire on " + expiryDate.Value.ToString("dd/MM/yyyy HH:mm");
                var emailconfig = _emailServices.GetFirstActiveTenantEmailConfiguration();
                body = "<div style='text-align:center;padding: 40px;background-color: #ebf7f7;'>" + body + "</div>";
                var emailSender = new EmailSender(emailconfig, body, subject, model.EmailAddress);
                emailSender.SendMail();

                var result = new { SentResetLinkSuccessfully = true, FailureMessage = "", EmailRegistered = true };
                return Ok(result);

            }


        }

    }
}