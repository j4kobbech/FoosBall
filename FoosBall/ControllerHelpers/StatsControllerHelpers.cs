namespace FoosBall.ControllerHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Main;
    using Models;
    using Models.Custom;
    using Models.Domain;
    using Models.ViewModels;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public static class StatsControllerHelpers
    {
        private const string Blue = "blue";
        private const string Red = "red";
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
                    .FirstOrDefault(x => !x.Deactivated);
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
            return players.OrderByDescending(x => x.Rating).FirstOrDefault();
        }

        public static Player GetStatLowestRatingEver(List<Player> players)
        {
            return players.OrderBy(x => x.Rating).FirstOrDefault();
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

        public static int GetPlayersLongestWinningStreak(string playerId, List<Match> matches)
        {
            var streaks = new Streaks();

            foreach (var match in matches.Where(x => x.ContainsPlayer(playerId)))
            {
                if (match.WonTheMatch(playerId))
                {
                    streaks.Add(playerId);
                }
                else
                {
                    streaks.Reset(playerId);
                }
            }

            return streaks.GetLongestStreak().Count;
        }

        public static int GetPlayersLongestLosingStreak(string playerId, List<Match> matches)
        {
            var streaks = new Streaks();

            foreach (var match in matches.Where(x => x.ContainsPlayer(playerId)))
            {
                if (!match.WonTheMatch(playerId))
                {
                    streaks.Add(playerId);
                }
                else
                {
                    streaks.Reset(playerId);
                }
            }

            return streaks.GetLongestStreak().Count;
        }

        public static StatsAggregateViewModel.BiggestRatingWinModel GetBiggestRatingWin()
        {
            var matches = Dbh.GetCollection<Match>("Matches").FindAll();
            var biggestRatingWin = new StatsAggregateViewModel.BiggestRatingWinModel();
            
            foreach (var match in matches)
            {
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
                var ratingModifier = Rating.GetRatingModifier(winners.GetTeamRating(), losers.GetTeamRating());

                if (ratingModifier > biggestRatingWin.Rating)
                {
                    biggestRatingWin.Rating = ratingModifier;
                    biggestRatingWin.WinningTeam = winners;
                    biggestRatingWin.LosingTeam = losers;
                }
            }

            return biggestRatingWin;
        }

        public static Dictionary<string, BestFriendForever> GetBestFriendForever(string playerId, List<Match> matches)
        {
            var bff = new Dictionary<string, BestFriendForever>();

            foreach (var teamMate in matches.Select(match => match.GetTeamMate(playerId)).Where(teamMate => teamMate.Id != null))
            {
                if (bff.ContainsKey(teamMate.Id))
                {
                    bff[teamMate.Id].Occurrences++;
                }
                else
                {
                    bff.Add(teamMate.Id, new BestFriendForever { Player = teamMate });
                }
            }

            return bff;
        }

        public static Dictionary<string, RealBestFriendForever> GetRealBestFriendForever(string playerId, List<Match> matches)
        {
            var rbff = new Dictionary<string, RealBestFriendForever>();

            foreach (var teamMate in from match in matches let teamMate = match.GetTeamMate(playerId) where teamMate.Id != null && match.WonTheMatch(playerId) select teamMate)
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

            return rbff;
        }

        public static Dictionary<string, EvilArchEnemy> GetEvilArchEnemy(string playerId, List<Match> matches)
        {
            var eae = new Dictionary<string, EvilArchEnemy>();

            foreach (var match in matches.Where(match => !match.WonTheMatch(playerId)))
            {
                var goalDiff = match.GetWinnerScore() - match.GetLoserScore();

                foreach (var opponent in match.GetWinners())
                {
                    if (eae.ContainsKey(opponent.Id))
                    {
                        eae[opponent.Id].Occurrences++;
                        eae[opponent.Id].GoalDiff += goalDiff;
                    }
                    else
                    {
                        eae.Add(
                            opponent.Id,
                            new EvilArchEnemy { Player = opponent, GoalDiff = goalDiff });
                    }
                }
            }

            return eae;
        }

        public static Dictionary<string, PreferredColor> GetPreferredColor(string playerId, List<Match> matches)
        {
            var preferredColor = new Dictionary<string, PreferredColor>();

            foreach (var match in matches)
            {
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
                }
            }

            return preferredColor;
        }

        public static Dictionary<string, WinningColor> GetWinningColor(string playerId, List<Match> matches)
        {
            var winningColor = new Dictionary<string, WinningColor>();

            foreach (var match in matches.Where(match => match.WonTheMatch(playerId)))
            {
                if (match.IsOnRedTeam(playerId))
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
                    if (winningColor.ContainsKey(Blue))
                    {
                        winningColor[Blue].Occurrences++;
                    }
                    else
                    {
                        winningColor.Add(Blue, new WinningColor { Color = Blue, Occurrences = 1 });
                    }
                }
            }

            return winningColor;
        }

        public static double GetHighestRating(string playerId, List<Match> matches)
        {
            return matches.Where(x => x.ContainsPlayer(playerId)).Select(x => x.GetPlayer(playerId)).Max(x => x.Rating);
        }

        public static double GetLowestRating(string playerId, List<Match> matches)
        {
            return Math.Min(1000, matches.Where(x => x.ContainsPlayer(playerId)).Select(x => x.GetPlayer(playerId)).Min(x => x.Rating));
        }
    }
}
