namespace FoosBall.Models.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    
    public class Team
    {
        public Team()
        {
            this.Players = new List<Player>();
        }
        
        public List<Player> Players { get; set; }
        
        public double GetTeamRating()
        {
            return this.Players.Where(player => player.Id != null).Sum(player => player.Rating);
        }
    }
}