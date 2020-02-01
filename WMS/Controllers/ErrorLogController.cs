using Ganedata.Core.Services;
using System;
using System.Web;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class ErrorLogController : Controller
    {
        //
        // GET: /Elmah/
        public ActionResult Index(string type)
        {

            // check authorization status
            // if authorize false then return to the appropirate redirection
            // redirections are set in casession class
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return new ElmahResult(type);
        }
    }

    class ElmahResult : ActionResult
    {
        private string _resouceType;

        public ElmahResult(string resouceType)
        {
            _resouceType = resouceType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var factory = new Elmah.ErrorLogPageFactory();

            if (!string.IsNullOrEmpty(_resouceType))
            {
                var pathInfo = "." + _resouceType;
                HttpContext.Current.RewritePath(PathForStylesheet(), pathInfo, HttpContext.Current.Request.QueryString.ToString());
            }

            var httpHandler = factory.GetHandler(HttpContext.Current, null, null, null);
            httpHandler.ProcessRequest(HttpContext.Current);
        }

        private string PathForStylesheet()
        {
            return _resouceType != "stylesheet" ? HttpContext.Current.Request.Path.Replace(String.Format("/{0}", _resouceType), string.Empty) : HttpContext.Current.Request.Path;
        }
    }
}