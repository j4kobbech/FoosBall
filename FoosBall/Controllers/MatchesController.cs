namespace FoosBall.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    using FoosBall.Main;
    using FoosBall.Models.Custom;
    using FoosBall.Models.Domain;
    using FoosBall.Models.ViewModels;
    using Models.Base;
    using MongoDB.Bson;
    using MongoDB.Driver.Builders;

    public class MatchesController : BaseController
    {       
        private const int PageSize = 30;
        
        // GET: /Matches/
        public ActionResult Index()
        {

            // Fetch all players to display in a <select>
            var playerCollection = this.Dbh.GetCollection<Player>("Players")
                                           .FindAll()
                                           .SetSortOrder(SortBy.Ascending("Name"))
                                           .ToList();
            
            // Fetch all FoosBall matches
            var playedMatches = 
                this.Dbh.GetCollection<Match>("Matches")
                    .Find(Query.NE("GameOverTime", BsonDateTime.Create(DateTime.MinValue)))
                    .ToList()
                    .OrderByDescending(x => x.GameOverTime)
                    .Take(PageSize);

            // Create content for the <select> 
            var selectItems = playerCollection
                .Select(x => new CustomSelectListItem
                    {
                        Selected = false, 
                        Text = x.Name, 
                        Value = x.Id, 
                        CssClass = x.Gender
                    })
                .ToList();

            var played = playedMatches.OrderByDescending(x => x.GameOverTime);

            return View(new MatchesViewModel
                            {
                                PlayedMatches = played, 
                                SelectPlayers = selectItems,
                                Settings = this.Settings
                            });
        }



        // POST: /Matches/RegisterMatch
        [HttpPost]
        public ActionResult RegisterMatch(FormCollection formCollection)
        {
            var currentUser = (Player)Session["User"];
            var unresolvedMatch = CreateMatch(currentUser, formCollection);
            var resolvedMatch = SaveMatchResult(unresolvedMatch, formCollection);
            var matchCollection = this.Dbh.GetCollection<Match>("Matches");

            matchCollection.Save(resolvedMatch);
            Events.SubmitEvent(EventType.MatchResolve, resolvedMatch, currentUser.Id);

            return this.RedirectToAction("Index");
        }

        // POST: /Matches/Delete/{id}
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var currentUser = (Player)Session["User"];

            if (currentUser != null)
            {
                var matchCollection = this.Dbh.GetCollection<Match>("Matches");
                var query = Query.EQ("_id", BsonObjectId.Parse(id));
                var match = matchCollection.FindOne(query);

                Events.SubmitEvent(EventType.MatchDelete, match, currentUser.Id);
                matchCollection.Remove(query);
            }

            return RedirectToAction("Index");
        }

        // POST: /Matches/Create/{FormCollection}
        private Match CreateMatch(Player user, FormCollection formValues)
        {
            Match newMatch = null;

            if (user != null)
            {
                var r1 = formValues.GetValue("red-player-1").AttemptedValue;
                var r2 = formValues.GetValue("red-player-2").AttemptedValue;
                var b1 = formValues.GetValue("blue-player-1").AttemptedValue;
                var b2 = formValues.GetValue("blue-player-2").AttemptedValue;

                // only try to create a match if properties are set correctly
                if (!string.IsNullOrEmpty(r1) && !string.IsNullOrEmpty(b1))
                {
                    // var matchCollection = this.Dbh.GetCollection<Match>("Matches");
                    var playerCollection = this.Dbh.GetCollection<Player>("Players");
                    var redPlayer1 = string.IsNullOrEmpty(r1)
                                         ? new Player()
                                         : playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Create(r1)));
                    var redPlayer2 = string.IsNullOrEmpty(r2)
                                         ? new Player()
                                         : playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Create(r2)));
                    var bluePlayer1 = string.IsNullOrEmpty(b1)
                                          ? new Player()
                                          : playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Create(b1)));
                    var bluePlayer2 = string.IsNullOrEmpty(b2)
                                          ? new Player()
                                          : playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Create(b2)));

                    newMatch = new Match
                                    {
                                        RedPlayer1 = redPlayer1,
                                        RedPlayer2 = redPlayer2,
                                        BluePlayer1 = bluePlayer1,
                                        BluePlayer2 = bluePlayer2,
                                        CreationTime = new BsonDateTime(DateTime.Now),
                                        GameOverTime = new BsonDateTime(DateTime.MinValue),
                                        Created = new BsonDateTime(DateTime.Now),
                                        CreatedBy = user.Id
                                    };
                }
            }

            return newMatch;
        }

        // POST: /Matches/SaveMatchResult/{FormCollection}
        [HttpPost]
        private Match SaveMatchResult(Match match, FormCollection form)
        {
            var redScore = form.GetValue("team-red-score").AttemptedValue;
            var blueScore = form.GetValue("team-blue-score").AttemptedValue;
            
            if (string.IsNullOrEmpty(redScore) == false && string.IsNullOrEmpty(blueScore) == false)
            {
                var playerCollection = this.Dbh.GetCollection<Player>("Players");

                // Update players from the match with players from the Db.
                if (match.RedPlayer1.Id != null)
                {
                    match.RedPlayer1 = playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.RedPlayer1.Id)));
                }

                if (match.RedPlayer2.Id != null)
                {
                    match.RedPlayer2 = playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.RedPlayer2.Id)));
                }
                
                if (match.BluePlayer1.Id != null)
                {
                    match.BluePlayer1 = playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.BluePlayer1.Id)));
                }
                
                if (match.BluePlayer2.Id != null)
                {
                    match.BluePlayer2 = playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Parse(match.BluePlayer2.Id)));
                }

                var currentUser = (Player)Session["User"];
                if (currentUser != null)
                {
                    // Get the scores
                    var intRedScore = int.Parse(redScore, NumberStyles.Float);
                    var intBlueScore = int.Parse(blueScore, NumberStyles.Float);
                    match.RedScore = intRedScore;
                    match.BlueScore = intBlueScore;

                    // Determine the winners and the losers
                    var winners = new Team();
                    var losers = new Team();

                    if (match.RedScore > match.BlueScore)
                    {
                        winners.MatchTeam.Add(match.RedPlayer1);
                        winners.MatchTeam.Add(match.RedPlayer2);
                        losers.MatchTeam.Add(match.BluePlayer1);
                        losers.MatchTeam.Add(match.BluePlayer2);
                    }
                    else
                    {
                        winners.MatchTeam.Add(match.BluePlayer1);
                        winners.MatchTeam.Add(match.BluePlayer2);
                        losers.MatchTeam.Add(match.RedPlayer1);
                        losers.MatchTeam.Add(match.RedPlayer2);
                    }

                    // Get the rating modifier
                    match.DistributedRating = Rating.GetRatingModifier(winners.GetTeamRating(), losers.GetTeamRating());

                    // Propagate the rating and stats to the team members of both teams
                    foreach (var member in winners.MatchTeam.Where(member => member.Id != null))
                    {
                        member.Rating += match.DistributedRating;
                        member.Won++;
                        member.Played++;
                        playerCollection.Save(member);
                    }

                    foreach (var member in losers.MatchTeam.Where(member => member.Id != null))
                    {
                        member.Rating -= match.DistributedRating;
                        member.Lost++;
                        member.Played++;
                        playerCollection.Save(member);
                    }

                    // Update match time stats
                    match.GameOverTime = new BsonDateTime(DateTime.Now);
                }
            }
            
            return match;
        }
    }
}