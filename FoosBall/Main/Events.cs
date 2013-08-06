namespace FoosBall.Main
{
    using System;

    using Models.Base;
    using Models.Domain;
    using MongoDB.Bson;

    public static class Events
    {
        public static void SubmitEvent(EventType eventType, object targetObject, string userId)
        {
            EventHub.Send(targetObject);
            SaveEvent(eventType, targetObject, userId);
        }

        private static void SaveEvent(EventType eventType, object targetObject, string userId) 
        {
            if (eventType != EventType.Default && targetObject != null)
            {
                var dbh = new Db(AppConfig.GetEnvironment()).Dbh;
                var eventCollection = dbh.GetCollection<Event>("Events");

                var newEvent = new Event
                {
                    EventType = eventType,
                    Created = new BsonDateTime(DateTime.Now),
                    CreatedBy = userId
                };

                if (eventType == EventType.PlayerLogin || eventType == EventType.PlayerCreate)
                {   
                    newEvent.Player = (Player)targetObject;
                }

                if (eventType == EventType.MatchResolve)
                {
                    newEvent.Match = (Match)targetObject;
                }

                eventCollection.Insert(newEvent);
            }
        }
    }
}