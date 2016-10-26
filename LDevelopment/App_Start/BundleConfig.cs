using System.Web;
using System.Web.Optimization;

namespace LDevelopment
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-{version}.js", "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/editor")
                .Include("~/Scripts/bootstrap-multiselect.js", "~/Scripts/ldev/editor.js"));

            bundles.Add(new ScriptBundle("~/bundles/analytics")
                .Include("~/Scripts/ldev/analytics.js"));

            bundles.Add(new ScriptBundle("~/bundles/insights")
                .Include("~/Scripts/ldev/insights.js"));

            bundles.Add(new ScriptBundle("~/bundles/parallax")
                .Include("~/Scripts/ldev/parallax.js"));

            /* STYLES */

            bundles.Add(new StyleBundle("~/Content/Styles/css")
                .Include("~/Content/Styles/bootstrap.css", "~/Content/Styles/site.css", "~/Content/Styles/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/Styles/multiselect")
                .Include("~/Content/Styles/bootstrap-multiselect.css"));
        }
    }
}
