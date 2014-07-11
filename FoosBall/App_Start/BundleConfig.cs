namespace FoosBall
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/all").Include(
                            "~/js/lib/jquery.js",
                            "~/js/lib/angular.js",
                            "~/js/lib/angular-resource.js",
                            "~/js/lib/angular-route.js",
                            "~/js/lib/md5.js",
                            "~/js/lib/highcharts.js",
                            "~/js/lib/event.js",
                            "~/js/app/jquery.foosBall.js",
                            "~/js/app/foosball.app.js",
                            "~/js/services/api.service.js",
                            "~/js/services/appsettings.service.js",
                            "~/js/services/session.service.js",
                            "~/js/services/advancedstats.service.js",
                            "~/js/services/staticresources.service.js",
                            "~/js/directives/foosball-score.directive.js",
                            "~/js/controllers/base.controller.js",
                            "~/js/controllers/admin.controller.js",
                            "~/js/controllers/login.controller.js",
                            "~/js/controllers/matches.controller.js",
                            "~/js/controllers/players.controller.js",
                            "~/js/controllers/playerstats.controller.js",
                            "~/js/controllers/signup.controller.js",
                            "~/js/controllers/stats.controller.js",
                            "~/js/controllers/submitmatch.controller.js",
                            "~/js/controllers/user.controller.js",
                            "~/js/controllers/advancedstats.controller.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
} 