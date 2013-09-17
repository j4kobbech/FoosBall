namespace FoosBall.Models.ViewModels
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using FoosBall.Models.Base;
    using FoosBall.Models.Domain;

    public class PlayerBaseDataViewModel : BaseViewModel
    {
        public PlayerBaseDataViewModel()
        {
            this.ReferralUrl = "/Players";
        }

        public Player Player { get; set; }

        public bool SaveSuccess { get; set; }

        public string ReferralUrl { get; set; }
    }
}
