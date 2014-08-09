namespace FoosBall.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using ControllerHelpers;
    using Models;
    using Models.Domain;
    using Models.ViewModels;
    using MongoDB.Bson;
    using MongoDB.Driver.Builders;

    public class StatsController : BaseController
    {
        public ActionResult GetStatistics()
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
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }

            var players = matches.Select(x => x.BluePlayer1).ToList();
            players.AddRange(matches.Select(x => x.BluePlayer2).ToList());
            players.AddRange(matches.Select(x => x.RedPlayer1).ToList());
            players.AddRange(matches.Select(x => x.RedPlayer2).ToList());

            viewModel.HighestRatingEver = StatsControllerHelpers.GetStatHighestRatingEver(players);
            viewModel.LowestRatingEver = StatsControllerHelpers.GetStatLowestRatingEver(players);

            var winningStreak = StatsControllerHelpers.GetLongestWinningStreak(matchesList);
            winningStreak.Player = DbHelper.GetPlayer(winningStreak.Player.Id);
            viewModel.LongestWinningStreak = winningStreak;

            var losingStreak = StatsControllerHelpers.GetLongestLosingStreak(matchesList);
            losingStreak.Player = DbHelper.GetPlayer(losingStreak.Player.Id);
            viewModel.LongestLosingStreak = losingStreak;
            viewModel.BiggestRatingWin = StatsControllerHelpers.GetBiggestRatingWin();

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlayerStatistics(string playerId)
        {
            if (playerId == null)
            {
                return new HttpStatusCodeResult(403, "Invalid request");
            }
            else
            {
                var playerCollection = Dbh.GetCollection<Player>("Players");
                var matches =
                    Dbh.GetCollection<Match>("Matches")
                        .FindAll()
                        .SetSortOrder(SortBy.Ascending("GameOverTime"))
                        .ToList()
                        .Where(match => match.ContainsPlayer(playerId))
                        .Where(match => match.GameOverTime != DateTime.MinValue);

                var playedMatches = matches as List<Match> ?? matches.ToList();
                var player = playerCollection.FindOne(Query.EQ("_id", BsonObjectId.Parse(playerId)));
                var stats = new PlayerStatsViewModel
                {
                    Player = player,
                    PlayedMatches = playedMatches.OrderByDescending(x => x.GameOverTime),
                    LatestMatch = playedMatches.Last()
                };

                if (playedMatches.Count == 0)
                {
                    return Json(stats, JsonRequestBehavior.AllowGet);
                }

                var bff = StatsControllerHelpers.GetBestFriendForever(playerId, playedMatches);
                var rbff = StatsControllerHelpers.GetRealBestFriendForever(playerId, playedMatches);
                var eae = StatsControllerHelpers.GetEvilArchEnemy(playerId, playedMatches);
                var preferredColor = StatsControllerHelpers.GetPreferredColor(playerId, playedMatches);
                var winningColor = StatsControllerHelpers.GetWinningColor(playerId, playedMatches);
                var highestRating = StatsControllerHelpers.GetHighestRating(playerId, playedMatches);
                var lowestRating = StatsControllerHelpers.GetLowestRating(playerId, playedMatches);
                var longestWinningStreak = StatsControllerHelpers.GetPlayersLongestWinningStreak(playerId, playedMatches);
                var longestLosingStreak = StatsControllerHelpers.GetPlayersLongestLosingStreak(playerId, playedMatches);
                    
                foreach (var match in playedMatches)
                {
                    stats.Played++;
                    stats.Won = match.WonTheMatch(playerId) ? ++stats.Won : stats.Won;
                    stats.Lost = !match.WonTheMatch(playerId) ? ++stats.Lost : stats.Lost;
                    stats.Ranking = playerCollection.FindAll()
                                                    .SetSortOrder(SortBy.Descending("Rating"))
                                                    .Where(x => x.Played > 0 && x.Deactivated == false)
                                                    .ToList()
                                                    .FindIndex(x => x.Id == playerId) + 1;

                    stats.TotalNumberOfPlayers = (int)playerCollection.Count(Query.GT("Played", 0));
                }
                    
                stats.Bff = bff.OrderByDescending(x => x.Value.Occurrences)
                                .Select(x => x.Value)
                                .FirstOrDefault();
                stats.Rbff = rbff.OrderByDescending(x => x.Value.Occurrences).Select(x => x.Value).FirstOrDefault();
                stats.Eae = eae.OrderByDescending(x => x.Value.Occurrences)
                                .ThenByDescending(x => x.Value.GoalDiff)
                                .Select(x => x.Value)
                                .FirstOrDefault();
                stats.PreferredColor = preferredColor.OrderByDescending(x => x.Value.Occurrences)
                                                        .Select(x => x.Value)
                                                        .FirstOrDefault();
                stats.WinningColor = winningColor.OrderByDescending(x => x.Value.Occurrences)
                                                    .Select(x => x.Value)
                                                    .FirstOrDefault();
                stats.HighestRating = highestRating;
                stats.LowestRating = lowestRating;
                stats.LongestWinningStreak = longestWinningStreak;
                stats.LongestLosingStreak = longestLosingStreak;

                return Json(stats, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetPlayerRatingData(string playerId)
        {
            var chartData = new PlayerRatingChartData();
            const int defaultRating = 1000;

            chartData.DataPoints.Add(new PlayerRatingChartDataPoint
                        {
                            TimeSet = new List<string>(),
                            Rating = defaultRating 
                        });

            if (playerId != null)
            {
                var playerCollection = Dbh.GetCollection<Player>("Players");
                var player = playerCollection.Find(Query.EQ("_id", BsonObjectId.Parse(playerId))).FirstOrDefault();
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

                    chartData.DataPoints.Add(new PlayerRatingChartDataPoint
                        {
                            TimeSet = time, 
                            Rating = matchPlayer.Rating
                        });
                }

                chartData.MinimumValue = minRating;
                chartData.MaximumValue = maxRating;
                chartData.Player = player;
            }
          
            return Json(chartData, JsonRequestBehavior.AllowGet);
        }
    }
}
