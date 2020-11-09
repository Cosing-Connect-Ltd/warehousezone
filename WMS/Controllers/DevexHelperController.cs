using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Ganedata.Core.Services;
using WMS.Helpers;
using WMS.Helpers.DevexHelpers;

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
        public ActionResult HtmlEditorWebsiteContentEditorPartial()
        {
            return PartialView("PlaceholdersHtmlEditorPartial", new HtmlEditorModel() { EditorName = "Content", CallbackController = "DevexHelper", CallbackAction = "HtmlEditorWebsiteContentEditorPartial", Height = 200 });
        }
        public ActionResult HtmlEditorWebsiteDescriptionEditorPartial()
        {
            return PartialView("PlaceholdersHtmlEditorPartial", new HtmlEditorModel() { EditorName = "Description", CallbackController = "DevexHelper", CallbackAction = "HtmlEditorWebsiteDescriptionEditorPartial", Height = 300 });
        }

        public ActionResult HtmlEditorLoyaltyAppAboutUsPageTextEditorPartial()
        {
            return PartialView("PlaceholdersHtmlEditorPartial", new HtmlEditorModel() { EditorName = "LoyaltyAppAboutUsPageText", CallbackController = "DevexHelper", CallbackAction = "HtmlEditorLoyaltyAppAboutUsPageTextEditorPartial", Height = 300 });
        }
        public ActionResult ImageUpload(string editorName)
        {
            if (!Directory.Exists(Server.MapPath(HtmlEditorFeaturesDemosHelper.ImagesDirectory)))
            {
                Directory.CreateDirectory(Server.MapPath(HtmlEditorFeaturesDemosHelper.ImagesDirectory));
            }

            HtmlEditorExtension.SaveUploadedFile(string.IsNullOrEmpty(editorName)?"Content":editorName, HtmlEditorFeaturesDemosHelper.ImageUploadValidationSettings, HtmlEditorFeaturesDemosHelper.ImagesDirectory);
            return null;
        }
    }
}