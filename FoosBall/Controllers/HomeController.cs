namespace FoosBall.Controllers
{
    using System.Web.Mvc;

    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var dingo = this.Request;
            return View();
        }
        
        public ActionResult Features()
        {   
            return View();
        }
    }
}
