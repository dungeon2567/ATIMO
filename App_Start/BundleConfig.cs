using System.Web.Optimization;

namespace Atimo
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*"
                //"~/Scripts/jquery.validate*",
                       // "~/Scripts/methods_pt.js")
                       ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/others").Include(
                       "~/Scripts/meiomask.js",
                       "~/Scripts/meio-mask-init.js",
                       "~/Scripts/Validacao.js",
                       "~/Scripts/json2.js",
                       "~/Scripts/bootstrap-filestyle.js"));

            bundles.Add(new ScriptBundle("~/bundles/DataTable").Include(
                        "~/Scripts/jquery.dataTables.js",
                        "~/Scripts/dataTables.tableTools.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/font").Include("~/Content/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/DataTable")
                .Include(
                       "~/Content/dataTables.bootstrap.css",
                       "~/Content/dataTables.tableTools.css"));

            bundles.Add(new StyleBundle("~/Content/style")
                .Include("~/Content/style.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}