namespace FoosBall.Models.ViewModels
{
    using System.Collections.Generic;
    using Custom;
    using Domain;

    public class StatsAggregateViewModel
    {
        public long TotalNumberOfPlayedMatches { get; set; }

        public Player MostFights { get; set; }

        public Player MostWins { get; set; }

        public Player MostLosses { get; set; }

        public Player TopRanked { get; set; }

        public Player BottomRanked { get; set; }

        public Player HighestRatingEver { get; set; }

        public Player LowestRatingEver { get; set; }

        public Streak LongestWinningStreak { get; set; }

        public Streak LongestLosingStreak { get; set; }
        
        public BiggestRatingWinModel BiggestRatingWin { get; set; }

        public class BiggestRatingWinModel
        {
            public BiggestRatingWinModel()
            {
                this.WinningTeam = new Team();
                this.LosingTeam = new Team();
            }

            public Team WinningTeam { get; set; }

            public Team LosingTeam { get; set; }

            public double Rating { get; set; }
        }
    }
}
