namespace FoosBall.Controllers
{
    using System.Web.Mvc;
    using ControllerHelpers;
    using Main;
    using Models.Base;
    using Models.Domain;
    using MongoDB.Bson;
    using MongoDB.Driver.Builders;
    
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult GetUser()
        {
            var currentUser = (User)Session["User"];

            return Json(DbHelper.GetUser(currentUser.Id), JsonRequestBehavior.AllowGet);
        }

        public bool Login(User user)
        {
            // Set or remove cookie for future auto-login
            if (user != null)
            {
                if (user.RememberMe)
                {
                    // Save an autologin token as cookie and in the Db
                    var userCollection = Dbh.GetCollection<User>("Users");
                    var autoLoginCollection = Dbh.GetCollection<AutoLogin>("AutoLogin");
                    var autoLogin = autoLoginCollection.FindOne(Query.EQ("Email", user.Email));

                    if (autoLogin == null)
                    {
                        autoLogin = new AutoLogin
                        {
                            Email = user.Email,
                            Token = GetAuthToken(user),
                        };
                        autoLoginCollection.Save(autoLogin);
                    }

                    CreateRememberMeCookie(user);
                    user.RememberMe = user.RememberMe;
                    userCollection.Save(user);
                }
                else
                {
                    RemoveRememberMeCookie();
                }

                Session["Admin"] = Settings.AdminAccounts.Contains(user.Email);
                Session["IsLoggedIn"] = true;
                Session["User"] = user;

                return true;
            }

            return false;
        }
        
        [HttpGet]
        public void LogOn()
        {
            var autoLoginCollection = this.Dbh.GetCollection<AutoLogin>("AutoLogin");
            var userCollection = this.Dbh.GetCollection<User>("Users");
            
            if (Session["IsLoggedIn"] != null && Session["IsLoggedIn"].ToString() != "false")
            {
                return;
            }

            var authCookie = this.Request.Cookies.Get("FoosBallAuth");

            if (authCookie == null || authCookie["Token"] == null)
            {
                return;
            }

            var autoLoginToken = autoLoginCollection.FindOne(Query.EQ("Token", authCookie["Token"]));
            
            if (autoLoginToken == null)
            {
                return;
            }

            var user = userCollection.FindOne(Query.EQ("Email", autoLoginToken.Email.ToLower()));

            Login(user);
        }

        [HttpPost]
        public ActionResult LogOn(string email, string password, string refUrl, bool rememberMe = false)
        {
            var loginEmail = email;
            var userCollection = Dbh.GetCollection<User>("Users");
            var user = userCollection.FindOne(Query.EQ("Email", loginEmail));
            var loginInfo = new AjaxResponse();

            if (user != null)
            {
                if (user.Password == Md5.CalculateMd5(password))
                {
                    if (Login(user))
                    {
                        loginInfo.Message = "Success";
                        loginInfo.Data = GetSessionInfo();
                        loginInfo.Success = true;
                    }
                }
                else
                {
                    loginInfo.Message = "Wrong user name or password";
                    loginInfo.Success = false;
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
            var urlReferrer = Request.UrlReferrer;
            if (urlReferrer != null)
            {
                return Redirect(urlReferrer.ToString());
            } 
                
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Register(string email, string name, string password)
        {
            var userEmail = email.ToLower();
            var userName = name;
            var userPassword = Md5.CalculateMd5(password);
            var validation = new Validation();

            var response = validation.ValidateNewUserData(userEmail, userName, userPassword);
            if (!response.Success)
            {
                return Json(response);
            }

            var userCollection = Dbh.GetCollection<User>("Users");
            var newUser = new User
            {
                Id = BsonObjectId.GenerateNewId().ToString(),
                Email = userEmail,
                Name = userName,
                Password = userPassword,
            };

            var playerCollection = Dbh.GetCollection<Player>("Players");
            var newPlayer = new Player
            {
                Id = BsonObjectId.GenerateNewId().ToString(),
                Email = userEmail,
                Name = userName,
                Password = userPassword,
            };

            playerCollection.Save(newPlayer);
            userCollection.Save(newUser);
            Login(newUser);
            Events.SubmitEvent(EventType.PlayerCreate, newUser, newUser.Id);

            response.Data = GetSession(refresh: true);

            return Json(response);
        }

        [HttpPost]
        public ActionResult Edit(string email, string name, string oldPassword = "", string newPassword = "", bool Deactivated = false)
        {
            var response = new AjaxResponse { Success = false };
            var currentUser = (User)Session["User"];
            var user = DbHelper.GetUser(currentUser.Id);
            var newMd5Password = Md5.CalculateMd5(newPassword);
            var validation = new Validation();
            var player = Dbh.GetCollection<Player>("Players").FindOne(Query.EQ("Email", user.Email));

            if (user == null)
            {
                response.Message = "You have to be logged in to change user information";
                return Json(response);
            }

            if (!validation.ValidateEmail(email))
            {
                response.Message = "You must provide a valid email";
                return Json(response);
            }

            user.Email = string.IsNullOrEmpty(email) ? user.Email : email;
            user.Name = string.IsNullOrEmpty(name) ? user.Name : name;
            player.Email = user.Email;
            player.Name = user.Name;

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (Md5.CalculateMd5(oldPassword) == user.Password)
                {
                    user.Password = newMd5Password;
                }
                else
                {
                    response.Message = "The old password is not correct";
                    return Json(response);
                }
            }

            DbHelper.SaveUser(user);
            DbHelper.SavePlayer(player);

            response.Success = true;
            response.Message = "User updated succesfully";
            response.Data = GetSession(refresh: true);

            return Json(response);
        }
    }
}
