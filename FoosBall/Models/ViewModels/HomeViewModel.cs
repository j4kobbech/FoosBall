namespace FoosBall.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Base;

    public class HomeViewModel : BaseViewModel
    {
        private readonly List<string> welcomeMessages = new List<string>
                                             {
                                                 "Welcome to the FoosBall Tournament App!!",
                                                 "Are you ready to score The Goal Of The Year?",
                                                 "Welcome FoosBall Fighter, We've been expecting you.",
                                                 "Ready to challenge another FoosBall Fighter?",
                                                 "Come on in, set up a match, enjoy!"
                                             };

        public HomeViewModel()
        {
            WelcomeMessage = GetWelcomeMessage();
        }

        public string WelcomeMessage { get; set; }

        public string GetWelcomeMessage(int index = -1)
        {
            return index < 0 ? welcomeMessages[new Random().Next(0, welcomeMessages.Count)] : welcomeMessages[index];
        }
    }
}
