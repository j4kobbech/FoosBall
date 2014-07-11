namespace Foosball.Main
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using FoosBall.Main;
    using FoosBall.Models.Base;
    using MongoDB.Driver.Builders;

    public class AuthorisationFilterAttribute : ActionFilterAttribute
    {
        private Role role;

        public AuthorisationFilterAttribute(Role role)
        {
            this.role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var accessTokenHeaderValue = filterContext.HttpContext.Request.Headers.Get("AccessToken");

            if (accessTokenHeaderValue == null || string.IsNullOrWhiteSpace(accessTokenHeaderValue))
            {
                throw new HttpException(401, "Authentication failed");
            }

            var dbh = new Db().Dbh;
            var accessTokenCollection = dbh.GetCollection<AccessToken>("AccessTokens");
            var accessToken = accessTokenCollection.FindOne(Query.EQ("TokenValue", accessTokenHeaderValue));

            if (accessToken == null || accessToken.Expires.ToLocalTime() < DateTime.Now.ToLocalTime())
            {
                throw new HttpException(401, "Authentication failed");                
            }
        }
    }
}