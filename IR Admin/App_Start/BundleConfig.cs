using System.Web;
using System.Web.Optimization;

namespace IR_Admin
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Content/admin-press/assets/plugins/jquery/jquery.min.js",
                "~/Content/admin-press/assets/plugins/bootstrap/js/popper.min.js",
                "~/Content/admin-press/assets/plugins/bootstrap/js/bootstrap.min.js",
                "~/Content/admin-press/main/js/jquery.slimscroll.js",
                "~/Content/admin-press/main/js/waves.js",
                "~/Content/admin-press/main/js/sidebarmenu.js",
                "~/Content/admin-press/main/js/custom.min.js",
                "~/Content/admin-press/main/js/dashboard4.js",
                "~/Content/admin-press/assets/plugins/sticky-kit-master/dist/sticky-kit.min.js",
                "~/Content/admin-press/assets/plugins/sparkline/jquery.sparkline.min.js",
                "~/Content/admin-press/assets/plugins/raphael/raphael-min.js",
                "~/Content/admin-press/assets/plugins/morrisjs/morris.min.js",
                "~/Content/admin-press/assets/plugins/sparkline/jquery.sparkline.min.js",
                "~/Content/admin-press/assets/plugins/styleswitcher/jQuery.style.switcher.js"
                ));
      
            bundles.Add(new StyleBundle("~/Content/styles").Include(
                "~/Content/admin-press/assets/plugins/bootstrap/css/bootstrap.min.css",
                      "~/Content/admin-press/assets/plugins/morrisjs/morris.css",
                      "~/Content/admin-press/main/css/style.css",
                      "~/Content/admin-press/main/css/colors/blue.css"
                      ));
        }
    }
}
