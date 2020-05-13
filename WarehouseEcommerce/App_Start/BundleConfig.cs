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

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/app/css/styles/smart").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Theme/Smart/app/css/app.css",
                      "~/Content/Theme/Smart/site.css"
                      ));
            bundles.Add(new StyleBundle("~/Content/app/css/styles/university").Include(
                     "~/Content/bootstrap.css",
                      "~/Content/Theme/Smart/app/css/app.css",
                      "~/Content/Theme/Smart/site.css",
                      "~/Content/Theme/University/UniversityCustomStyle.css",
                     "~/Content/Theme/University/app/css/northumbria_custom.css",
                     "~/Content/Theme/University/app/css/customfontIcons.css",
                      "~/Content/Theme/University/app/css/skew_carousle_style.css",
                       "~/Content/Theme/University/app/css/northumbria_responsive.css",
                        "~/Content/Theme/University/app/css/skew_carousle_style.css",
                        "~/Content/Theme/University/app/css/skew_carousle_style.css",
                      "~/Contents/owl.carousel/owl.carousel.css"
                     ));

        }
    }
}
