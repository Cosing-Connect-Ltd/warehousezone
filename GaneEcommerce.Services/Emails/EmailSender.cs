using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace Ganedata.Core.Services
{
    public class EmailSender
    {
        public  Attachment fileattachment  { get; set; }
        private string logourl { get; set; }
        private string mailto { get; set; }
        private string from { get; set; }
        private string htmlBody { get; set; }
        private string subject { get; set; }
        private string attachment { get; set; }
        private string smtphost {get;set;}
        private int port{get;set;} 
        private string username {get;set;}
        private string password {get;set;}
        private bool   EnableSsl {get;set;}     
        
             
        public List<string> MailErrors=new List<string>();



        public EmailSender(string mailto,string from ,string htmlBody, string subject, string attachment, string smtphost, int port, string username, string password)
        {
           
            ///////////////////////////////////////////////////////////
            this.logourl =  @"~\Content\images\Emaillogo.jpg";
            this.mailto = mailto.Trim();
            this.from = from.Trim();
            this.htmlBody = htmlBody;
            this.subject = subject.Trim();
            this.attachment = attachment.Trim();
            this.smtphost = smtphost.Trim();
            this.port = port;
            this.username = username.Trim();
            this.password = password.Trim();
            this.EnableSsl = true;
          /////////////////////////////////////////////
        }
        public  bool SendMail(){

            try
            {

               //creating MailMessage Object
                MailMessage mailmsg = new System.Net.Mail.MailMessage();
                mailmsg.AlternateViews.Add(this.embedlogo(htmlBody));
                mailmsg.From = new MailAddress(this.from);

                ///if there is comma
                if (this.mailto.Count(x => x == ',') > 0)
                {

                    string[] fields = mailto.Split(',');

                      foreach (string recp in fields)
                        mailmsg.To.Add(new MailAddress(recp));
                }else
                    mailmsg.To.Add(new MailAddress(mailto));


                mailmsg.Subject = this.subject;

                if (attachment != "")
                {

                    mailmsg.Attachments.Add(new Attachment(HttpContext.Current.Server.MapPath(attachment)));
                }

                if (fileattachment != null)
                {
                    mailmsg.Attachments.Add(fileattachment);

                }


                SmtpClient smtp = new SmtpClient();
                smtp.Host = this.smtphost;
                smtp.Port = this.port;
                smtp.Credentials = new System.Net.NetworkCredential(this.username,this.password);
                smtp.EnableSsl =this.EnableSsl;

                smtp.Send(mailmsg);
                return true;
            }
            catch (Exception ex)
            {
                MailErrors.Add(ex.Message);
                return false; 
            }
            
        }
        private AlternateView embedlogo(string htmlBody)
        {
            //  Create an AlternateView object for supporting the HTML
            AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);


            if (File.Exists(HttpContext.Current.Server.MapPath(this.logourl)))
            {

                // Create a LinkedResource object for each embedded image
                LinkedResource pic1 = new LinkedResource(HttpContext.Current.Server.MapPath(this.logourl));

                pic1.ContentId = "logo";
                avHtml.LinkedResources.Add(pic1);
            }
            
            
            return avHtml;
        }




        


    }
}