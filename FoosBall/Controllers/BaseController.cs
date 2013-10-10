﻿namespace FoosBall.Controllers
{
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using ControllerHelpers;
    using Main;
    using Models.Base;
    using Models.Domain;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class BaseController : Controller
    {
        protected readonly MongoDatabase Dbh;

        public BaseController()
        {
            var environment = AppConfig.GetEnvironment();

            this.Dbh = new Db(environment).Dbh;
            this.Settings = this.Dbh.GetCollection<Config>("Config").FindOne();
            this.Settings.Environment = environment;

            ViewBag.Settings = this.Settings;
        }

        protected Config Settings { get; set; }
        
        public static string CalculateMd5(string input, Encoding useEncoding)
        {
            var cryptoService = new System.Security.Cryptography.MD5CryptoServiceProvider();

            var inputBytes = useEncoding.GetBytes(input);
            inputBytes = cryptoService.ComputeHash(inputBytes);
            return BitConverter.ToString(inputBytes).Replace("-", string.Empty);
        }

        // Calculates a MD5 hash from the given string. 
        // (By using the default encoding)
        public static string CalculateMd5(string input)
        {
            // That's just a shortcut to the base method
            return CalculateMd5(input, Encoding.Default);
        }

        public static string GetAuthToken(Player player)
        {
            return Md5.CalculateMd5(player.Id + player.Email + "FoosBall4Ever");
        }

        public JsonResult GetSession(bool refresh = false)
        {
            return Json(GetSessionInfo(refresh), JsonRequestBehavior.AllowGet);
        }

        protected SessionInfo GetSessionInfo(bool refresh = false)
        {
            SessionInfo session;
            if (Session["IsLoggedIn"] == null || Session["IsLoggedIn"].ToString() == "false")
            {
                session = new SessionInfo()
                {
                    IsAdmin = false,
                    IsLoggedIn = false,
                    User = null
                };
            }
            else
            {

                if (refresh)
                {
                    var player = (Player) Session["User"];
                    var refreshedPlayer = DbHelper.GetPlayer(player.Id);
                    session = new SessionInfo()
                    {
                        IsAdmin = Settings.AdminAccount.Contains(refreshedPlayer.Email),
                        IsLoggedIn = (bool)Session["IsLoggedIn"],
                        User = refreshedPlayer
                    };
                }
                else
                {
                    session = new SessionInfo()
                    {
                        IsAdmin = (bool)Session["Admin"],
                        IsLoggedIn = (bool)Session["IsLoggedIn"],
                        User = (Player)Session["User"]
                    };
                }
            }

            return session;
        }

        // COOKIE DOUGH
        public void CreateRememberMeCookie(Player player)
        {
            HttpContext.Response.Cookies.Add(new HttpCookie("FoosBallAuth"));
            var httpCookie = ControllerContext.HttpContext.Response.Cookies["FoosBallAuth"];
            if (httpCookie != null)
            {
                httpCookie["Token"] = GetAuthToken(player);
            }

            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddDays(30);
            }
        }

        public void RemoveRememberMeCookie()
        {
            ControllerContext.HttpContext.Response.Cookies.Add(new HttpCookie("FoosBallAuth"));
            var httpCookie = ControllerContext.HttpContext.Response.Cookies["FoosBallAuth"];
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddDays(-1);
            }
        }
        
        public bool Login(Player player)
        {
            // Set or remove cookie for future auto-login
            if (player != null) 
            {
                if (player.RememberMe)
                {
                    // Save an autologin token as cookie and in the Db
                    var playerCollection = this.Dbh.GetCollection<Player>("Players");
                    var autoLoginCollection = this.Dbh.GetCollection<AutoLogin>("AutoLogin");
                    var autoLogin = autoLoginCollection.FindOne(Query.EQ("Email", player.Email));

                    if (autoLogin == null)
                    {
                        autoLogin = new AutoLogin
                        {
                            Email = player.Email,
                            Token = GetAuthToken(player),
                        };
                        autoLoginCollection.Save(autoLogin);
                    }

                    CreateRememberMeCookie(player);
                    player.RememberMe = player.RememberMe;
                    playerCollection.Save(player);
                }
                else
                {
                    RemoveRememberMeCookie();
                }

                this.Session["Admin"] = Settings.AdminAccount.Contains(player.Email);
                this.Session["IsLoggedIn"] = true;
                this.Session["User"] = player;
            
                return true;
            }

            return false;
        }
    }
}
