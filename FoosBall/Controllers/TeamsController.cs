namespace FoosBall.Controllers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Foosball.Main;
    using Models.Base;
    using Models.Domain;
    using MongoDB.Driver.Builders;
    using MongoDB.Bson;

    public class TeamsController : BaseController
    {
        [HttpGet]
        public ActionResult GetTeams()
        {
            //Fetch all FoosBall teamNames
            var teamNames = Dbh.GetCollection<TeamName>("TeamNames")
                                .FindAll()
                                .OrderByDescending(x => x.Name);

            return Json(teamNames, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTeamName(string name)
        {
            //Fetch all FoosBall teamNames
            var teamName = Dbh.GetCollection<TeamName>("TeamNames")
                    .FindOne(Query.EQ("Name", name));

            return Json(teamName, JsonRequestBehavior.AllowGet);
        }

        // POST: /Teams/CreateTeamName
        [HttpPost]
        [AuthorisationFilter(Role.User)]
        public ActionResult CreateTeamName(TeamName newTeamName)
        {
            var teamNameCollection = Dbh.GetCollection<TeamName>("TeamNames");
            var playerCollection = Dbh.GetCollection<Player>("Players");
            var player1 = playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Create(newTeamName.Team.Players[0].Id)));
            var player2 = playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Create(newTeamName.Team.Players[1].Id)));

            var teamName = new TeamName() {
                Name = newTeamName.Name,
                Team = new Team() {
                    Players = new List<Player>() { player1, player2 }
                }
            };

            teamNameCollection.Save(teamName);

            return Json(new { success = true, newTeam = teamName });
        }

    }
}
