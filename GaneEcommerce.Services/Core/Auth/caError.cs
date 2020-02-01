using Ganedata.Core.Entities.Helpers;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;

namespace Ganedata.Core.Services
{
    [Serializable]
    public class caError
    {
        public string ErrorTtile { set; get; }
        public string ErrorMessage { set; get; }
        public string ErrorDetail { set; get; }
        public string ErrorController { get; set; }
        public string ErrorAction { get; set; }

        public caError()
        {
            ErrorTtile = "Oops! Something went wrong :-(";
            ErrorMessage = "Something went wrong and we couldn't complete your request";
            ErrorDetail = "";
            ErrorController = "";
            ErrorAction = "";
        }

        // log application errors 

        public Boolean ErrorLogWriter()
        {
            Boolean status = false;

            int TenantId = 0;
            int UserId = 0;
            string TenantName = "";
            string UserName = "";
            string SubDoamin = "";

            if (HttpContext.Current.Session["caError"] != null)
            {
                caError error = (caError)HttpContext.Current.Session["caError"];
                ErrorTtile = error.ErrorTtile;
                ErrorMessage = error.ErrorMessage;
                ErrorDetail = error.ErrorDetail;
                ErrorController = error.ErrorController;
                ErrorAction = error.ErrorAction;

                if (HttpContext.Current.Session["caTenant"] != null)
                {
                    // current tenant id, name and subdomain form session
                    caTenant tenant = (caTenant)HttpContext.Current.Session["caTenant"];
                    TenantId = tenant.TenantId;
                    TenantName = tenant.TenantName;
                    SubDoamin = tenant.TenantSubDmoain;
                }

                if (HttpContext.Current.Session["caUser"] != null)
                {
                    // get properties of user
                    caUser user = (caUser)HttpContext.Current.Session["caUser"];
                    UserId = user.UserId;
                    UserName = user.UserName;
                }

                // write in error log file using string builder and stream writer
                StringBuilder builder = new StringBuilder();
                builder
                    .AppendLine("Date/Time: " + DateTime.UtcNow.ToString())
                    .AppendLine("Tenant Name: " + TenantName + "( Teanant Id: " + TenantId + ")")
                    .AppendLine("User: " + UserName + "( Id: " + UserId + ")")
                    .AppendLine("Controller: " + ErrorController)
                    .AppendLine("Action: " + ErrorAction)
                    .AppendLine("Error Title: " + ErrorTtile)
                    .AppendLine("Error Message: " + ErrorMessage)
                    .AppendLine("Error Detail: " + ErrorDetail)
                    .AppendLine("-----------------------------------------------")
                    .Append(Environment.NewLine);


                // get log preferences Local | Azure | Both
                int LogWriting = Convert.ToInt32(ConfigurationManager.AppSettings.Get("LogWriting"));

                if (LogWriting != 0)
                {

                    if (LogWriting == 1 || LogWriting == 3)
                    {
                        //*************************** File Writing for Local Storage *****************************************

                        string filePath = HttpContext.Current.Server.MapPath("~/Logs/Error.log");

                        using (StreamWriter writer = File.AppendText(filePath))
                        {
                            writer.Write(builder.ToString());
                            writer.Flush();
                        }
                    }

                    //if (LogWriting == 2 || LogWriting == 3)
                    //{
                    //    //*************************** Blob Writing for Azure  Hosting only *****************************************

                    //    // Retrieve storage account from connection string.
                    //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    //         CloudConfigurationManager.GetSetting("StorageConnectionString"));

                    //    // Create the blob client.
                    //    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    //    // Retrieve a reference to a container. 
                    //    CloudBlobContainer container = blobClient.GetContainerReference("logs");

                    //    CloudBlockBlob blob = container.GetBlockBlobReference("Error.log");
                    //    string contents = builder.ToString(); /* content to append */

                    //    if (blob.Exists())
                    //    {
                    //        using (Stream blobStream = blob.OpenRead())
                    //        {
                    //            byte[] buffer = new byte[4096];
                    //            using (Stream tempBlobStream = blob.OpenWrite())
                    //            {
                    //                int read;
                    //                while ((read = blobStream.Read(buffer, 0, 4096)) > 0)
                    //                {
                    //                    tempBlobStream.Write(buffer, 0, read);
                    //                }

                    //                using (StreamWriter writer = new StreamWriter(tempBlobStream))
                    //                {
                    //                    writer.Write(contents);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }

                status = true;
            }

            return status;
        }




        public Boolean SimpleErrorLogWriter()
        {
            Boolean status = false;

            // write in error log file using string builder and stream writer
            StringBuilder builder = new StringBuilder();
            builder
                .AppendLine("Date/Time: " + DateTime.UtcNow.ToString())
                .AppendLine("Controller: " + ErrorController)
                .AppendLine("Action: " + ErrorAction)
                .AppendLine("Error Title: " + ErrorTtile)
                .AppendLine("Error Message: " + ErrorMessage)
                .AppendLine("Error Detail: " + ErrorDetail)
                .AppendLine("-----------------------------------------------")
                .Append(Environment.NewLine);


            // get log preferences Local | Azure | Both
            int LogWriting = Convert.ToInt32(ConfigurationManager.AppSettings.Get("LogWriting"));

            if (LogWriting != 0)
            {

                if (LogWriting == 1 || LogWriting == 3)
                {
                    //*************************** File Writing for Local Storage *****************************************

                    string filePath = HttpContext.Current.Server.MapPath("~/Logs/Error.log");

                    using (StreamWriter writer = File.AppendText(filePath))
                    {
                        writer.Write(builder.ToString());
                        writer.Flush();
                    }
                }

                //if (LogWriting == 2 || LogWriting == 3)
                //{
                //    //*************************** Blob Writing for Azure  Hosting only *****************************************

                //    // Retrieve storage account from connection string.
                //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                //         CloudConfigurationManager.GetSetting("StorageConnectionString"));

                //    // Create the blob client.
                //    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                //    // Retrieve a reference to a container. 
                //    CloudBlobContainer container = blobClient.GetContainerReference("logs");

                //    CloudBlockBlob blob = container.GetBlockBlobReference("Error.log");
                //    string contents = builder.ToString(); /* content to append */

                //    if (blob.Exists())
                //    {
                //        using (Stream blobStream = blob.OpenRead())
                //        {
                //            byte[] buffer = new byte[4096];
                //            using (Stream tempBlobStream = blob.OpenWrite())
                //            {
                //                int read;
                //                while ((read = blobStream.Read(buffer, 0, 4096)) > 0)
                //                {
                //                    tempBlobStream.Write(buffer, 0, read);
                //                }

                //                using (StreamWriter writer = new StreamWriter(tempBlobStream))
                //                {
                //                    writer.Write(contents);
                //                }
                //            }
                //        }
                //    }
                //}
            }

            status = true;

            return status;
        }
    }
}