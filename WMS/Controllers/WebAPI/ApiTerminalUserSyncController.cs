﻿using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ganedata.Core.Entities.Enums;
using System.Threading.Tasks;
using AutoMapper;
using System.Net;
using System.Web.Http.Cors;

namespace WMS.Controllers.WebAPI
{
    [EnableCorsAttribute("*", "*", "*")]
    public class ApiTerminalUserSyncController : BaseApiController
    {
        private readonly IActivityServices _activityServices;
        private readonly ITenantLocationServices _tenantLocationServices;
        private readonly IUserService _userService;
        private readonly IGaneConfigurationsHelper _configurationsHelper;
        private readonly IAccountServices _accountServices;
        private readonly IMapper _mapper;
        private readonly IShoppingVoucherService _shoppingVoucherService;

        public ApiTerminalUserSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, IActivityServices activityServices, IGaneConfigurationsHelper configurationsHelper, IAccountServices accountServices, IMapper mapper, IShoppingVoucherService shoppingVoucherService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _activityServices = activityServices;
            _tenantLocationServices = tenantLocationServices;
            _userService = userService;
            _configurationsHelper = configurationsHelper;
            _accountServices = accountServices;
            _mapper = mapper;
            _shoppingVoucherService = shoppingVoucherService;
        }

        //GET http://localhost:8005/api/sync/users/{reqDate}/{serialNo}
        //GET http://localhost:8005/api/sync/users/2014-11-23/920013c000814
        public IHttpActionResult GetUsers(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var users = UserService.GetAuthUsersByTenantAndDateUpdated(terminal.TenantId, reqDate);
            List<UserSync> newUsers = new List<UserSync>();

            foreach (var usr in users)
            {
                UserSync newUser = new UserSync();
                newUser.UserId = usr.UserId;

                var resourceId = UserService.GetResourceIdByUserId(usr.UserId);
                newUser.IsResource = resourceId > 0;
                newUser.ResourceId = resourceId;

                newUser.Username = usr.UserName;
                newUser.Password = usr.UserPassword;
                newUser.Name = usr.DisplayName;
                newUser.IsActive = usr.IsActive;

                newUser.IsDeleted = usr.IsDeleted;
                newUser.DateUpdated = usr.DateUpdated;
                newUser.PersonalReferralCode = usr.PersonalReferralCode;
                newUser.ReferralConfirmed = usr.ReferralConfirmed;
                //get parent warehouse to check permissions
                int warehouseId = terminal.WarehouseId;

                var location = _tenantLocationServices.GetActiveTenantLocationById(terminal.WarehouseId);

                if (location.IsMobile == true)
                {
                    warehouseId = location.ParentWarehouseId ?? warehouseId;
                }

                newUser.PurchaseOrderPerm = _activityServices.PermCheck("Handheld", "PurchaseOrderPerm", usr.UserId, warehouseId);
                newUser.SalesOrderPerm = _activityServices.PermCheck("Handheld", "SalesOrderPerm", usr.UserId, warehouseId);
                newUser.TransferOrderPerm = _activityServices.PermCheck("Handheld", "TransferOrderPerm", usr.UserId, warehouseId);
                newUser.GoodsReturnPerm = _activityServices.PermCheck("Handheld", "GoodsReturnPerm", usr.UserId, warehouseId);
                newUser.StockTakePerm = _activityServices.PermCheck("Handheld", "StockTakePerm", usr.UserId, warehouseId);
                newUser.PalletingPerm = _activityServices.PermCheck("Handheld", "PalletingPerm", usr.UserId, warehouseId);
                newUser.WorksOrderPerm = _activityServices.PermCheck("Handheld", "WorksOrderPerm", usr.UserId, warehouseId);
                newUser.MarketRoutesPerm = _activityServices.PermCheck("Handheld", "MarketRoutesPerm", usr.UserId, warehouseId);
                newUser.RandomJobsPerm = _activityServices.PermCheck("Handheld", "RandomJobsPerm", usr.UserId, warehouseId);
                newUser.PODPerm = _activityServices.PermCheck("Handheld", "PODPerm", usr.UserId, warehouseId);
                newUser.StockEnquiryPerm = _activityServices.PermCheck("Handheld", "StockEnquiryPerm", usr.UserId, warehouseId);
                newUser.EndOfDayPerm = _activityServices.PermCheck("Handheld", "EndOfDayPerm", usr.UserId, warehouseId);
                newUser.HolidaysPerm = _activityServices.PermCheck("Handheld", "HolidaysPerm", usr.UserId, warehouseId);
                newUser.AddProductsOnScan = _activityServices.PermCheck("Handheld", "AddProductsOnScan", usr.UserId, warehouseId);
                newUser.DirectSalesPerm = _activityServices.PermCheck("Handheld", "DirectSalesPerm", usr.UserId, warehouseId);
                newUser.WastagesPerm = _activityServices.PermCheck("Handheld", "WastagesPerm", usr.UserId, warehouseId);
                newUser.GeneratePalletLabelsPerm = _activityServices.PermCheck("Handheld", "GeneratePalletLabelsPerm", usr.UserId, warehouseId);
                newUser.GoodsReceiveCountPerm = _activityServices.PermCheck("Handheld", "GoodsReceiveCountPerm", usr.UserId, warehouseId);
                newUser.HandheldOverridePerm = _activityServices.PermCheck("Handheld", "HandheldOverridePerm", usr.UserId, warehouseId);
                newUser.ExchangeOrdersPerm = _activityServices.PermCheck("Handheld", "ExchangeOrdersPerm", usr.UserId, warehouseId);
                newUser.AllowModifyPriceInTerminal = _activityServices.PermCheck("Handheld", "AllowModifyPriceInTerminal", usr.UserId, warehouseId);
                newUser.PrintBarcodePerm = _activityServices.PermCheck("Handheld", "PrintBarcodePerm", usr.UserId, warehouseId);
                newUser.PendingOrdersPerm = _activityServices.PermCheck("Handheld", "PendingOrdersPerm", usr.UserId, warehouseId);

                newUsers.Add(newUser);
            }

            int count = users.Count();

            try
            {

                UsersSyncCollection collection = new UsersSyncCollection
                {
                    Count = count,
                    TerminalLogId = TerminalServices
                        .CreateTerminalLog(reqDate, terminal.TenantId, count, terminal.TerminalId,
                            TerminalLogTypeEnum.UsersSync).TerminalLogId,
                    Users = newUsers
                };

                return Ok(collection);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception while getting user sync collection - " + ex.Message.ToString(), ex.InnerException);
            }
        }

        //GET http://localhost:8005/api/sync/verify-acks/{id}/{count}/{serialNo}
        //GET http://ganetest.qsrtime.net/api/sync/verify-acks/8cbb3504-5824-47dd-8b23-e49ee6dd19f1/47/920013c000814
        [HttpGet]
        public IHttpActionResult VerifyAcknowlegement(Guid id, int count, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            TerminalsLog log = TerminalServices.GetTerminalLogByLogId(id);

            if (log == null)
            {
                return BadRequest();
            }

            log.Ack = true;
            log.RecievedCount = count;

            TerminalServices.UpdateTerminalLog(log);

            if (log.SentCount != count)
            {
                return BadRequest();
            }

            return Ok("Success");
        }

        public IHttpActionResult GetConnectionCheck(string serialNo)
        {

            if (serialNo == null) return Unauthorized();

            serialNo = serialNo.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            return Ok();
        }


        public IHttpActionResult GetTerminalGeoLocations(string serialNo)
        {

            if (serialNo == null) return Unauthorized();

            serialNo = serialNo.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var locations = TerminalServices.GetTerminalGeoLocations(terminal.TerminalId);

            return Ok(locations);
        }


        [HttpPost]
        public IHttpActionResult PostTerminalGeoLocation(TerminalGeoLocationViewModel geoLocation)
        {
            string serialNo = geoLocation.SerialNo.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }
            else
            {
                geoLocation.TerminalId = terminal.TerminalId;
                geoLocation.TenantId = terminal.TenantId;
            }

            int res = TerminalServices.SaveTerminalGeoLocation(geoLocation);

            if (res > 0)
            {
                return Ok("Success");
            }
            else
            {
                return BadRequest("Unable to save records");
            }
        }


        [HttpPost]
        public IHttpActionResult GetUserLoginStatus(UserLoginStatusViewModel loginStatus)
        {
            string serialNo = loginStatus?.SerialNo?.Trim().ToLower();
            loginStatus.Md5Pass = GaneStaticAppExtensions.GetMd5(loginStatus.Md5Pass);

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var resp = _userService.GetUserLoginStatus(loginStatus);

            if (resp.Success == true)
            {
                return Ok(resp);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public IHttpActionResult GetUserLoginStatusNew(UserLoginStatusViewModel loginStatus)
        {

            loginStatus.Md5Pass = GaneStaticAppExtensions.GetMd5(loginStatus.Md5Pass);
            UserLoginStatusResponseViewModel resp = new UserLoginStatusResponseViewModel();
            resp = _userService.GetUserLoginStatus(loginStatus);

            if (resp.Success == true)
            {
                return Ok(resp);
            }
            else
            {
                resp.ErrorMessage = "User Name or Password is Incorrect";
                return Ok(resp);
            }
        }

        [HttpPost]
        public IHttpActionResult GetWebUserLoginStatus(UserLoginStatusViewModel loginStatus)
        {
            string serialNo = loginStatus?.SerialNo?.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var resp = _userService.GetUserLoginStatus(loginStatus, true);

            if (resp.Success == true)
            {
                return Ok(resp);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostWebUserRegister(UserRegisterRequestViewModel user)
        {
            string serialNo = user?.SerialNo?.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var existingUser = _userService.GetAuthUserByUserName(user.UserName, user.TenantId);

                if (existingUser != null)
                {
                    return BadRequest("Username already exist, please choose another one");
                }

                existingUser = _userService.GetAuthUserByUserInfo(user.UserEmail, user.UserMobileNumber,  user.TenantId);

                if (existingUser != null)
                {
                    return BadRequest("Phone Number or Email already registered, please choose another one");
                }

                AuthUser authUser = new AuthUser();
                Account account = new Account();
                account.AccountCode = user.UserFirstName + GaneStaticAppExtensions.GenerateRandomNo();
                account.CompanyName = account.AccountCode;
                account.RegNo = "";
                account.VATNo = "";
                account.AccountStatusID = AccountStatusEnum.Active;
                var accountModel = _accountServices.SaveAccount(account, null, null, 1, 1, 1, null, 1, null, null, 0, user.TenantId, null);

                authUser.UserEmail = user.UserEmail;
                authUser.UserFirstName = user.UserFirstName;
                authUser.UserLastName = user.UserLastName;
                authUser.UserPassword = user.Md5Pass;
                authUser.UserName = user.UserName;
                authUser.UserMobileNumber = user.UserMobileNumber;

                authUser.IsActive = true;
                authUser.WebUser = true;
                authUser.VerificationRequired = true;
                authUser.AccountId = accountModel.AccountID;
                int res = _userService.SaveAuthUser(authUser, 0, user.TenantId);
                if (res > 0)
                {
                    _shoppingVoucherService.LoadDefaultSystemVouchersForNewUser(authUser.UserId);
                    await _userService.CreateUserVerificationCode(authUser.UserId, terminal.TenantId, UserVerifyTypes.Mobile);
                    UserLoginStatusResponseViewModel resp = new UserLoginStatusResponseViewModel();
                    _mapper.Map(authUser, resp);
                    resp.Success = true;
                    return Ok(resp);
                }
                else
                {
                    return BadRequest("Unable to create user");
                }
            }
            else
            {
                UserLoginStatusResponseViewModel resp = new UserLoginStatusResponseViewModel();
                return BadRequest("Required fields not present");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateUserVerification(UserVerifyRequestViewModel request)
        {
            string serialNo = request?.SerialNo?.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var resp = await _userService.CreateUserVerificationCode(request.UserId, terminal.TenantId, request.Type);

            if (resp != null)
            {
                return Ok(true);
            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPost]
        public IHttpActionResult VerifyUser(UserVerifyRequestViewModel request)
        {
            string serialNo = request?.SerialNo?.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var res = _userService.VerifyUserVerificationCode(request.UserId, terminal.TenantId, request.Code, request.Type);

            if (res == true)
            {
                UserLoginStatusResponseViewModel resp = new UserLoginStatusResponseViewModel();
                var authUser = _userService.GetAuthUserById(request.UserId);
                authUser.MobileNumberVerified = true;
                authUser.VerificationRequired = false;
                authUser.UserPassword = null;
                _userService.UpdateAuthUser(authUser, 0, terminal.TenantId);
                _mapper.Map(authUser, resp);
                resp.Success = true;
                return Ok(resp);
            }
            else
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }
        }


        public async Task<IHttpActionResult> PostPasswordResetRequest(UserPasswordResetRequestModel model)
        {
            string serialNo = model?.SerialNo?.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var response  = await _userService.CreateUserVerificationCode(model.EmailAddress, terminal.TenantId, UserVerifyTypes.Mobile);
             
            return Ok(new UserVerifyResponseModel(){  UserId = response.UserId, Code = response .Code, EmailAddress = response .EmailAddress});
        }

        public IHttpActionResult PostUserUpdateRequest(UserSyncUpdateViewModel model)
        {
            string serialNo = model?.TerminalSerialNumber?.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var user = _userService.GetAuthUserById(model.UserId);

            user.UserFirstName = model.FirstName;
            user.UserLastName = model.Surname;
            user.UserMobileNumber = model.MobileNumber;
            user.UserPassword = model.PasswordMd5;
            user.IsActive = true;
            _userService.UpdateAuthUser(user, model.UserId, terminal.TenantId, true);

            var result = GetUserInfo(model.UserId, serialNo);

            return Ok(result);
        }

        private UserSyncUpdateViewModel GetUserInfo(int userId, string serialNumber)
        {
            var user = _userService.GetAuthUserById(userId);

            var result = new UserSyncUpdateViewModel()
            {
                FirstName = user.UserFirstName,
                Surname = user.UserLastName,
                EmailAddress = user.UserEmail,
                MobileNumber = user.UserMobileNumber,
                PasswordMd5 = user.UserPassword,
                TerminalSerialNumber = serialNumber,
                UserId = userId,
                Username = user.UserName
            };

            return result;
        }

        public IHttpActionResult UserDetails(int id, string serialNumber)
        {
            var result = GetUserInfo(id, serialNumber);
            return Ok(result);
        }
    }
}