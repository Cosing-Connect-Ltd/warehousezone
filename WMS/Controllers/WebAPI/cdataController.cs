using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using Ganedata.Core.Entities.Enums;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WMS.Controllers.WebAPI
{
    public class cdataController : BaseApiController
    {
        //Note: When debugging: Run VS in Administrator mode for API to work in capturing iFace remote request
        //      see "applicationhost.config" file, <binding protocol="http" bindingInformation="*:8006:*" />        
        readonly IAttLogsServices _attLogsServices;
        readonly IEmployeeShiftsServices _employeeShiftsServices;
        readonly IAttLogsStampsServices _attLogsStampsServices;
        readonly IOperLogsServices _operLogs;
        readonly IEmployeeServices _employeeServices;
        readonly IEmployeeShiftsStoresServices _shiftStoreServices;

        public cdataController(IAttLogsServices attLogsServices, IEmployeeShiftsServices employeeShiftsServices,
            IAttLogsStampsServices attLogsStampsServices,
            IOperLogsServices operLogs, IEmployeeServices employeeServices, IEmployeeShiftsStoresServices shiftsStoresServices, ITerminalServices terminalServices,
            ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IAccountServices accountServices, IProductPriceService productPriceService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _attLogsServices = attLogsServices;
            _employeeShiftsServices = employeeShiftsServices;
            _attLogsStampsServices = attLogsStampsServices;
            _operLogs = operLogs;
            _employeeServices = employeeServices;
            _shiftStoreServices = shiftsStoresServices;
        }

        /// <summary>
        /// This is used by TnA device to get initial settings when device reboots
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Get()
        {
            string req = Request.RequestUri.AbsoluteUri; //http://localhost/iclock/cdata?sn=xxxxxx&options=all&pushver=2.0.1&language=XX

            var request = new Uri(req);
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound);
            string sn = HttpUtility.ParseQueryString(request.Query).Get("sn");
            string clientIp = HttpContext.Current.Request.UserHostAddress;
            string ServerIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            var returnResponse = "";
            // time zone zero means GMT (UK time), central europe = 1, China = 8 and Canada = -8
            string TimeZoneId = "0";


            Terminals terminal = TerminalServices.GetTerminalBySerial(sn);


            if (terminal == null)
            {
                resp = new HttpResponseMessage(HttpStatusCode.NotFound);
                returnResponse = $"Not Found: {sn}";
                return resp;
            }
            else
            {
                //get latest AttLogsStamp
                int lastAttLogsStamp = (_attLogsStampsServices.GetAllAttLogsStamps(terminal.TerminalId, TnALogsStampType.AttendanceLogStamp).Count() >= 1 ?
                    _attLogsStampsServices.GetAllAttLogsStamps(terminal.TerminalId, TnALogsStampType.AttendanceLogStamp).OrderByDescending(x => x.Id).FirstOrDefault().SStamp : 0);

                //get latest OperLogsStamp
                int lastOperLogsStamp = (_attLogsStampsServices.GetAllAttLogsStamps(terminal.TerminalId, TnALogsStampType.OperatorLogStamp).Count() >= 1 ?
                    _attLogsStampsServices.GetAllAttLogsStamps(terminal.TerminalId, TnALogsStampType.OperatorLogStamp).OrderByDescending(x => x.Id).FirstOrDefault().SStamp : 0);

                //return server response 
                var transFlag = "1100000000"; //10000000 = attlog only; 11000000 = attlog and operlog

                returnResponse = $"GET OPTION FROM:{sn}\r\nErrorDelay=60\r\nDelay=120\r\nTransInterval=0\r\nTransFlag={transFlag}\r\nRealtime=1\r\nEncrypt=0\r\nAttStamp={lastAttLogsStamp}\r\nOpStamp={lastOperLogsStamp}\r\nTimeZone={TimeZoneId}\r\n";

                // send OK response with content
                resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(returnResponse, Encoding.UTF8, "text/plain");

                // Create log
                TerminalLogTypeEnum logType = (TerminalLogTypeEnum)Enum.Parse(typeof(TerminalLogTypeEnum), "TnAGetConfig");

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

        /// <summary>
        /// TnA device will send all Time logs and operator logs to this method
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post()
        {
            string req = Request.RequestUri.AbsoluteUri; //http://localhost/iclock/cdata?sn=xxxxxx&table=ATTLOG&Stamp=12345678
            string body = await Request.Content.ReadAsStringAsync();
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound);
            var returnResponse = "";
            string clientIp = HttpContext.Current.Request.UserHostAddress;
            string ServerIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            var request = new Uri(req);

            string table = HttpUtility.ParseQueryString(request.Query).Get("table");
            string sn = HttpUtility.ParseQueryString(request.Query).Get("sn");
            string stamp = "";

            Terminals terminal = TerminalServices.GetTerminalBySerial(sn);

            if (terminal == null)
            {
                resp = new HttpResponseMessage(HttpStatusCode.NotFound);
                return resp;
            }
            else
            {
                switch (table.ToLower())
                {
                    case "attlog":
                        stamp = HttpUtility.ParseQueryString(request.Query).Get("stamp");

                        if (!String.IsNullOrWhiteSpace(body))
                        {
                            //-----possible values (single or multiple packets sent from terminal)-----
                            //body = "1\t2017-03-02 10:48:08\t0\t15\t0\t1\n";
                            //body = "2\t2017-04-02 08:48:08\t0\t15\t0\t1\n2\t2017-04-02 12:16:08\t0\t15\t0\t1\n2\t2017-04-02 13:03:08\t0\t15\t0\t1\n2\t2017-04-02 17:00:08\t0\t15\t0\t1\n"
                            //body = "1\t2017-03-03 00:07:29\t0\t15\t0\t2\n1\t2017-03-03 00:07:31\t0\t15\t0\t2\n1\t2017-03-03 00:07:33\t0\t15\t0\t2\n1\t2017-03-02 17:09:47\t0\t15\t0\t2\n1\t2017-03-02 17:09:51\t0\t15\t0\t2\n1\t2017-03-02 17:09:53\t0\t15\t0\t2\n1\t2017-03-02 17:13:42\t0\t15\t0\t2\n1\t2017-03-02 17:13:44\t0\t15\t0\t2\n1\t2017-03-02 17:13:46\t0\t15\t0\t2\n1\t2017-03-02 17:13:48\t0\t15\t0\t2\n1\t2017-03-02 17:13:58\t0\t15\t0\t2\n1\t2017-03-02 17:14:00\t0\t15\t0\t2\n1\t2017-03-02 17:14:02\t0\t15\t0\t2\n1\t2017-03-02 17:14:11\t0\t15\t0\t2\n1\t2017-03-02 17:14:25\t0\t15\t0\t2\n1\t2017-03-02 17:14:27\t0\t15\t0\t2\n1\t2017-03-02 17:19:53\t0\t4\t0\t2\n1\t2017-03-02 17:19:56\t0\t4\t0\t2\n1\t2017-03-02 17:19:59\t0\t4\t0\t2\n1\t2017-03-02 17:21:09\t0\t15\t0\t2\n"
                            //-------------------------

                            body = body.Replace("\\t", "\t").Replace("\\n", "\n"); //do replace for Debugging purposes, b/c fiddler converts single slash into double slashes

                            char[] delimiters = { '\n' };
                            string[] fieldValue = { };

                            fieldValue = body.Split(delimiters);

                            //loop through each timestamp
                            foreach (string item in fieldValue)
                            {
                                if (!String.IsNullOrEmpty(item))
                                {
                                    char[] itemDelimiters = { '\t' };
                                    string[] itemValue = item.Split(itemDelimiters);
                                    var userPIN = itemValue[0];
                                    DateTime dateTimeStamp = Convert.ToDateTime(itemValue[1]);
                                    var ifaceStatus = Convert.ToInt32(itemValue[2]); //Clock In, see API Docs Table4
                                    var verify = Convert.ToInt32(itemValue[3]); //FingerPrint/Face, see API Docs Table4
                                    var eventCode = "";

                                    if (itemValue.Count() > 4)
                                    {
                                        eventCode = itemValue[4];
                                    }


                                    //insert to db for audit purposes
                                    int attLogsId = _attLogsServices.Insert(new AttLogs()
                                    {
                                        UserPIN = userPIN,
                                        Time = dateTimeStamp,
                                        Status = ifaceStatus,
                                        Verify = verify,
                                        EventCode = eventCode,
                                        DeviceSerialNo = sn
                                    });

                                    //--------insert to EmployeeShifts table-----------
                                    CultureInfo cInfo = CultureInfo.CurrentCulture;
                                    int weekNumber = cInfo.Calendar.GetWeekOfYear(dateTimeStamp.Date, cInfo.DateTimeFormat.CalendarWeekRule, cInfo.DateTimeFormat.FirstDayOfWeek);

                                    //check if employeeId exist
                                    int employeeId = Convert.ToInt32(userPIN);
                                    var employeeInfo = _employeeServices.GetByEmployeeId(Convert.ToInt32(employeeId));

                                    var dupStamp = _employeeShiftsServices.SearchDuplicateStampByDateAndEmployee(Convert.ToInt32(userPIN), dateTimeStamp);

                                    //go to next record if same stamp already exist
                                    //add it otherwise
                                    if (dupStamp != null && dupStamp.TimeStamp == dateTimeStamp)
                                    {
                                        continue;
                                    }

                                    //count employeeId for specific date
                                    var lastStamp = _employeeShiftsServices.SearchLastStampByDateAndEmployee(Convert.ToInt32(userPIN), dateTimeStamp);
                                    int statusId = 4; //4 = Unknown, Note: see Enum "EnumStatusType" for Status reference (of statusId)


                                    if (lastStamp == null || lastStamp.StatusType == "Out")
                                    {
                                        statusId = 5;
                                    }
                                    else
                                    {
                                        statusId = 6;
                                    }

                                    if (employeeInfo == null)
                                        statusId = 4; //4 = unknown

                                    //get shift status
                                    EnumAttStatusType statusType = (EnumAttStatusType)Enum.Parse(typeof(EnumAttStatusType), statusId.ToString());

                                    if (employeeInfo != null)
                                    {
                                        int employeeShiftsId = _employeeShiftsServices.Insert(new ResourceShifts()
                                        {
                                            EmployeeShiftID = userPIN,
                                            ResourceId = employeeInfo?.ResourceId,
                                            Date = dateTimeStamp.Date,
                                            WeekNumber = weekNumber,
                                            TimeStamp = dateTimeStamp,
                                            StatusType = statusType.ToString(),
                                            TerminalId = terminal.TerminalId,
                                            DateCreated = DateTime.UtcNow,
                                            TenantId = terminal.TenantId
                                        });

                                        // insert into EmployeeSHifts_Stores

                                        if (!employeeInfo.EmployeeShifts_Stores.Any(x => x.WarehouseId == terminal.WarehouseId))
                                        {
                                            EmployeeShifts_Stores shift = new EmployeeShifts_Stores();
                                            shift.ResourceId = Convert.ToInt32(userPIN);
                                            shift.WarehouseId = terminal.WarehouseId;
                                            _shiftStoreServices.Insert(shift);
                                        }
                                    }
                                }
                            }

                            //---------insert to AttLogsStamps table-------
                            _attLogsStampsServices.Insert(new AttLogsStamps()
                            {
                                SStamp = Convert.ToInt32(stamp),
                                TerminalId = terminal.TerminalId,
                                TnALogsStampType = TnALogsStampType.AttendanceLogStamp
                            });
                        }

                        //return response as OK status with message "OK"
                        resp = new HttpResponseMessage(HttpStatusCode.OK);
                        returnResponse = "OK";
                        resp.Content = new StringContent(returnResponse, Encoding.UTF8, "text/plain");
                        break;

                    case "operlog":
                        stamp = HttpUtility.ParseQueryString(request.Query).Get("opstamp");

                        if (!String.IsNullOrWhiteSpace(body))
                        {
                            if (body.Contains("OPLOG"))
                            {
                                body = body.Replace("\\t", "\t").Replace("\\n", "\n"); //do replace for Debugging purposes, b/c fiddler converts single slash into double slashes

                                string[] fieldValue = { };
                                fieldValue = body.Split(new string[] { "OPLOG" }, StringSplitOptions.None);

                                //loop through each operation log
                                foreach (string item in fieldValue)
                                {
                                    if (!String.IsNullOrEmpty(item))
                                    {
                                        char[] itemDelimiters = { '\t' };
                                        string[] itemValue = item.Replace("\n", String.Empty).Split(itemDelimiters); //removing line ending \n then split
                                        var operType = itemValue[0]; //operation type
                                        var adminId = Convert.ToInt32(itemValue[1]); //admin id
                                        DateTime dateTimeStamp = Convert.ToDateTime(itemValue[2]); //operation time
                                                                                                   //DateTime operTime = dateTimeStamp.ToUniversalTime(); //converts to UTC
                                        int object1 = 0;
                                        int.TryParse(itemValue[3], out object1);

                                        int object2 = 0;
                                        int.TryParse(itemValue[4], out object2);

                                        int object3 = 0;
                                        int.TryParse(itemValue[5], out object3);

                                        int object4 = 0;
                                        int.TryParse(itemValue[6], out object4);

                                        //insert to db for audit purposes
                                        _operLogs.Insert(new OperLogs()
                                        {
                                            OperationType = operType,
                                            AdminID = Convert.ToString(adminId),
                                            OperationTime = dateTimeStamp,
                                            OperationObject1 = object1,
                                            OperationObject2 = object2,
                                            OperationObject3 = object3,
                                            OperationObject4 = object4
                                        });
                                    }
                                }
                            }

                            //---------insert to AttLogsStamps table-------
                            _attLogsStampsServices.Insert(new AttLogsStamps()
                            {
                                SStamp = Convert.ToInt32(stamp),
                                TerminalId = terminal.TerminalId,
                                TnALogsStampType = TnALogsStampType.OperatorLogStamp
                            });
                        }

                        //return response as OK status with message "OK"
                        resp = new HttpResponseMessage(HttpStatusCode.OK);
                        returnResponse = "OK";
                        resp.Content = new StringContent(returnResponse, Encoding.UTF8, "text/plain");
                        break;

                    default:

                        resp = new HttpResponseMessage(HttpStatusCode.OK);
                        returnResponse = "OK";
                        resp.Content = new StringContent(returnResponse, Encoding.UTF8, "text/plain");
                        break;
                }

                // Create log
                TerminalLogTypeEnum logType = (TerminalLogTypeEnum)Enum.Parse(typeof(TerminalLogTypeEnum), "TnAPostStampsAndLogs");

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

                //return server response 
                resp.Content = new StringContent(returnResponse, Encoding.UTF8, "text/plain");
                return resp;
            }
        }
    }
}