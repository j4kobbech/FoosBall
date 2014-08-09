namespace FoosBall.Models.Domain
{
    using Base;
    using Main;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Player : FoosBallDoc
    {
        public Player()
        {
            Rating = 1000;
            Won = 0;
            Lost = 0;
            Played = 0;
        }

        public Player(string id)
        {
            Id = id;
            Rating = 1000;
            Won = 0;
            Lost = 0;
            Played = 0;
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool Deactivated { get; set; }

        public string StatsUrl
        {
            get
            {
                return string.Format("/#/playerstats?playerId={0}", Id);
            }
        }

        public string GravatarUrl
        {
            get
            {
                return !string.IsNullOrEmpty(Email) ? Md5.GetGravatarUrl(Email) : string.Empty;
            }
        }

        public int Won { get; set; }
        
        public int Lost { get; set; }
        
        public int Played { get; set; }

        public double Rating { get; set; }

        public double Ratio
        {
            get
            {
                double ratio;

                if (this.Played == 0)
                {
                    ratio = 0;
                }
                else
                {
                    ratio = (this.Won / (double)this.Played) * 100;
                }

                return ratio;                
            }
        }
    }
}
