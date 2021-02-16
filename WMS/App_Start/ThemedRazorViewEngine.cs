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
        public static bool ThemingEnabled = false;

        private string[] _newAreaViewLocations = new string[] {
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
            if (ThemingEnabled)
            {
                AreaViewLocationFormats = AppendLocationFormats(_newAreaViewLocations, AreaViewLocationFormats);

                AreaMasterLocationFormats = AppendLocationFormats(_newAreaMasterLocations, AreaMasterLocationFormats);

                AreaPartialViewLocationFormats =
                    AppendLocationFormats(_newAreaPartialViewLocations, AreaPartialViewLocationFormats);

                ViewLocationFormats = AppendLocationFormats(_newViewLocations, ViewLocationFormats);

                MasterLocationFormats = AppendLocationFormats(_newMasterLocations, MasterLocationFormats);

                PartialViewLocationFormats =
                    AppendLocationFormats(_newPartialViewLocations, PartialViewLocationFormats);
            }
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
            if (ThemingEnabled)
            {
                return base.CreateView(controllerContext, viewPath.Replace("%1", GetThemeName()), masterPath);
            }
            return base.CreateView(controllerContext, viewPath, masterPath);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            if (ThemingEnabled)
            {
                return base.CreatePartialView(controllerContext, partialPath.Replace("%1", GetThemeName()));
            }
            return base.CreatePartialView(controllerContext, partialPath);
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            if (ThemingEnabled)
            {
                return base.FileExists(controllerContext, virtualPath.Replace("%1", GetThemeName()));
            }
            return base.FileExists(controllerContext, virtualPath);
        }

        public string GetThemeName()
        {
            WarehouseThemeEnum ThemeName = WarehouseThemeEnum.Default;

            if (HttpContext.Current.Session["caTenant"] != null)
            {
                caTenant tenant = (caTenant)HttpContext.Current.Session["caTenant"];
                if (tenant.Theme > 0)
                {
                    Enum.TryParse<WarehouseThemeEnum>(Enum.GetName(typeof(WarehouseThemeEnum), tenant.Theme), out ThemeName);
                }
            }

            return ThemeName.ToString();

        }

    }
}