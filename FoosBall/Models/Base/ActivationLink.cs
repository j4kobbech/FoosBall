namespace FoosBall.Models.Base
{
    using System;
    using System.Web.Helpers;
    using MongoDB.Bson;

    public class ActivationLink : FoosBallDoc
    {
        public ActivationLink(string userId)
        {
            this.Expires = DateTime.Now.AddDays(7);
            this.Token = Crypto.SHA256(BsonObjectId.GenerateNewId().ToString());
            this.UserId = userId;
        }

        public DateTime Expires { get; set; }

        public string UserId { get; set; }

        public string Token { get; set; }
    }
}
