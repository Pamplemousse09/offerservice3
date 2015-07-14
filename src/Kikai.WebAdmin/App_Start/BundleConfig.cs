using System.Web;
using System.Web.Optimization;

namespace Kikai.WebAdmin
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/providerval").Include(
                        "~/Scripts/provider.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));          

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css"));

            //Color scheme bundles
            bundles.Add(new StyleBundle("~/Content/bluescheme").Include(
                      "~/Content/Site_blue.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/cyanscheme").Include(
                      "~/Content/Site_cyan.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/grayscheme").Include(
                      "~/Content/Site_gray.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/greenscheme").Include(
                      "~/Content/Site_green.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/magentascheme").Include(
                      "~/Content/Site_magenta.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/orangescheme").Include(
                     "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/redscheme").Include(
                      "~/Content/Site_red.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/yellowscheme").Include(
                      "~/Content/Site_yellow.css",
                      "~/Content/Site.css"));

            //Knockout
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/knockout-3.2.0.js",
                "~/Scripts/date.js",
                "~/Scripts/app.js"));

            //Knockout Editor
            bundles.Add(new ScriptBundle("~/bundles/editor").Include(
                "~/Scripts/knockout-3.2.0.js",
                "~/Scripts/editor.js"));

            //ZeroClipboard
            bundles.Add(new ScriptBundle("~/bundles/zeroclipboard").Include(
                "~/Scripts/ZeroClipboard.js"));

            //Javascript files for pages
            bundles.Add(new ScriptBundle("~/bundles/general").Include(
                        "~/Scripts/general.js"));

            //Javascript files for pages
            bundles.Add(new ScriptBundle("~/bundles/attribute").Include(
                        "~/Scripts/attribute.js"));

            //Javascript files for pages
            bundles.Add(new ScriptBundle("~/bundles/provider").Include(
                        "~/Scripts/provider.js"));

            //Javascript files for pages
            bundles.Add(new ScriptBundle("~/bundles/provideredit").Include(
                        "~/Scripts/provideredit.js"));

            //Javascript files for pages
            bundles.Add(new ScriptBundle("~/bundles/provideradd").Include(
                        "~/Scripts/provideradd.js"));

            //Javascript files for pages
            bundles.Add(new ScriptBundle("~/bundles/offerdetails").Include(
                        "~/Scripts/offerdetails.js"));

            //Javascript files for pages
            bundles.Add(new ScriptBundle("~/bundles/offerview").Include(
                        "~/Scripts/offerview.js"));

            //Javascript files for pages
            bundles.Add(new ScriptBundle("~/bundles/offer").Include(
                        "~/Scripts/offer.js"));
        }
    }
}
