using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMS.App_Start
{

    public class ThemedRazorViewEngine : RazorViewEngine
    {
        public static bool ThemingEnabled = true;
        private string[] _newAreaViewLocations = new string[]
        {
        "~/Areas/{2}/Views/Theme/%1/{1}/{0}.cshtml",
        "~/Areas/{2}/Views/Theme/%1/{1}/{0}.vbhtml",
        "~/Areas/{2}/Views/Theme/%1/Shared/{0}.cshtml",
        "~/Areas/{2}/Views/Theme/%1/Shared/{0}.vbhtml"
    };

        private string[] _newAreaMasterLocations = new string[] {
        "~/Areas/{2}/Views/Theme/%1/{1}/{0}.cshtml",
        "~/Areas/{2}/Views/Theme/%1/{1}/{0}.vbhtml",
        "~/Areas/{2}/Views/Theme/%1/Shared/{0}.cshtml",
        "~/Areas/{2}/Views/Theme/%1/Shared/{0}.vbhtml"
    };

        private string[] _newAreaPartialViewLocations = new string[] {
        "~/Areas/{2}/Views/Theme/%1/{1}/{0}.cshtml",
        "~/Areas/{2}/Views/Theme/%1/{1}/{0}.vbhtml",
        "~/Areas/{2}/Views/Theme/%1/Shared/{0}.cshtml",
        "~/Areas/{2}/Views/Theme/%1/Shared/{0}.vbhtml"
    };

        private string[] _newViewLocations = new string[] {
        "~/Views/Theme/%1/{1}/{0}.cshtml",
        "~/Views/Theme/%1/{1}/{0}.vbhtml",
        "~/Views/Theme/%1/Shared/{0}.cshtml",
        "~/Views/Theme/%1/Shared/{0}.vbhtml"
    };

        private string[] _newMasterLocations = new string[] {
        "~/Views/Theme/%1/{1}/{0}.cshtml",
        "~/Views/Theme/%1/{1}/{0}.vbhtml",
        "~/Views/Theme/%1/Shared/{0}.cshtml",
        "~/Views/Theme/%1/Shared/{0}.vbhtml"
    };

        private string[] _newPartialViewLocations = new string[] {
        "~/Views/Theme/%1/{1}/{0}.cshtml",
        "~/Views/Theme/%1/{1}/{0}.vbhtml",
        "~/Views/Theme/%1/Shared/{0}.cshtml",
        "~/Views/Theme/%1/Shared/{0}.vbhtml"
    };

        public ThemedRazorViewEngine()
            : base()
        {

            AreaViewLocationFormats = AppendLocationFormats(_newAreaViewLocations, AreaViewLocationFormats);

            AreaMasterLocationFormats = AppendLocationFormats(_newAreaMasterLocations, AreaMasterLocationFormats);

            AreaPartialViewLocationFormats = AppendLocationFormats(_newAreaPartialViewLocations, AreaPartialViewLocationFormats);

            ViewLocationFormats = AppendLocationFormats(_newViewLocations, ViewLocationFormats);

            MasterLocationFormats = AppendLocationFormats(_newMasterLocations, MasterLocationFormats);

            PartialViewLocationFormats = AppendLocationFormats(_newPartialViewLocations, PartialViewLocationFormats);
        }

        private string[] AppendLocationFormats(string[] newLocations, string[] defaultLocations)
        {
            List<string> viewLocations = new List<string>();
            viewLocations.AddRange(newLocations);
            viewLocations.AddRange(defaultLocations);
            return viewLocations.ToArray();
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return base.CreateView(controllerContext, viewPath.Replace("%1", GetThemeName()), masterPath);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return base.CreatePartialView(controllerContext, partialPath.Replace("%1", GetThemeName()));
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return base.FileExists(controllerContext, virtualPath.Replace("%1", GetThemeName()));
        }

        public string GetThemeName()
        {
            if (!ThemingEnabled) return null;

            WebsiteThemeEnum ThemeName = WebsiteThemeEnum.University;

            if (HttpContext.Current.Session["CurrentTenantWebsites"] != null)
            {
                caTenantWebsites website = (caTenantWebsites)HttpContext.Current.Session["CurrentTenantWebsites"];
                if (website.Theme > 0)
                {
                    Enum.TryParse<WebsiteThemeEnum>(Enum.GetName(typeof(WebsiteThemeEnum), website.Theme), out ThemeName);
                }
            }

            return ThemeName.ToString();
        }
    }
}