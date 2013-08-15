namespace FoosBall.Main
{
    using System.Web.Helpers;
    using Microsoft.AspNet.SignalR;
    using Models.Domain;

    public class EventHub : Hub
    {
        public static void Send(object message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
            var match = (Match)message; 

            context.Clients.All.BroadcastMessage(Json.Encode(new 
            {
                BluePlayer1 = match.BluePlayer1.Name,
                BluePlayer2 = match.BluePlayer2.Name ?? string.Empty,
                RedPlayer1 = match.RedPlayer1.Name,
                RedPlayer2 = match.RedPlayer2.Name ?? string.Empty,
                RedScore = match.RedScore,
                BlueScore = match.BlueScore
            }));
        }
    }
}