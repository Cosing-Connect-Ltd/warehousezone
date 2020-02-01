using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Services;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class DevexHelperController : Controller
    {
        // GET: DevexHelper
        public ActionResult HtmlEditorEmailTemplateHeaderPartial()
        {
            return PartialView("PlaceholdersHtmlEditorPartial", new HtmlEditorModel() { EditorName = "HtmlHeader", CallbackController = "DevexHelper", CallbackAction = "HtmlEditorEmailTemplateHeaderPartial", Height = 200 });
        }

        public ActionResult HtmlEditorEmailTemplateBodyPartial()
        {
            return PartialView("PlaceholdersHtmlEditorPartial", new HtmlEditorModel() { EditorName = "Body", CallbackController = "DevexHelper", CallbackAction = "HtmlEditorEmailTemplateBodyPartial" });
        }

        public ActionResult HtmlEditorEmailTemplateFooterPartial()
        {
            return PartialView("PlaceholdersHtmlEditorPartial", new HtmlEditorModel() { EditorName = "HtmlFooter", CallbackController = "DevexHelper", CallbackAction = "HtmlEditorEmailTemplateFooterPartial", Height = 200 });
        }

    }
}