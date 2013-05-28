namespace FoosBall.ControllerHelpers
{
    using System.Collections.Generic;
    using System.Linq;

    using FoosBall.Main;
    using FoosBall.Models.Custom;
    using FoosBall.Models.Domain;

    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public static class StatsControllerHelpers
    {
        private static readonly MongoDatabase Dbh = new Db(AppConfig.GetEnvironment()).Dbh;

        public static Player GetStatMostFights()
        {
            return Dbh.GetCollection<Player>("Players")
                    .FindAll()
                    .SetSortOrder(SortBy.Descending("Played"))
                    .FirstOrDefault();
        }

        public static Player GetStatMostWins()
        {
            return Dbh.GetCollection<Player>("Players")
                    .FindAll()
                    .SetSortOrder(SortBy.Descending("Won"))
                    .FirstOrDefault();
        }

        public static Player GetStatMostLosses()
        {
            return Dbh.GetCollection<Player>("Players")
                    .FindAll()
                    .SetSortOrder(SortBy.Descending("Lost"))
                    .FirstOrDefault();
        }

        public static Player GetStatTopRanked()
        {
            return Dbh.GetCollection<Player>("Players")
                    .FindAll()
                    .SetSortOrder(SortBy.Descending("Rating"))
                    .FirstOrDefault();
        }

        public static Player GetStatBottomRanked()
        {
            return Dbh.GetCollection<Player>("Players")
                    .FindAll()
                    .SetSortOrder(SortBy.Ascending("Rating"))
                    .FirstOrDefault(x => x.Played > 0);
        }

        public static Player GetStatHighestRatingEver(List<Player> players)
        {
            return players.OrderByDescending(m => m.Rating).FirstOrDefault();
        }

        public static Player GetStatLowestRatingEver(List<Player> players)
        {
            return players.OrderBy(m => m.Rating).FirstOrDefault();
        }

        public static Streak GetLongestWinningStreak(List<Match> matches)
        {
            var streaks = new Streaks();

            foreach (var match in matches)
            {
                if (match.WonTheMatch(match.RedPlayer1.Id))
                {
                    streaks.Add(match.RedPlayer1.Id);
                    streaks.Add(match.RedPlayer2.Id);

                    streaks.Reset(match.BluePlayer1.Id);
                    streaks.Reset(match.BluePlayer2.Id);
                }
                else
                {
                    streaks.Add(match.BluePlayer1.Id);
                    streaks.Add(match.BluePlayer2.Id);

                    streaks.Reset(match.RedPlayer1.Id);
                    streaks.Reset(match.RedPlayer2.Id);
                }
            }

            return streaks.GetLongestStreak();
        }

        public static Streak GetLongestLosingStreak(List<Match> matches)
        {
            var streaks = new Streaks();

            foreach (var match in matches)
            {
                if (!match.WonTheMatch(match.RedPlayer1.Id))
                {
                    streaks.Add(match.RedPlayer1.Id);
                    streaks.Add(match.RedPlayer2.Id);

                    streaks.Reset(match.BluePlayer1.Id);
                    streaks.Reset(match.BluePlayer2.Id);
                }
                else
                {
                    streaks.Add(match.BluePlayer1.Id);
                    streaks.Add(match.BluePlayer2.Id);

                    streaks.Reset(match.RedPlayer1.Id);
                    streaks.Reset(match.RedPlayer2.Id);
                }
            }

            return streaks.GetLongestStreak();
        }

        public static RatingDifference GetBiggestRatingWin()
        {
            var matches = Dbh.GetCollection<Match>("Matches").FindAll().SetSortOrder(SortBy.Ascending("GameOverTime"));
            var ratingDiff = new RatingDifference();
            
            foreach (var match in matches)
            {
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
                var ratingModifier = Rating.GetRatingModifier(winners.GetTeamRating(), losers.GetTeamRating());

                if (ratingModifier > ratingDiff.Rating)
                {
                    ratingDiff.Rating = ratingModifier;
                    ratingDiff.Match = match;
                }
            }

            return ratingDiff;
        }
    }
}