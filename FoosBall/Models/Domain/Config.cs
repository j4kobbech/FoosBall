namespace FoosBall.Models.Domain
{
    using System.Collections.Generic;
    using Base;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Config : FoosBallDoc
    {
        public Config()
        {
            this.Name = "FoosBall";
            this.Domain = string.Empty;
            this.AdminAccounts = new List<string>();
        }

        public string Name { get; set; }

        public string Domain { get; set; }

        public string NotificationUrl { get; set; }

        public List<string> AdminAccounts { get; set; } 

        public Environment Environment { get; set; }
    }
}