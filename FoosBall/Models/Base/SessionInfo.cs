namespace FoosBall.Models.Base
{
    using Domain;

    public class SessionInfo
    {
        public Player User { get; set; }

        public bool IsLoggedIn { get; set; }
        
        public bool IsAdmin { get; set; }
    }
}