namespace FoosBall.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Foosball.Main;
    using Main;
    using Models.Base;
    using Models.Domain;
    using MongoDB.Bson;
    using MongoDB.Driver.Builders;

    public class AdminController : BaseController
    {
        [HttpGet]
        public JsonResult Config()
        {
            var currentUser = Main.Session.GetCurrentUser();

            if (currentUser != null && Settings.AdminAccounts.Contains(currentUser.Email))
            {
                return Json(Dbh.GetCollection<Config>("Config").FindOne(), JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorisationFilter(Role.Administrator)]
        public JsonResult Config(Config config)
        {
            var currentUser = Main.Session.GetCurrentUser();
            var configCollection = Dbh.GetCollection<Config>("Config");
            var currentConfig = configCollection.FindOne();
            var validation = new Validation();

            if (currentUser == null || !currentConfig.AdminAccounts.Contains(currentUser.Email))
            {
                return Json(new AjaxResponse { Success = false, Message = "You must be logged in as an Admin to make changes in the app configuration" });
            }

            var invalidEmail = config.AdminAccounts.FindAll(x => !validation.ValidateEmail(x));
            if (invalidEmail.Any())
            {
                return
                    Json(new AjaxResponse
                    {
                        Success = false,
                        Message = string.Format("One or more of the admin emails are invalid ({0})", string.Join(", ", invalidEmail))
                    });
            }

            var nonExistingEmails = config.AdminAccounts.FindAll(x => !validation.UserEmailExists(x));
            if (nonExistingEmails.Any())
            {
                return
                    Json(new AjaxResponse
                    {
                        Success = false,
                        Message = string.Format("One or more of the admin emails do not belong to a user ({0})", string.Join(", ", nonExistingEmails))
                    });
            }

            configCollection.Save(config);
            return Json(new AjaxResponse { Success = true, Message = "Configuration updated", Data = currentConfig });
        }
        
        [HttpPost]
        public void ReplayMatches()
        {
            var currentUser = Main.Session.GetCurrentUser();

            if (currentUser == null || !Settings.AdminAccounts.Contains(currentUser.Email))
            {
                return;
            }

            var allMatches = Dbh.GetCollection<Match>("Matches").FindAll().SetSortOrder(SortBy.Ascending("GameOverTime"));
            var allPlayers = Dbh.GetCollection<Player>("Players").FindAll();

            var copyMatches = Dbh.GetCollection<Match>("CopyMatches");
            var copyPlayers = Dbh.GetCollection<Player>("CopyPlayers");

            // Empty the Copies
            copyMatches.RemoveAll();
            copyPlayers.RemoveAll();

            // Reset all Players
            foreach (var player in allPlayers)
            {
                player.Lost = 0;
                player.Won = 0;
                player.Played = 0;
                player.Rating = 1000;

                copyPlayers.Save(player);
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // replay each match in chronological order
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            foreach (var match in allMatches)
            {
                // Update players from the match with players from the Db.
                match.RedPlayer1 = copyPlayers.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.RedPlayer1.Id)));
                    
                if (match.CountRedPlayers() == 2)
                {
                    match.RedPlayer2 = copyPlayers.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.RedPlayer2.Id)));
                }

                match.BluePlayer1 = copyPlayers.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.BluePlayer1.Id)));
                
                if (match.CountBluePlayers() == 2)
                {
                    match.BluePlayer2 = copyPlayers.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.BluePlayer2.Id)));
                }

                // Determine the winners and the losers
                var winners = new Team();
                var losers = new Team();

                if (match.RedScore > match.BlueScore)
                {
                    winners.Players.Add(match.RedPlayer1);
                    winners.Players.Add(match.RedPlayer2);
                    losers.Players.Add(match.BluePlayer1);
                    losers.Players.Add(match.BluePlayer2);
                }
                else
                {
                    winners.Players.Add(match.BluePlayer1);
                    winners.Players.Add(match.BluePlayer2);
                    losers.Players.Add(match.RedPlayer1);
                    losers.Players.Add(match.RedPlayer2);
                }

                // Get the rating modifier
                match.DistributedRating = Rating.GetRatingModifier(winners.GetTeamRating(), losers.GetTeamRating());

                // Propagate the rating and stats to the team members of both teams
                foreach (var member in winners.Players.Where(member => member.Id != null))
                {
                    member.Rating += match.DistributedRating;
                    member.Won++;
                    member.Played++;
                    copyPlayers.Save(member);
                }

                foreach (var member in losers.Players.Where(member => member.Id != null))
                {
                    member.Rating -= match.DistributedRating;
                    member.Lost++;
                    member.Played++;
                    copyPlayers.Save(member);
                }

                // Save the data to Db
                copyMatches.Save(match);
            }

            // Copy data into Production tables
            var matches = Dbh.GetCollection<Match>("Matches");
            var players = Dbh.GetCollection<Match>("Players");
            matches.RemoveAll();
            players.RemoveAll();

            foreach (var player in copyPlayers.FindAll())
            {
                players.Save(player);
            }

            foreach (var match in copyMatches.FindAll())
            {
                matches.Save(match);
            }
        }
        
        [HttpGet]
        public JsonResult GetUserEmails()
        {
            var allEmails = Dbh.GetCollection<User>("Users").FindAll().Select(x => x.Email).ToList();
            return Json(allEmails, JsonRequestBehavior.AllowGet);
        }
    }
}
