using System.Web;
using System.Web.Optimization;

namespace ExcellentMarketResearch
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.2.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.unobtrusive*",
            //            "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.unobtrusive.3.2.6.js",       
                "~/Scripts/jquery.validate.js*"
                       
                       ));

            // Use the development version of modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //  bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            //bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
            //            "~/Content/themes/base/jquery.ui.core.css",
            //            "~/Content/themes/base/jquery.ui.resizable.css",
            //            "~/Content/themes/base/jquery.ui.selectable.css",
            //            "~/Content/themes/base/jquery.ui.accordion.css",
            //            "~/Content/themes/base/jquery.ui.autocomplete.css",
            //            "~/Content/themes/base/jquery.ui.button.css",
            //            "~/Content/themes/base/jquery.ui.dialog.css",
            //            "~/Content/themes/base/jquery.ui.slider.css",
            //            "~/Content/themes/base/jquery.ui.tabs.css",
            //            "~/Content/themes/base/jquery.ui.datepicker.css",
            //            "~/Content/themes/base/jquery.ui.progressbar.css",
            //            "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapscript")
         .Include("~/Scripts/bootstrap.js"
         ));
            bundles.Add(new StyleBundle("~/bundles/bootstrapstyle")
                .Include("~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/bundles/templatestyle")
                .Include("~/css/bootstrap.min.css", // will look for bootstrap.min.css
                         "~/fonts/flaticon/flaticon.css",
                         "~/css/font-awesome.min.css",
                         "~/css/hippo-off-canvas.css",
                         "~/css/animate.css",
                         "~/css/language-select.css",
                         "~/owl.carousel/assets/owl.carousel.css",
                         "~/css/magnific-popup.css",
                         "~/css/menu.css",
                         "~/css/template.css",
                         "~/css/style.css",
                         "~/css/responsive.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/templatejs")
         .Include("~/js/vendor/modernizr-2.8.1.min.js"
         ));

            bundles.Add(new ScriptBundle("~/bundles/templatefooterjs")
         .Include("~/js/jquery.js",
                  "~/js/bootstrap.min.js",
                  "~/owl.carousel/owl.carousel.min.js", 
                  "~/js/jquery.magnific-popup.min.js",
                  "~/js/hippo-offcanvas.js",
                  "~/js/jquery.inview.min.js",
                  "~/js/jquery.stellar.js",
                  "~/js/jquery.countTo.js",
                  "~/js/classie.js",
                  "~/js/selectFx.js",
                  "~/js/jquery.sticky-kit.min.js",
                  "~/js/googlemapaip.js",
                  "~/js/twitterFetcher_min.js",
                  "~/js/scripts.js"
         ));


            BundleTable.EnableOptimizations = true;
        }
    }
}