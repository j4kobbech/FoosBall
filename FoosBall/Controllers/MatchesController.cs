namespace FoosBall.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Foosball.Main;
    using Main;
    using Models.Base;
    using Models.Domain;
    using MongoDB.Bson;
    using MongoDB.Driver.Builders;

    public class MatchesController : BaseController
    {       
        private const int PageSize = 30;
        
        [HttpGet]
        public ActionResult GetMatches(int numberOfMatches = PageSize, int startFromMatch = 0)
        {
            var fetchNumberOfMatches = numberOfMatches == 0 ? int.MaxValue : numberOfMatches;

            // Fetch all FoosBall matches
            var playedMatches = Dbh.GetCollection<Match>("Matches")
                    .Find(Query.GT("GameOverTime", DateTime.Parse("01/01/2012")))
                    .OrderByDescending(x => x.GameOverTime)
                    .Skip(startFromMatch)
                    .Take(fetchNumberOfMatches);

            return Json(playedMatches, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRating(double winnerRating, double loserRating)
        {
            var ratingModifier = Rating.GetRatingModifier(winnerRating, loserRating);
            var expectedScore = Rating.GetWinnerExpectedScore(winnerRating, loserRating);
            var result = new
            {
                RatingModifier = ratingModifier, 
                ExpectedScore = expectedScore,
                KModifier = Rating.KModifier
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /Matches/RegisterMatch
        [HttpPost]
        [AuthorisationFilter(Role.User)]
        public ActionResult SubmitMatch(Match newMatch)
        {
            newMatch.RedPlayer1 = newMatch.RedPlayer1 == null ? new Player() : new Player(newMatch.RedPlayer1.Id);
            newMatch.RedPlayer2 = newMatch.RedPlayer2 == null ? new Player() : new Player(newMatch.RedPlayer2.Id);
            newMatch.BluePlayer1 = newMatch.BluePlayer1 == null ? new Player() : new Player(newMatch.BluePlayer1.Id);
            newMatch.BluePlayer2 = newMatch.BluePlayer2 == null ? new Player() : new Player(newMatch.BluePlayer2.Id);

            var savedMatch = RegisterMatch(newMatch);
            return Json(new { success = true, returnedMatch = savedMatch });
        }

        // POST: /Matches/Delete/{id}
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var currentUser = Main.Session.GetCurrentUser();

            if (currentUser != null)
            {
                var matchCollection = Dbh.GetCollection<Match>("Matches");
                matchCollection.Remove(Query.EQ("_id", BsonObjectId.Parse(id)));
            }

            return new HttpStatusCodeResult(200);
        }

        // POST: /Matches/RegisterMatch
        private Match RegisterMatch(Match newMatch)
        {
            var currentUser = Main.Session.GetCurrentUser();
            var basicMatch = CreateMatch(currentUser, newMatch);
            var resolvedMatch = ResolveMatch(basicMatch);
            var matchCollection = Dbh.GetCollection<Match>("Matches");

            matchCollection.Save(resolvedMatch);
            return resolvedMatch;
        }

        // POST: /Matches/Create/{FormCollection}
        private Match CreateMatch(User user, Match newMatch)
        {
            Match match = null;

            if (user != null)
            {
                var red1 = newMatch.RedPlayer1.Id;
                var red2 = string.IsNullOrWhiteSpace(newMatch.RedPlayer2.Id) ? string.Empty : newMatch.RedPlayer2.Id;
                var blue1 = newMatch.BluePlayer1.Id;
                var blue2 = string.IsNullOrWhiteSpace(newMatch.BluePlayer2.Id) ? string.Empty : newMatch.BluePlayer2.Id;

                // only try to create a match if properties are set correctly
                if (!string.IsNullOrEmpty(red1) && !string.IsNullOrEmpty(blue1))
                {
                    var playerCollection = Dbh.GetCollection<Player>("Players");
                    var redPlayer1 = string.IsNullOrEmpty(red1)
                                         ? new Player()
                                         : playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Create(red1)));
                    var redPlayer2 = string.IsNullOrEmpty(red2)
                                         ? new Player()
                                         : playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Create(red2)));
                    var bluePlayer1 = string.IsNullOrEmpty(blue1)
                                          ? new Player()
                                          : playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Create(blue1)));
                    var bluePlayer2 = string.IsNullOrEmpty(blue2)
                                          ? new Player()
                                          : playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Create(blue2)));

                    match = new Match
                                    {
                                        RedPlayer1 = redPlayer1,
                                        RedPlayer2 = redPlayer2,
                                        BluePlayer1 = bluePlayer1,
                                        BluePlayer2 = bluePlayer2,
                                        RedScore = newMatch.RedScore,
                                        BlueScore = newMatch.BlueScore,
                                        CreationTime = DateTime.Now,
                                        GameOverTime = DateTime.MinValue,
                                        CreatedBy = user.Id,
                                    };
                }
            }

            return match;
        }

        // POST: /Matches/SaveMatchResult/{FormCollection}
        [HttpPost]
        private Match ResolveMatch(Match match)
        {
            var redScore = match.RedScore;
            var blueScore = match.BlueScore;
            
            if (redScore != blueScore)
            {
                var playerCollection = Dbh.GetCollection<Player>("Players");

                // Update players from the match with players from the Db.
                if (match.RedPlayer1.Id != null)
                {
                    match.RedPlayer1 = 
                        playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.RedPlayer1.Id)));
                }

                if (match.RedPlayer2.Id != null)
                {
                    match.RedPlayer2 = 
                        playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.RedPlayer2.Id)));
                }
                
                if (match.BluePlayer1.Id != null)
                {
                    match.BluePlayer1 = 
                        playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.BluePlayer1.Id)));
                }
                
                if (match.BluePlayer2.Id != null)
                {
                    match.BluePlayer2 = 
                        playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.BluePlayer2.Id)));
                }

                var currentUser = Main.Session.GetCurrentUser();
                if (currentUser != null)
                {
                    // Get the scores
                    match.RedScore = redScore;
                    match.BlueScore = blueScore;

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
                        playerCollection.Save(member);
                    }

                    foreach (var member in losers.Players.Where(member => member.Id != null))
                    {
                        member.Rating -= match.DistributedRating;
                        member.Lost++;
                        member.Played++;
                        playerCollection.Save(member);
                    }

                    // Update match time stats
                    match.GameOverTime = DateTime.Now;
                }
            }
            
            return match;
        }
    }
}
