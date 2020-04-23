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
                      "~/Areas/Shop/Content/Theme/Smart/app/css/app.css",
                      "~/Areas/Shop/Content/Theme/Smart/site.css"
                      ));

        }
    }
}
