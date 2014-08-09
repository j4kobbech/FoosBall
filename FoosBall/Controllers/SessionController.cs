namespace FoosBall.Controllers
{
    using System.Web;
    using System.Web.Mvc;
    using ControllerHelpers;
    using Foosball.Main;
    using Main;
    using Models.Base;
    using MongoDB.Driver.Builders;

    public class SessionController : BaseController
    {
        [HttpPost]
        [AuthorisationFilter(Role.User)]
        public ActionResult Logout()
        {
            var accessTokenHeaderValue = HttpContext.Request.Headers.Get("AccessToken");

            if (accessTokenHeaderValue == null || string.IsNullOrWhiteSpace(accessTokenHeaderValue))
            {
                throw new HttpException(401, "Authentication failed");
            }

            var dbh = new Db().Dbh;
            var accessTokenCollection = dbh.GetCollection<AccessToken>("AccessTokens");
            accessTokenCollection.Remove(Query.EQ("TokenValue", accessTokenHeaderValue));

            return Json(new AjaxResponse { Success = true, Message = "User Logged out" });
        }

        [HttpGet]
        public ActionResult GetSession()
        {
            var ajaxResponse = new AjaxResponse { };

            return Json(ajaxResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var ajaxResponse = new AjaxResponse { Message = "user or password is incorrect" };
            var user = DbHelper.GetUserByEmail(email);

            if (Main.Session.IsUserCredentialsValid(user, password))
            {
                var accessToken = Main.Session.CreateNewAccessTokenOnUser(user);
                Main.Session.SaveAccessToken(accessToken);
                
                ajaxResponse.Success = true;
                ajaxResponse.Message = "Access granted";
                ajaxResponse.Data = new { AccessToken = Main.Session.BuildSessionInfo(accessToken, user) };
            }
            
            return Json(ajaxResponse);  
        }
    }
}