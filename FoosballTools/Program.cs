namespace FoosballTools
{
    using System;
    using FoosBall.Main;
    using FoosBall.Models.Domain;

    public class Program
    {
        public static void Main(string[] args)
        {
            var dbConnection = new Db();
            var playersCollection = dbConnection.Dbh.GetCollection<Player>("Players");
            
            dbConnection.Dbh.DropCollection("Users");
            
            var usersCollection = dbConnection.Dbh.GetCollection<User>("Users");

            foreach (var player in playersCollection.FindAll())
            {
                var user = new User
                {
                    Name = player.Name,
                    Email = player.Email,
                    Password  = player.Password,
                    Deactivated = player.Deactivated,
                };

                Console.WriteLine(player.Name);
                usersCollection.Save(user);
            }
        }
    }
}
