namespace FoosBall.Models.Domain
{
    using System;
    using System.Collections.Generic;
    using Base;
    using MongoDB.Bson;

    public class Match : FoosBallDoc
    {
        public int RedScore { get; set; }

        public int BlueScore { get; set; }

        public double DistributedRating { get; set; }
        
        public Player RedPlayer1 { get; set; }
        
        public Player RedPlayer2 { get; set; }
        
        public Player BluePlayer1 { get; set; }
        
        public Player BluePlayer2 { get; set; }
        
        public BsonDateTime CreationTime { get; set; }
        
        public BsonDateTime GameOverTime { get; set; }

        public bool WonTheMatch(string id)
        {
            return (IsOnRedTeam(id) && RedScore > BlueScore) || (IsOnBlueTeam(id) && BlueScore > RedScore);
        }

        public bool ContainsPlayer(string id)
        {
            return id == RedPlayer1.Id || id == RedPlayer2.Id || id == BluePlayer1.Id || id == BluePlayer2.Id;
        }

        public bool IsOnRedTeam(string id)
        {
            return id == RedPlayer1.Id || id == RedPlayer2.Id;
        }

        public bool IsOnBlueTeam(string id)
        {
            return id == BluePlayer1.Id || id == BluePlayer2.Id;
        }

        public Player GetTeamMate(string id)
        {
            if (id == RedPlayer1.Id)
            {
                return RedPlayer2;
            }

            if (id == RedPlayer2.Id)
            {
                return RedPlayer1;
            }
            
            if (id == BluePlayer1.Id)
            {
                return BluePlayer2;
            }
            
            if (id == BluePlayer2.Id)
            {
                return BluePlayer1;
            }
            
            return null;
        }

        public int CountRedPlayers()
        {
            return (RedPlayer2.Id == null) ? 1 : 2;
        }
        
        public int CountBluePlayers()
        {
            return (BluePlayer2.Id == null) ? 1 : 2;
        }
        
        public double GetRedTeamRating()
        {
            return (RedPlayer2.Id == null) ? RedPlayer1.Rating : (RedPlayer1.Rating + RedPlayer2.Rating);
        }

        public double GetBlueTeamRating()
        {
            return (BluePlayer2.Id == null) ? BluePlayer1.Rating : (BluePlayer1.Rating + BluePlayer2.Rating);
        }

        public Player GetPlayer(string id)
        {
            if (id == RedPlayer1.Id)
            {
                return RedPlayer1;
            }

            if (id == RedPlayer2.Id)
            {
                return RedPlayer2;
            }

            if (id == BluePlayer1.Id)
            {
                return BluePlayer1;
            }

            return id == this.BluePlayer2.Id ? this.BluePlayer2 : null;
        }

        public List<Player> GetPlayers()
        {
            var listOfPlayers = new List<Player> { RedPlayer1, BluePlayer1 };

            if (CountRedPlayers() == 2)
            {
                listOfPlayers.Add(this.RedPlayer2);
            }

            if (CountBluePlayers() == 2)
            {
                listOfPlayers.Add(this.BluePlayer2);
            }

            return listOfPlayers;
        }

        public List<Player> GetWinners()
        {
            var listOfWinners = new List<Player>();

            if (this.WonTheMatch(this.RedPlayer1.Id))
            {
                listOfWinners.Add(this.RedPlayer1);
                if (this.CountRedPlayers() == 2)
                {
                    listOfWinners.Add(this.RedPlayer2);
                }
            }
            else
            {
                listOfWinners.Add(this.BluePlayer1);
                if (this.CountBluePlayers() == 2)
                {
                    listOfWinners.Add(this.BluePlayer2);
                }
            }

            return listOfWinners;
        }

        public List<Player> GetLosers()
        {
            var listOfLosers = new List<Player>();

            if (!this.WonTheMatch(this.RedPlayer1.Id))
            {
                listOfLosers.Add(this.RedPlayer1);
                if (this.CountRedPlayers() == 2)
                {
                    listOfLosers.Add(this.RedPlayer2);
                }
            }
            else
            {
                listOfLosers.Add(this.BluePlayer1);
                if (this.CountBluePlayers() == 2)
                {
                    listOfLosers.Add(this.BluePlayer2);
                }
            }

            return listOfLosers;
        }

        public int GetWinnerScore()
        {
            return Math.Max(RedScore, BlueScore);
        }

        public int GetLoserScore()
        {
            return Math.Min(RedScore, BlueScore);
        }
    }
}   
