namespace FoosBall.Controllers
{
    using System.Web.Mvc;
    using Main;
    using Models.Base;
    using Models.Domain;
    using MongoDB.Driver;

    public class BaseController : Controller
    {
        protected readonly MongoDatabase Dbh;

        public BaseController()
        {
            this.Dbh = new Db().Dbh;
            this.Settings = this.Dbh.GetCollection<Config>("Config").FindOne();
            this.Settings.Environment = AppConfig.GetEnvironment();
        }

        protected Config Settings { get; set; }
        
        public JsonResult GetAppSettings(bool refresh = false)
        {
            var appSettings = new AppSettings
            {
                AppName = this.Settings.Name,
                SportName = this.Settings.SportName,
                Environment = this.Settings.Environment.ToString()
            };

            return Json(appSettings, JsonRequestBehavior.AllowGet);
        }

    }
}
