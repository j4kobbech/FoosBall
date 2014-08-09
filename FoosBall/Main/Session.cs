namespace FoosBall.Main
{
    using System.Web;
    using Models.Base;
    using Models.Domain;
    using Models.ViewModels;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public static class Session
    {
        private static readonly MongoDatabase MongoDatabase = new Db().Dbh;

        public static bool IsUserCredentialsValid(User user, string password)
        {
            return user != null && Md5.CalculateMd5(password) == user.Password;
        }

        public static AccessToken GetAccessToken(HttpContext httpContext)
        {
            var accessTokenHeaderValue = httpContext.Request.Headers.Get("AccessToken");

            if (accessTokenHeaderValue == null || string.IsNullOrWhiteSpace(accessTokenHeaderValue))
            {
                return null;
            }

            var dbh = new Db().Dbh;
            var accessTokenCollection = dbh.GetCollection<AccessToken>("AccessTokens");
            var accessToken = accessTokenCollection.FindOne(Query.EQ("TokenValue", accessTokenHeaderValue));

            return accessToken;
        }

        public static AccessToken CreateNewAccessTokenOnUser(User user)
        {
            var accessToken = new AccessToken
            {
                TokenValue = ObjectId.GenerateNewId().ToString(),
                UserId = user.Id
            };

            return accessToken;
        }

        public static SessionInfo BuildSessionInfo(AccessToken accessToken, User user)
        {
            var sessionInfo = new SessionInfo();

            if (accessToken != null)
            {
                sessionInfo.UserId = accessToken.UserId;
                sessionInfo.UserName = user.Name;
                sessionInfo.TokenValue = accessToken.TokenValue;
                sessionInfo.Role = accessToken.Role.ToString();
            }

            return sessionInfo;
        }

        public static void SaveAccessToken(AccessToken accessToken)
        {
            var accessTokenCollection = MongoDatabase.GetCollection<AccessToken>("AccessTokens");

            // Remove previously set AccessTokens before setting a new one. 
            accessTokenCollection.Remove(Query.EQ("UserId", BsonObjectId.Parse(accessToken.UserId)));

            accessTokenCollection.Save(accessToken);
        }

        public static User GetCurrentUser()
        {
            var accessToken = GetAccessToken(HttpContext.Current);

            if (accessToken == null)
            {
                return null;
            }

            var userCollection = MongoDatabase.GetCollection<User>("Users");
            var user = userCollection.FindOne(Query.EQ("_id", ObjectId.Parse(accessToken.UserId)));

            return user;
        }
    }
}