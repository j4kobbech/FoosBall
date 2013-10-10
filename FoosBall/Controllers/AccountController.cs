namespace FoosBall.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using ControllerHelpers;
    using Main;
    using Models.Base;
    using Models.Domain;
    using Models.ViewModels;
    using MongoDB.Bson;
    using MongoDB.Driver.Builders;
    
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult GetPlayer()
        {
            var currentUser = (Player)Session["User"];

            return Json(DbHelper.GetPlayer(currentUser.Id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOn()
        {
            if (Session["IsLoggedIn"] == null || Session["IsLoggedIn"].ToString() == "false")
            {
                var authCookie = this.Request.Cookies.Get("FoosBallAuth");
                if (authCookie != null && authCookie["Token"] != null)
                {
                    var autoLoginCollection = this.Dbh.GetCollection<AutoLogin>("AutoLogin");
                    var autoLoginToken = autoLoginCollection.FindOne(Query.EQ("Token", authCookie["Token"]));

                    if (autoLoginToken != null) 
                    {
                        var playerCollection = this.Dbh.GetCollection<Player>("Players");
                        var player = playerCollection.FindOne(Query.EQ("Email", autoLoginToken.Email.ToLower()));
                    
                        if (Login(player))
                        {
                            // Go back to where we were before logging in
                            var referrer = this.Request.UrlReferrer;
                            if (referrer != null)
                            {
                                return this.Redirect(referrer.ToString());
                            }
                        } 
                    }
                }
            }

            var urlReferrer = this.Request.UrlReferrer;
            var viewModel = new LogOnViewModel { RefUrl = urlReferrer.ToString(), Settings = this.Settings };

            if (urlReferrer != null)
            {
                return this.View(viewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult LogOn(string email, string password, string refUrl, bool rememberMe = false)
        {
            var loginEmail = (email + "@" + Settings.Domain).ToLower();
            var playerCollection = Dbh.GetCollection<Player>("Players");
            var player = playerCollection.FindOne(Query.EQ("Email", loginEmail));
            var loginInfo = new AjaxResponse();

            if (player != null)
            {
                if (player.Password == Md5.CalculateMd5(password))
                {
                    if (Login(player))
                    {
                        loginInfo.Message = "Success";
                        loginInfo.Data = GetSessionInfo();
                        loginInfo.Success = true;
                    }
                }
            }
            else
            {
                loginInfo.Message = "Wrong user name or password";
                loginInfo.Data = GetSessionInfo(refresh: true);
                loginInfo.Success = false;
            }

            return Json(loginInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOff()
        {
            Session.Clear();
            RemoveRememberMeCookie();
            
            // Go back to where we were before logging in
            var urlReferrer = this.Request.UrlReferrer;
            if (urlReferrer != null)
            {
                return this.Redirect(urlReferrer.ToString());
            } 
                
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            var viewModel = new PlayerBaseDataViewModel
                                {
                                    Player = new Player(),
                                    Settings = this.Settings
                                };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Register(PlayerBaseDataViewModel viewModel)
        {
            var email = viewModel.Player.Email.ToLower();
            email += "@" + this.Settings.Domain;

            var name = viewModel.Player.Name;
            var password = Md5.CalculateMd5(viewModel.Player.Password);
            
            var playerCollection = this.Dbh.GetCollection<Player>("Players");

            var newPlayer = new Player
                                {
                                    Id = BsonObjectId.GenerateNewId().ToString(),
                                    Email = email,
                                    Name = name,
                                    Password = password,
                                    Won = 0,
                                    Lost = 0,
                                    Played = 0
                                };

            playerCollection.Save(newPlayer);

            Login(newPlayer);

            Events.SubmitEvent(EventType.PlayerCreate, newPlayer, newPlayer.Id);

            return this.Redirect(Url.Action("Index", "Players") + "#" + newPlayer.Id);
        }

        [HttpPost]
        public ActionResult Edit(string email, string name, string oldPassword = "", string newPassword = "")
        {
            var response = new AjaxResponse { Success = false };
            var currentUser = (Player)Session["User"];
            var player = DbHelper.GetPlayer(currentUser.Id);
            var newMd5Password = Md5.CalculateMd5(newPassword);

            if (player == null)
            {
                response.Message = "You have to be logged in to change user information";
                return Json(response);
            }

            if (!ValidateEmail(email))
            {
                response.Message = "You must provide a valid trustpilot email";
                return Json(response);
            }

            player.Email = string.IsNullOrEmpty(email) ? player.Email : email;
            player.Name = string.IsNullOrEmpty(name) ? player.Name : name;

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (Md5.CalculateMd5(oldPassword) == player.Password)
                {
                    player.Password = newMd5Password;
                }
                else
                {
                    response.Message = "The old password is not correct";
                    return Json(response);
                }
            }

            DbHelper.SavePlayer(player);
            response.Success = true;
            response.Message = "User updated succesfully";
            response.Data = GetSession(refresh: true);

            return Json(response);
        }

        [HttpPost]
        public JsonResult PlayerEmailIsValid(string email)
        {
            var query = Query.EQ("Email", email.ToLower());
            var playerCollection = this.Dbh.GetCollection<Player>("Players");
            var player = playerCollection.FindOne(query);

            if (player != null)
            {
                return Json(new ExistsResponse { Exists = true, Name = player.Name, Email = player.Email });
            }
    
            return Json(new ExistsResponse { Exists = false, Name = null, Email = null });
        }

        [HttpPost]
        public JsonResult PlayerNameExists(string name)
        {
            var playerCollection = this.Dbh.GetCollection<Player>("Players");
            var query = Query.EQ("Name", name);
            var player = playerCollection.FindOne(query);

            if (player != null)
            {
                return Json(new ExistsResponse { Exists = true, Name = player.Name, Email = player.Email });
            }

            return Json(new ExistsResponse { Exists = false, Name = null, Email = null });
        }

        [HttpGet]
        public JsonResult GetGravatarUrl(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var gravatarUrl = Md5.GetGravatarEmailHash(email);
                return Json(new { url = gravatarUrl }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { url = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        private bool ValidateEmail(string email)
        {
            const string domain = "@trustpilot.com";

            return !string.IsNullOrEmpty(email) &&
                   email.EndsWith(domain) &&
                   email.Count(x => x == '@') == 1 &&
                   email.Length > domain.Length;
        }
    }
}
