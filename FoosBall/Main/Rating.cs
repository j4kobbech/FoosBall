namespace FoosBall.Main
{
    using System;

    public static class Rating
    {
        public const int KModifier = 35;

        public static double GetRatingModifier(double winnerRating, double loserRating)
        {
            var winnerExpectedScore = GetWinnerExpectedScore(winnerRating, loserRating);
            return KModifier * (1 - winnerExpectedScore);
        }

        public static double GetWinnerExpectedScore(double winnerRating, double loserRating)
        {
            return 1 / (1 + Math.Pow(10, (loserRating - winnerRating) / 400));
        }
    }
}