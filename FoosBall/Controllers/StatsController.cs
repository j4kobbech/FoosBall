namespace FoosBall.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    using FoosBall.ControllerHelpers;
    using FoosBall.Models;
    using FoosBall.Models.Domain;
    using FoosBall.Models.ViewModels;

    using MongoDB.Bson;
    using MongoDB.Driver.Builders;

    using StackExchange.Profiling;

    public class StatsController : BaseController
    {
        protected readonly MiniProfiler Profiler = MiniProfiler.Current;

        public ActionResult Index()
        {
            var viewModel = new StatsAggregateViewModel
                {
                    MostFights = StatsControllerHelpers.GetStatMostFights(),
                    MostWins = StatsControllerHelpers.GetStatMostWins(),
                    MostLosses = StatsControllerHelpers.GetStatMostLosses(),
                    TopRanked = StatsControllerHelpers.GetStatTopRanked(),
                    BottomRanked = StatsControllerHelpers.GetStatBottomRanked()
                };

            var matches = Dbh.GetCollection<Match>("Matches").FindAll();
            var matchesList = matches.SetSortOrder(SortBy.Ascending("GameOverTime")).ToList();

            viewModel.TotalNumberOfPlayedMatches = matches.Count();

            if (viewModel.TotalNumberOfPlayedMatches == 0)
            {
                return View(viewModel);
            }

            var players = matches.Select(m => m.BluePlayer1).ToList();
            players.AddRange(matches.Select(m => m.BluePlayer2).ToList());
            players.AddRange(matches.Select(m => m.RedPlayer1).ToList());
            players.AddRange(matches.Select(m => m.RedPlayer2).ToList());

            viewModel.HighestRatingEver = StatsControllerHelpers.GetStatHighestRatingEver(players);
            viewModel.LowestRatingEver = StatsControllerHelpers.GetStatLowestRatingEver(players);

            var winningStreak = StatsControllerHelpers.GetLongestWinningStreak(matchesList);
            winningStreak.Player = DbHelper.GetPlayer(winningStreak.Player.Id);
            viewModel.LongestWinningStreak = winningStreak;

            var losingStreak = StatsControllerHelpers.GetLongestLosingStreak(matchesList);
            losingStreak.Player = DbHelper.GetPlayer(losingStreak.Player.Id);
            viewModel.LongestLosingStreak = losingStreak;

            using (Profiler.Step("Calculating BiggestRatingWin"))
            {
                viewModel.BiggestRatingWin = StatsControllerHelpers.GetBiggestRatingWin();
            }

            return this.View(viewModel);
        }

        public ActionResult Player(string playerId)
        {
            using (Profiler.Step("Calculating Player Statistics"))
            {
                if (playerId != null)
                {
                    var bff = new Dictionary<string, BestFriendForever>();
                    var rbff = new Dictionary<string, RealBestFriendForever>();
                    var eae = new Dictionary<string, EvilArchEnemy>();
                    var preferredColor = new Dictionary<string, PreferredColor>();
                    var winningColor = new Dictionary<string, WinningColor>();
                    const string Blue = "blue";
                    const string Red = "red";

                    var playerCollection = Dbh.GetCollection<Player>("Players");
                    var player = playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Parse(playerId)));
                    var stats = new PlayerStatsViewModel { Player = player };

                    var matches =
                        Dbh.GetCollection<Match>("Matches")
                           .FindAll()
                           .SetSortOrder(SortBy.Ascending("GameOverTime"))
                           .ToList()
                           .Where(match => match.ContainsPlayer(playerId))
                           .Where(match => match.GameOverTime != DateTime.MinValue);

                    var playedMatches = matches as List<Match> ?? matches.ToList();

                    if (playedMatches.Count == 0)
                    {
                        return View(stats);
                    }

                    stats.PlayedMatches = playedMatches.OrderByDescending(x => x.GameOverTime);
                    stats.LatestMatch = playedMatches.Last();

                    foreach (var match in playedMatches)
                    {
                        var teamMate = match.GetTeamMate(playerId);
                        if (teamMate.Id != null)
                        {
                            if (bff.ContainsKey(teamMate.Id))
                            {
                                bff[teamMate.Id].Occurrences++;
                            }
                            else
                            {
                                bff.Add(teamMate.Id, new BestFriendForever { Player = teamMate });
                            }

                            if (match.WonTheMatch(playerId))
                            {
                                if (rbff.ContainsKey(teamMate.Id))
                                {
                                    rbff[teamMate.Id].Occurrences++;
                                }
                                else
                                {
                                    rbff.Add(teamMate.Id, new RealBestFriendForever { Player = teamMate });
                                }
                            }
                        }

                        if (match.IsOnRedTeam(playerId))
                        {
                            if (preferredColor.ContainsKey(Red))
                            {
                                preferredColor[Red].Occurrences++;
                            }
                            else
                            {
                                preferredColor.Add(Red, new PreferredColor { Color = Red, Occurrences = 1 });
                            }

                            if (match.WonTheMatch(playerId))
                            {
                                if (winningColor.ContainsKey(Red))
                                {
                                    winningColor[Red].Occurrences++;
                                }
                                else
                                {
                                    winningColor.Add(Red, new WinningColor { Color = Red, Occurrences = 1 });
                                }
                            }
                            else
                            {
                                var goalDiff = match.BlueScore - match.RedScore;
                                if (eae.ContainsKey(match.BluePlayer1.Id))
                                {
                                    eae[match.BluePlayer1.Id].Occurrences++;
                                    eae[match.BluePlayer1.Id].GoalDiff += goalDiff;
                                }
                                else
                                {
                                    eae.Add(
                                        match.BluePlayer1.Id,
                                        new EvilArchEnemy { Player = match.BluePlayer1, GoalDiff = goalDiff });
                                }

                                if (match.BluePlayer2.Id != null)
                                {
                                    if (eae.ContainsKey(match.BluePlayer2.Id))
                                    {
                                        eae[match.BluePlayer2.Id].Occurrences++;
                                        eae[match.BluePlayer2.Id].GoalDiff += goalDiff;
                                    }
                                    else
                                    {
                                        eae.Add(
                                            match.BluePlayer2.Id,
                                            new EvilArchEnemy { Player = match.BluePlayer2, GoalDiff = goalDiff });
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (preferredColor.ContainsKey(Blue))
                            {
                                preferredColor[Blue].Occurrences++;
                            }
                            else
                            {
                                preferredColor.Add(Blue, new PreferredColor { Color = Blue, Occurrences = 1 });
                            }

                            if (match.WonTheMatch(playerId))
                            {
                                if (winningColor.ContainsKey(Blue))
                                {
                                    winningColor[Blue].Occurrences++;
                                }
                                else
                                {
                                    winningColor.Add(Blue, new WinningColor { Color = Blue, Occurrences = 1 });
                                }
                            }
                            else
                            {
                                var goalDiff = match.BlueScore - match.RedScore;
                                if (eae.ContainsKey(match.RedPlayer1.Id))
                                {
                                    eae[match.RedPlayer1.Id].Occurrences++;
                                    eae[match.RedPlayer1.Id].GoalDiff = goalDiff;
                                }
                                else
                                {
                                    eae.Add(
                                        match.RedPlayer1.Id,
                                        new EvilArchEnemy { Player = match.RedPlayer1, GoalDiff = goalDiff });
                                }

                                if (match.RedPlayer2.Id != null)
                                {
                                    if (eae.ContainsKey(match.RedPlayer2.Id))
                                    {
                                        eae[match.RedPlayer2.Id].Occurrences++;
                                        eae[match.RedPlayer2.Id].GoalDiff = goalDiff;
                                    }
                                    else
                                    {
                                        eae.Add(
                                            match.RedPlayer2.Id,
                                            new EvilArchEnemy { Player = match.RedPlayer2, GoalDiff = goalDiff });
                                    }
                                }
                            }
                        }

                        stats.Played++;
                        stats.Won = match.WonTheMatch(playerId) ? ++stats.Won : stats.Won;
                        stats.Lost = !match.WonTheMatch(playerId) ? ++stats.Lost : stats.Lost;
                        stats.PlayedToday = match.GameOverTime.ToLocalTime().Day == DateTime.Now.Day
                                                ? ++stats.PlayedToday
                                                : stats.PlayedToday;
                        stats.PlayedLast7Days = match.GameOverTime.ToLocalTime() > DateTime.Now.AddDays(-7)
                                                    ? ++stats.PlayedLast7Days
                                                    : stats.PlayedLast7Days;
                        stats.PlayedLast30Days = match.GameOverTime.ToLocalTime() > DateTime.Now.AddDays(-30)
                                                     ? ++stats.PlayedLast30Days
                                                     : stats.PlayedLast30Days;
                        stats.Ranking = playerCollection.FindAll()
                                            .SetSortOrder(SortBy.Descending("Rating"))
                                            .ToList()
                                            .FindIndex(x => x.Id == playerId) + 1; // convert zero-based to 1-based index

                        stats.TotalNumberOfPlayers = (int)playerCollection.Count();
                    }

                    stats.Bff = bff.OrderByDescending(i => i.Value.Occurrences).Select(i => i.Value).FirstOrDefault();
                    stats.Rbff = rbff.OrderByDescending(i => i.Value.Occurrences).Select(i => i.Value).FirstOrDefault();
                    stats.Eae =
                        eae.OrderByDescending(i => i.Value.Occurrences)
                           .ThenByDescending(i => i.Value.GoalDiff)
                           .Select(i => i.Value)
                           .FirstOrDefault();
                    stats.PreferredColor =
                        preferredColor.OrderByDescending(i => i.Value.Occurrences).Select(i => i.Value).FirstOrDefault();
                    stats.WinningColor =
                        winningColor.OrderByDescending(i => i.Value.Occurrences).Select(i => i.Value).FirstOrDefault();

                    return View(stats);
                }
            }

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public JsonResult GetPlayerRatingData(string playerId)
        {
            var chartData = new PlayerRatingChartData(); 

            if (playerId != null)
            {
                var matches = Dbh.GetCollection<Match>("Matches")
                    .FindAll()
                    .SetSortOrder(SortBy.Ascending("GameOverTime"))
                    .ToList()
                    .Where(match => match.ContainsPlayer(playerId))
                    .Where(match => match.GameOverTime != DateTime.MinValue);

                var playedMatches = matches as List<Match> ?? matches.ToList();
                double minRating = 10000;
                double maxRating = 0;

                foreach (var match in playedMatches)
                {
                    var matchPlayer = match.GetPlayer(playerId);
                    minRating = (minRating > matchPlayer.Rating) ? matchPlayer.Rating : minRating;
                    maxRating = (maxRating < matchPlayer.Rating) ? matchPlayer.Rating : maxRating;

                    var time = new List<string>
                        {
                            match.GameOverTime.ToLocalTime().Year.ToString(CultureInfo.InvariantCulture),
                            match.GameOverTime.ToLocalTime().Month.ToString(CultureInfo.InvariantCulture),
                            match.GameOverTime.ToLocalTime().Day.ToString(CultureInfo.InvariantCulture),
                            match.GameOverTime.ToLocalTime().Hour.ToString(CultureInfo.InvariantCulture),
                            match.GameOverTime.ToLocalTime().Minute.ToString(CultureInfo.InvariantCulture),
                            match.GameOverTime.ToLocalTime().Second.ToString(CultureInfo.InvariantCulture)
                        };

                    chartData.DataPoints.Add(new PlayerRatingChartDataPoint { TimeSet = time, Rating = matchPlayer.Rating });
                }

                chartData.MinimumValue = minRating;
                chartData.MaximumValue = maxRating;
            }
          
            return Json(chartData, JsonRequestBehavior.AllowGet);
        }
    }
}
