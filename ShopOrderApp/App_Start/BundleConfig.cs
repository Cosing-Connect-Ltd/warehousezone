using System.Web;
using System.Web.Optimization;

namespace ShopOrderApp
{

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.5.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/unobtrusive").Include(
                        "~/Scripts/jquery.unobtrusive*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/cookie").Include(
                "~/Scripts/jquery.cookie.js"));


            //see Report prerequesities https://documentation.devexpress.com/#Dashboard/CustomDocument16936
            //note: the date.js must be initialize at the end to prevent related Reporting issue https://www.devexpress.com/Support/Center/Question/Details/T115434
            bundles.Add(new ScriptBundle("~/bundles/globalize").Include(
                "~/Scripts/globalize.js",
                "~/Scripts/globalize/plural.js",
                "~/Scripts/globalize/relative-time.js",
                "~/Scripts/globalize/unit.js",
                "~/Scripts/globalize/message.js",
                "~/Scripts/globalize/number.js",
                "~/Scripts/globalize/currency.js",
                "~/Scripts/globalize/date.js"));

            bundles.Add(new ScriptBundle("~/bundles/cldr").Include(
                "~/Scripts/cldr.js",
                "~/Scripts/cldr/event.js",
                "~/Scripts/cldr/supplemental.js"));

            bundles.Add(new ScriptBundle("~/bundles/ace").Include(
                "~/Scripts/ace.js"));


           

            bundles.Add(new ScriptBundle("~/bundles/ext-language").Include(
                "~/Scripts/ext-language_tools.js"));

            bundles.Add(new StyleBundle("~/Content/Themes/UI-WZ/uiwz").Include(
                "~/Content/Themes/UI-WZ/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/fontawesome-bundle").Include(
                "~/Content/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
              "~/Content/bootstrap.css",
              "~/Content/Chosen/component-chosen.css",
              "~/Content/settings-bar.css",
              "~/Scripts/qTip/jquery.qtip.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/common").Include(
                "~/Content/themes/base/core.css",
                "~/Content/themes/base/resizable.css",
                "~/Content/themes/base/selectable.css",
                "~/Content/themes/base/accordion.css",
                "~/Content/themes/base/autocomplete.css",
                "~/Content/themes/base/button.css",
                "~/Content/themes/base/dialog.css",
                "~/Content/themes/base/slider.css",
                "~/Content/themes/base/tabs.css",
                "~/Content/themes/base/datepicker.css",
                "~/Content/themes/base/progressbar.css",
                "~/Content/themes/base/theme.css"));

            bundles.Add(new StyleBundle("~/Content/Theme/default/css").Include(
             "~/Content/Theme/Default/site.css"));

            bundles.Add(new StyleBundle("~/Content/Theme/modern/css").Include(
             "~/Content/Theme/Modern/site.css"));
        }
    }

}
