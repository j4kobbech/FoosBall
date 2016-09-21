namespace FoosBall.Models.Domain
{
    using System.Collections.Generic;
    using Base;
    
    public class TeamName : FoosBallDoc
    {
        public string Name { get; set; }

        public Team Team { get; set; }
    }
}