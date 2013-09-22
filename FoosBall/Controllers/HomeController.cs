namespace FoosBall.Controllers
{
    using System.Web.Mvc;
    using Models.ViewModels;

    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View(new HomeViewModel());
        }
        
        public ActionResult Features()
        {   
            return View(new FeaturesViewModel { Settings = Settings });
        }
    }
}
