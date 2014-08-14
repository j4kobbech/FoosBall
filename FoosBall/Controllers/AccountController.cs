namespace FoosBall.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using ControllerHelpers;
    using Foosball.Main;
    using Main;
    using Models.Base;
    using Models.Domain;
    using MongoDB.Bson;
    using MongoDB.Driver.Builders;

    public class AccountController : BaseController
    {
        [HttpPost]
        public ActionResult Register(string email, string name, string password)
        {
            var userEmail = email.ToLower();
            var userName = name;
            var userPassword = Md5.CalculateMd5(password);
            var validation = new Validation();

            var ajaxResponse = validation.ValidateNewUserData(userEmail, userName, userPassword);
            if (!ajaxResponse.Success)
            {
                return Json(ajaxResponse);
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

            var activationLinkCollection = Dbh.GetCollection<ActivationLink>("ActivationLinks");
            var activationLink = new ActivationLink(newUser.Id);
            activationLinkCollection.Save(activationLink);
            playerCollection.Save(newPlayer);
            userCollection.Save(newUser);
            

            var accessToken = Main.Session.CreateNewAccessTokenOnUser(newUser);
            Main.Session.SaveAccessToken(accessToken);
            ajaxResponse.Data = new { AccessToken = Main.Session.BuildSessionInfo(accessToken, newUser) };

            return Json(ajaxResponse);
        }

        [HttpGet]
        [AuthorisationFilter(Role.User)]
        public ActionResult GetCurrentUserInformation()
        {
            return Json(Main.Session.GetCurrentUser(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorisationFilter(Role.User)]
        public ActionResult Edit(string email, string name, string oldPassword = "", string newPassword = "", bool Deactivated = false)
        {
            var response = new AjaxResponse();
            var currentUser = Main.Session.GetCurrentUser();
            var newMd5Password = Md5.CalculateMd5(newPassword);
            var validation = new Validation();
            var player = DbHelper.GetPlayerByEmail(email);

            if (currentUser == null)
            {
                response.Message = "You have to be logged in to change user information";
                return Json(response);
            }

            if (!validation.ValidateEmail(email))
            {
                response.Message = "You must provide a valid email";
                return Json(response);
            }

            currentUser.Email = string.IsNullOrEmpty(email) ? currentUser.Email : email;
            currentUser.Name = string.IsNullOrEmpty(name) ? currentUser.Name : name;
            player.Email = currentUser.Email;
            player.Name = currentUser.Name;

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (Md5.CalculateMd5(oldPassword) == currentUser.Password)
                {
                    currentUser.Password = newMd5Password;
                }
                else
                {
                    response.Message = "The old password is not correct";
                    return Json(response);
                }
            }

            DbHelper.SaveUser(currentUser);
            DbHelper.SavePlayer(player);
            response.Message = "User updated succesfully";

            return Json(response);
        }

        [HttpGet]
        public JsonResult Activate(string userId, string token)
        {
            var response = new AjaxResponse{Success = true};

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                response.Success = false;
                response.Message = "Missing userId or token";
            }

            var query = Query.And(Query.EQ("Token", token), Query.EQ("UserId", userId));
            var activationLinkCollection = Dbh.GetCollection<ActivationLink>("ActivationLinks");
            var activationLink = activationLinkCollection.Find(query).FirstOrDefault();

            if (activationLink != null)
            {
                if (activationLink.Expires < DateTime.Now)
                {
                    response.Success = false;
                    response.Message = "Expired";
                }

                response.Message = "Activation Successful";
                activationLinkCollection.Remove(query);
            }
            else
            {
                response.Success = false;
                response.Message = "Not found";
            }



            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}
