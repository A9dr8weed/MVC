using System.Web;
using System.Web.Optimization;

namespace MVS_Store
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(bundle: new ScriptBundle(virtualPath: "~/bundles/jquery").Include(
                        virtualPath: "~/Scripts/jquery-{version}.js"));

            bundles.Add(bundle: new ScriptBundle(virtualPath: "~/bundles/jqueryval").Include(
                        virtualPath: "~/Scripts/jquery.validate*"));

            // Підключаємо нову бібліотеку JQuery.UI
            bundles.Add(bundle: new ScriptBundle(virtualPath: "~/bundles/jqueryui").Include(
                        virtualPath: "~/Scripts/jquery-ui.js"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // готово к выпуску, используйте средство сборки по адресу https://modernizr.com, чтобы выбрать только необходимые тесты.
            bundles.Add(bundle: new ScriptBundle(virtualPath: "~/bundles/modernizr").Include(
                        virtualPath: "~/Scripts/modernizr-*"));

            bundles.Add(bundle: new ScriptBundle(virtualPath: "~/bundles/bootstrap").Include(
                      virtualPath: "~/Scripts/bootstrap.js"));

            bundles.Add(bundle: new StyleBundle(virtualPath: "~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/jquery-ui.css"));

            // Підключення нової бібліотеки CKEditor
            bundles.Add(bundle: new ScriptBundle(virtualPath: "~/bundles/ckeditor").Include(
                      virtualPath: "~/Scripts/ckeditor/ckeditor.js"));
        }
    }
}