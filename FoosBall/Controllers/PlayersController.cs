namespace FoosBall.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using FoosBall.Models.Domain;
    using FoosBall.Models.ViewModels;

    using MongoDB.Driver.Builders;

    public class PlayersController : BaseController
    {
        public ActionResult Index()
        {
            var playerCollection = Dbh.GetCollection<Player>("Players")
                                        .FindAll()
                                        .SetSortOrder(SortBy.Descending("Rating"))
                                        .Where(x => x.Played > 0)
                                        .ToList();

            return this.View(new PlayersViewModel { AllPlayers = playerCollection });
        }
    }
}