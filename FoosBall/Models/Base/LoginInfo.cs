namespace FoosBall.Models.Base
{
    public class LoginInfo
    {
        public SessionInfo Session { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}