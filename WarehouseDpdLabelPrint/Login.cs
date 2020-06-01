using EzioDll;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WarehouseDpdLabelPrint
{
    public partial class frmLogin : Form
    {
        GodexPrinter Printer = new GodexPrinter();
        bool loginStatus = false;
        int loginUserId = 0;
        string userName = "";
        Timer PrintTimer = new Timer();

        public frmLogin()
        {
            InitializeComponent();
        }

        private void toggleVisibility(bool status)
        {
            bool reverseStatus = status == true ? false : true;
            label1.Visible = status;
            label2.Visible = status;
            textBox1.Visible = status;
            textBox2.Visible = status;
            button1.Visible = status;

            label3.Visible = reverseStatus;
            button2.Visible = reverseStatus;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int tenantId = Convert.ToInt32(ConfigurationManager.AppSettings["TenantId"]);
            string siteUrl = ConfigurationManager.AppSettings["SiteUrl"];
            // Get shipments id from warehouse system

            UserLoginStatusViewModel userModel = new UserLoginStatusViewModel();
            userModel.UserName = textBox1.Text;
            userModel.Md5Pass = GetMd5(textBox2.Text);
            userModel.TenantId = tenantId;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(siteUrl + "api/sync/get-login-status");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(userModel);
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var resp = JsonConvert.DeserializeObject<UserLoginStatusResponseViewModel>(result);
                        loginStatus = true;
                        userName = resp.UserName;
                        loginUserId = resp.UserId;
                    }

                    toggleVisibility(false);
                    label3.Text = "Welcome " + userName;
                    label3.ForeColor = Color.Green;
                    this.WindowState = FormWindowState.Minimized;

                    PrintTimer.Interval = (10 * 1000); // 10 seconds
                    PrintTimer.Tick += new EventHandler(PrintTimer_Tick);
                    PrintTimer.Start();
                }

                else
                {
                    label3.Text = "failed";
                    label3.Visible = true;
                    label3.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                label3.Text = "failed: " + ex.Message;
                label3.Visible = true;
                label3.ForeColor = Color.Red;
            }
        }

        private void PrintTimer_Tick(object sender, EventArgs e)
        {
            CheckLabelsToPrint();
        }

        private void CheckLabelsToPrint()
        {
            int tenantId = Convert.ToInt32(ConfigurationManager.AppSettings["TenantId"]);
            string siteUrl = ConfigurationManager.AppSettings["SiteUrl"];
            PalletDispatchLabelPrintViewModel labelPrintData = new PalletDispatchLabelPrintViewModel();

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(siteUrl + $"api/sync/get-dispatch-for-label?tenantId={tenantId}&userId={loginUserId}");
                request.Method = "GET";
                request.ContentType = "application/json";
                var response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    labelPrintData = JsonConvert.DeserializeObject<PalletDispatchLabelPrintViewModel>(result);
                }
            }
            catch (Exception)
            {
                return;
            }

            if (labelPrintData.ShipmentId != null && labelPrintData.GeoAccount != null && labelPrintData.ApiUrl != null && labelPrintData.GeoSession != null)
            {

                string url = String.Format($"{labelPrintData.ApiUrl}shipping/shipment/{labelPrintData.ShipmentId}/label/");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "text/vnd.eltron-epl";

                WebHeaderCollection myWebHeaderCollection = httpWebRequest.Headers;
                myWebHeaderCollection.Add($"GeoClient:account/{labelPrintData.GeoAccount}");
                myWebHeaderCollection.Add($"GeoSession:{labelPrintData.GeoSession}");
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                string lineTerminater = "\r\n";
                string printerAutoModeCommand = "~S,ESA";
                string data = "";

                using (StreamReader sr = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        data += line;
                        data += lineTerminater;
                    }
                }

                if (data.EndsWith(lineTerminater))
                {
                    data = data.Substring(0, data.Length - lineTerminater.Length);
                }

                // count how many print are in response
                int count = data.Split(new string[] { "P1" }, StringSplitOptions.None).Length - 1;
                int threadSleepTime = 2500 * count;
                // add two seconds sleep time for each label

                Printer.Open(PortType.USB);
                Printer.Command.Send(printerAutoModeCommand);
                Printer.Command.Send(data);
                Thread.Sleep(threadSleepTime);
                Printer.Command.Send(printerAutoModeCommand);
                Printer.Close();

                // update status pf that shipment

                try
                {
                    var updateRequest = (HttpWebRequest)WebRequest.Create(siteUrl + $"api/sync/update-dispatch-for-label?shipmentId={labelPrintData.ShipmentId}");
                    updateRequest.Method = "GET";
                    updateRequest.ContentType = "application/json";
                    var response = (HttpWebResponse)updateRequest.GetResponse();
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                }
                catch (Exception)
                {
                    //log exceptions
                    return;
                }
            }
        }

        public static string GetMd5(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public class UserLoginStatusViewModel
        {
            public string UserName { get; set; }
            public string Md5Pass { get; set; }
            public int TenantId { get; set; }
        }

        public class UserLoginStatusResponseViewModel
        {
            public bool Success { get; set; }
            public string UserName { get; set; }
            public int UserId { get; set; }
        }

        public class PalletDispatchLabelPrintViewModel
        {
            public string ShipmentId { get; set; }
            public string GeoSession { get; set; }
            public string GeoAccount { get; set; }
            public string ApiUrl { get; set; }
        }

        private void frmLogin_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //Hide();
                //notifyIcon1.Visible = true;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            loginStatus = false;
            loginUserId = 0;
            userName = "";
            toggleVisibility(true);
            textBox1.Text = "";
            textBox2.Text = "";
            PrintTimer.Stop();

        }
    }
}
