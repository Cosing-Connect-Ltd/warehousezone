using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ganedata.Core.Entities.Enums;

namespace WMS.Controllers.WebAPI
{
    public class ApiTerminalUserSyncController : BaseApiController
    {
        private readonly IActivityServices _activityServices;
        private readonly ITenantLocationServices _tenantLocationServices;
        private readonly IUserService _userService;

        public ApiTerminalUserSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IActivityServices activityServices)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _activityServices = activityServices;
            _tenantLocationServices = tenantLocationServices;
            _userService = userService;
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


            bool res = _userService.GetUserLoginStatus(loginStatus);

            if (res == true)
            {
                return Ok("Success");
            }
            else
            {
                return BadRequest("Failed");
            }
        }


    }
}