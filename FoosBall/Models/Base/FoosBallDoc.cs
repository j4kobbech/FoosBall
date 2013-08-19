namespace FoosBall.Models.Base
{
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public abstract class FoosBallDoc
    {
        protected FoosBallDoc()
        {
            this.Created = DateTime.Now;
            this.Updated = DateTime.Now;
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public DateTime Created { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string CreatedBy { get; set; }

        public DateTime Updated { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UpdatedBy { get; set; }
    }
}