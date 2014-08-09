namespace FoosBall.Models.Base
{
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class AccessToken : FoosBallDoc
    {
        public AccessToken()
        {
            Expires = DateTime.Now.AddMinutes(60);
            Role = Role.User;
        }
        
        public DateTime Expires { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        
        public string TokenValue { get; set; }

        public Role Role { get; set; }
    }
}
