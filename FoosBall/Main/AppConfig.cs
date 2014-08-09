namespace FoosBall.Main
{
    using System.Configuration;
    using System.Linq;
    using Models.Base;
    using Models.Domain;

    public static class AppConfig
    {
        public static void InitalizeConfig()
        {
            var configCollection = new Db().Dbh.GetCollection<Config>("Config");
            var config = configCollection.FindAll().FirstOrDefault();
            var environment = GetEnvironment();

            if (config != null)
            {
                return;
            }

            config = new Config
            {
                Environment = environment
            };

            configCollection.Save(config);
        }

        public static Environment GetEnvironment()
        {
            Environment environment;
            var environmentString = ConfigurationManager.AppSettings["Environment"];

            switch (environmentString)
            {
                case "Production":
                    environment = Environment.Production;
                    break;
                case "Staging":
                    environment = Environment.Staging;
                    break;
                default:
                    environment = Environment.Local;
                    break;
            }

            return environment;
        } 
    }
}