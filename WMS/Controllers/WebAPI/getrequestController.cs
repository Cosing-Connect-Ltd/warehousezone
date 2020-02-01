using Ganedata.Core.Services;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Ganedata.Core.Entities.Enums;

namespace WMS.Controllers.WebAPI
{
    public class getrequestController : BaseApiController
    {
        public getrequestController(ITerminalServices terminalServices,
            ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
        }

        /// <summary>
        /// This action is used by TnA device to get if there are any commands to run
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Get()
        {

            //getrequest LISTENS FOR ANY NEW COMMAND
            string req = Request.RequestUri.AbsoluteUri; //http://localhost/iclock/getrequest?sn=6647164900009

            var request = new Uri(req);
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound);
            string sn = HttpUtility.ParseQueryString(request.Query).Get("sn");
            string clientIp = HttpContext.Current.Request.UserHostAddress;
            string ServerIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            var returnResponse = "";

            Terminals terminal = TerminalServices.GetTerminalBySerial(sn);

            if (terminal == null)
            {
                resp = new HttpResponseMessage(HttpStatusCode.NotFound);
                return resp;
            }
            else
            {
                List<TerminalCommandsQueue> commandQueue = TerminalServices.GetTerminalCommandQueue(terminal.TerminalId, DateTime.UtcNow);

                if (commandQueue.Count() > 0)
                {
                    foreach (var item in commandQueue)
                    {
                        returnResponse += item.TerminalCommands.CommandString + "\r\n";

                        item.sent = true;
                        item.SentDate = DateTime.UtcNow;
                        item.DateUpdated = DateTime.UtcNow;
                        TerminalServices.SaveTerminalCommandQueueStatus(item);

                    }
                }
                //return server response

                //This following commented command works fine to restart the device
                //returnResponse = $"C:ID1:REBOOT\n";

                //returnResponse = $"\r\n";
                resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(returnResponse, System.Text.Encoding.UTF8, "text/plain");

                // Create log
                TerminalLogTypeEnum logType = (TerminalLogTypeEnum)Enum.Parse(typeof(TerminalLogTypeEnum), "TnAGetCommand");

                TerminalsLog newDeviceLog = new TerminalsLog();
                newDeviceLog.TerminalLogId = Guid.NewGuid();
                newDeviceLog.TerminalId = terminal.TerminalId;
                newDeviceLog.TerminalLogType = logType.ToString();
                newDeviceLog.Response = resp.StatusCode.ToString();
                newDeviceLog.ResponseText = returnResponse;
                newDeviceLog.DateCreated = DateTime.UtcNow;
                newDeviceLog.clientIp = clientIp;
                newDeviceLog.ServerIp = ServerIp;
                newDeviceLog.TenantId = terminal.TenantId;
                newDeviceLog.DateRequest = DateTime.UtcNow;

                TerminalServices.SaveTerminalLog(newDeviceLog, terminal.TenantId);

                return resp;
            }
        }
    }
}