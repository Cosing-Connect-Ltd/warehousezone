using System.Web;
using System.Web.Optimization;

namespace WarehouseEcommerce
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/unobtrusive").Include(
                        "~/Scripts/jquery.unobtrusive*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.js",
                      "~/Scripts/modal-steps.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/settings-bar.css",
                      "~/Content/font-awesome.min.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/Theme/Smart/app/css/smart").Include(
                      "~/Content/Theme/Smart/app/css/App.css",
                      "~/Content/Theme/Smart/app/css/Site.css"
                      ));
            bundles.Add(new StyleBundle("~/Content/Theme/University/app/css/university").Include(
                     "~/Content/Theme/University/app/css/app.css",
                     "~/Content/Theme/University/app/css/customfontIcons.css",
                     "~/Content/Theme/University/app/css/skew_carousle_style.css",
                     "~/Content/Theme/University/app/css/app_responsive.css",
                     "~/Content/owl.carousel/owl.carousel.css"

                     ));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                    "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/fonts/custom-fonts").Include(
                    "~/fonts/custom-fonts.css"));
        }
    }
}
