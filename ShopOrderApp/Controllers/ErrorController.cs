using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOrderApp.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            caError error = new caError();
            ViewBag.ErrorTtile = error.ErrorTtile;
            ViewBag.ErrorMessage = error.ErrorMessage;
            ViewBag.ErrorDetail = error.ErrorDetail;
            if (Request.UrlReferrer != null)
            {
                Session["LastUrlFrom"] = Request.UrlReferrer.AbsoluteUri;
            }

            if (Session["caError"] != null)
            {
                caError error2 = (caError)Session["caError"];
                ViewBag.ErrorTtile = error2.ErrorTtile;
                ViewBag.ErrorMessage = error2.ErrorMessage;
                ViewBag.ErrorDetail = error2.ErrorDetail;
            }

            error.ErrorLogWriter();
            return View();
        }
    }
}