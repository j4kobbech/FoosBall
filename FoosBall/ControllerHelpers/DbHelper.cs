namespace FoosBall.ControllerHelpers
{
    using System;
    using Main;
    using Models.Domain;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public static class DbHelper
    {
        private static readonly MongoDatabase Dbh = new Db().Dbh;

        public static User GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("id parameter cannot be null or empty");
            }

            return Dbh.GetCollection<User>("Users").FindOne(Query.EQ("Email", email));
        }

        public static User GetUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("id parameter cannot be null or empty");
            }

            return Dbh.GetCollection<User>("Users").FindOne(Query.EQ("_id", BsonObjectId.Parse(id)));
        }

        public static Player GetPlayer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("id parameter cannot be null or empty");
            }

            return Dbh.GetCollection<Player>("Players").FindOne(Query.EQ("_id", BsonObjectId.Parse(id)));
        }

        public static SafeModeResult SaveUser(User user)
        {
            if (user == null)
            {
                throw new Exception("user parameter cannot be null");
            }

            return Dbh.GetCollection<User>("Users").Save(user);
        }

        public static SafeModeResult SavePlayer(Player user)
        {
            if (user == null)
            {
                throw new Exception("player parameter cannot be null");
            }

            return Dbh.GetCollection<Player>("Players").Save(user);
        }
    }
}
