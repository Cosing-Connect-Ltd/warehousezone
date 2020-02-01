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
                        "~/Areas/Shop/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Areas/Shop/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Areas/Shop/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Areas/Shop/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/app/css/styles").Include(
                      "~/Areas/Shop/Content/bootstrap.css",
                      "~/Areas/Shop/Content/app/css/app.css",
                      "~/Areas/Shop/Content/site.css"
                      ));

        }
    }
}
