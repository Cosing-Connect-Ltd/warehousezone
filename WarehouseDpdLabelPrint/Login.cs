using EzioDll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using Timer = System.Windows.Forms.Timer;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Configuration;

namespace WarehouseDpdLabelPrint
{
    public partial class frmLogin : Form
    {
        GodexPrinter Printer = new GodexPrinter();
        bool loginStatus = false;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            int tenantId = Convert.ToInt32(ConfigurationManager.AppSettings["TenantId"]);
            string url = ConfigurationManager.AppSettings["SiteUrl"];
            // Get shipments id from warehouse system

            UserLoginStatusViewModel userModel = new UserLoginStatusViewModel();
            userModel.UserName = textBox1.Text;
            userModel.Md5Pass = GetMd5(textBox2.Text);
            userModel.TenantId = tenantId;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "api/sync/get-login-status");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(userModel);

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                textBox1.Visible = false;
                textBox2.Visible = false;
                button1.Visible = false;
                loginStatus = true;

                label3.Text = "failed";
                label3.BackColor = Color.Green;
                //frmLogin.ActiveForm.WindowState = FormWindowState.Minimized;


                Timer PrintTimer = new Timer();
                PrintTimer.Interval = (30 * 1000); // 10 seconds
                PrintTimer.Tick += new EventHandler(PrintTimer_Tick);
                PrintTimer.Start();


            }

            else
            {
                label3.Text = "failed";
                label3.BackColor = Color.Red;
            }

        }

        private void PrintTimer_Tick(object sender, EventArgs e)
        {
            CheckLabelsToPrint();
        }

        private void CheckLabelsToPrint()
        {
            //print labels
            string shipmentId = "466471865";
            string url = String.Format("https://api.dpd.co.uk/shipping/shipment/{0}/label/", shipmentId);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "text/vnd.eltron-epl";

            WebHeaderCollection myWebHeaderCollection = httpWebRequest.Headers;
            myWebHeaderCollection.Add("GeoClient:account/ITC142468");
            myWebHeaderCollection.Add("GeoSession:MTAuMjYuMy42OXw5MzI0MTc5MDg=");
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

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
