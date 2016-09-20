namespace FoosBall.Controllers
{
    using System.Web.Mvc;

    public class TournamentController : BaseController
    {
        public ActionResult Index()
        {
            return View("Index"); 
        }
    }
}
