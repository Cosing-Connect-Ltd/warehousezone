using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WMS.Controllers.WebAPI
{
    public class devicecmdController : BaseApiController
    {
        public devicecmdController(ITerminalServices terminalServices,
            ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
        }

        /// <summary>
        /// This method is used by TnA device to send response of a received command
        /// weather command was ok or not
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post()
        {
            string req = Request.RequestUri.AbsoluteUri; //http://localhost/iclock/devicecmd?sn=6647164900009&Return=0&ID=ID120&CMD=DATA
            var request = new Uri(req);
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound);
            var returnResponse = "";
            string clientIp = HttpContext.Current.Request.UserHostAddress;
            string ServerIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            string sn = HttpUtility.ParseQueryString(request.Query).Get("sn");

            Terminals terminal = TerminalServices.GetTerminalBySerial(sn);

            if (terminal == null)
            {
                resp = new HttpResponseMessage(HttpStatusCode.NotFound);
                return resp;
            }
            else
            {
                string body = await Request.Content.ReadAsStringAsync();
                var postData = HttpUtility.ParseQueryString(request.Query);
                resp = new HttpResponseMessage(HttpStatusCode.OK);
                returnResponse = "OK";
                resp.Content = new StringContent(returnResponse, System.Text.Encoding.UTF8, "text/plain");

                List<TerminalCommandsQueue> commandQueue = TerminalServices.GetTerminalCommandQueue(terminal.TerminalId, DateTime.UtcNow, true, false);

                foreach (var item in commandQueue)
                {
                    body = body.Replace("\\n", "\n"); //do replace for Debugging purposes, b/c fiddler converts single slash into double slashes

                    char[] delimiters = { '\n' };
                    string[] fieldValue = { };

                    fieldValue = body.Split(delimiters);

                    foreach (var res in fieldValue)
                    {
                        char[] innerDelimiters = { '&' };
                        var commandValue = res.Split(innerDelimiters);

                        if (commandValue[0].Contains(item.TerminalCommands.CommandIdentifier))
                        {
                            item.result = true;
                            item.ResultDate = DateTime.UtcNow;
                            item.resultString = res;
                            item.DateUpdated = DateTime.UtcNow;
                            TerminalServices.SaveTerminalCommandQueueStatus(item);
                        }
                    }
                }

                // Create log
                TerminalLogTypeEnum logType = (TerminalLogTypeEnum)Enum.Parse(typeof(TerminalLogTypeEnum), "TnAPostCmdResponse");

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