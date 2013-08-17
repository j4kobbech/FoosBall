namespace FoosBall.Models.ViewModels
{
    using System.Collections.Generic;

    using Domain;

    public class PlayerStatsViewModel
    {
        public Player Player { get; set; }

        public int Played { get; set; }

        public int Won { get; set; }

        public int Lost { get; set; }

        public int Ranking { get; set; }

        public BestFriendForever Bff { get; set; }

        public RealBestFriendForever Rbff { get; set; }

        public EvilArchEnemy Eae { get; set; }

        public PreferredColor PreferredColor { get; set; }

        public WinningColor WinningColor { get; set; }

        public double HighestRating { get; set; }

        public double LowestRating { get; set; }

        public int LongestWinningStreak { get; set; }

        public int LongestLosingStreak { get; set; }

        public Match LatestMatch { get; set; }

        public int TotalNumberOfPlayers { get; set; }

        public IEnumerable<Match> PlayedMatches { get; set; }
    }
}