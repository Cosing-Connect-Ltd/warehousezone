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
        public  Attachment _fileAttachment  { get; set; }
        private string _logourl { get; set; }
        private string _mailto { get; set; }
        private string _from { get; set; }
        private string _htmlBody { get; set; }
        private string _subject { get; set; }
        private string _attachment { get; set; }
        private string _smtphost {get;set;}
        private int _port{get;set;}
        private string _username {get;set;}
        private string _password {get;set;}
        private bool   _enableSsl {get;set;}


        public List<string> MailErrors=new List<string>();



        public EmailSender(string mailto,string from ,string htmlBody, string subject, string attachment, string smtphost, int port, string username, string password)
        {

            ///////////////////////////////////////////////////////////
            _logourl =  @"~\Content\images\Emaillogo.jpg";
            _mailto = mailto.Trim();
            _from = from.Trim();
            _htmlBody = htmlBody;
            _subject = subject.Trim();
            _attachment = attachment.Trim();
            _smtphost = smtphost.Trim();
            _port = port;
            _username = username.Trim();
            _password = password.Trim();
            _enableSsl = true;
          /////////////////////////////////////////////
        }
        public  bool SendMail(bool isBodyHtml = false)
        {
            try
            {

               //creating MailMessage Object
                MailMessage mailmsg = new MailMessage();
                mailmsg.AlternateViews.Add(embedlogo(_htmlBody));
                mailmsg.From = new MailAddress(_from);

                ///if there is comma
                if (_mailto.Count(x => x == ',') > 0)
                {

                    string[] fields = _mailto.Split(',');

                      foreach (string recp in fields)
                        mailmsg.To.Add(new MailAddress(recp));
                }else
                    mailmsg.To.Add(new MailAddress(_mailto));


                mailmsg.Subject = _subject;

                mailmsg.IsBodyHtml = isBodyHtml;

                if (_attachment != "")
                {

                    mailmsg.Attachments.Add(new Attachment(HttpContext.Current.Server.MapPath(_attachment)));
                }

                if (_fileAttachment != null)
                {
                    mailmsg.Attachments.Add(_fileAttachment);

                }


                SmtpClient smtp = new SmtpClient();
                smtp.Host = _smtphost;
                smtp.Port = _port;
                smtp.Credentials = new System.Net.NetworkCredential(_username,_password);
                smtp.EnableSsl =_enableSsl;

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


            if (File.Exists(HttpContext.Current.Server.MapPath(_logourl)))
            {

                // Create a LinkedResource object for each embedded image
                LinkedResource pic1 = new LinkedResource(HttpContext.Current.Server.MapPath(_logourl));

                pic1.ContentId = "logo";
                avHtml.LinkedResources.Add(pic1);
            }


            return avHtml;
        }







    }
}