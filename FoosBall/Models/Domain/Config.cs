﻿namespace FoosBall.Models.Domain
{
    using System.Collections.Generic;
    using FoosBall.Models.Base;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Config : FoosBallDoc
    {
        public Config()
        {
            this.Name = "FoosBall Fighting";
            this.Domain = "trustpilot.com";
            this.AdminAccount = new List<string>()
                {
                    "jbe@trustpilot.com",
                    "olj@trustpilot.com"
                };
        }

        public string Name { get; set; }

        public string Domain { get; set; }

        public List<string> AdminAccount { get; set; } 

        public Environment Environment { get; set; }
    }
}