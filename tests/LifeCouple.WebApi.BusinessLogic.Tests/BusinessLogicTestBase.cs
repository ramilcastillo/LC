using System;
using System.Linq;
using System.Runtime.CompilerServices;
using LifeCouple.DAL;
using LifeCouple.Server.Messaging;
using LifeCouple.WebApi.Common;
using LifeCouple.WebApi.DomainLogic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LifeCouple.WebApi.BusinessLogicTests
{
    public class BusinessLogicTestBase
    {
        public static string SeededUserExtRefId = "BusinessLogicTests"; //"EA5CEE24-90F9-4804-A45A-4D2E7FCA5D1A";

        internal readonly static IOptions<CosmosDbSettingsModel> cosmosDbSettingsOptions = SetOptions();
        internal readonly static IOptions<LCMessagingSettingsModel> lCMessagingSettingsModel = SetLCMessagingSettingsModelOptions();

        internal readonly static PhoneService phoneService = InitPhoneService();

        private readonly static CosmosDBRepo repo = InitRepoAndSeed();

        private readonly static bool isDbSeeded = SeedDb();

        private static IOptions<CosmosDbSettingsModel> SetOptions()
        {
            var dir = AppContext.BaseDirectory;
            //
            var config = new ConfigurationBuilder().SetBasePath(dir).AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
            var cosmosDbSettings = config.GetSection("CosmosDbSettings").Get<CosmosDbSettingsModel>();
            return Options.Create<CosmosDbSettingsModel>(cosmosDbSettings);
        }

        private static IOptions<LCMessagingSettingsModel> SetLCMessagingSettingsModelOptions()
        {
            var dir = AppContext.BaseDirectory;
            //
            var config = new ConfigurationBuilder().SetBasePath(dir).AddJsonFile("appsettings.json").AddEnvironmentVariables().AddUserSecrets("ac948920-353d-45bc-8520-906312b08889").Build();
            AppInfoModel.Init(config, config.GetSection(LifeCouple.Server.Startup.aDB2CCONFIGSECTION)?.GetChildren()?.Count() > 0, config.GetSection(LifeCouple.Server.Startup.cOSMOSDBSECTION)?.GetChildren()?.Count() > 0);
            var settings = config.GetSection(LCMessagingSettingsModel.SettingsSection).Get<LCMessagingSettingsModel>();
            return Options.Create<LCMessagingSettingsModel>(settings);
        }

        private static PhoneService InitPhoneService()
        {
            var r = new PhoneService(lCMessagingSettingsModel);
            return r;

        }




        private static CosmosDBRepo InitRepoAndSeed()
        {
            var r = new CosmosDBRepo(null, cosmosDbSettingsOptions);
            return r;
        }

        private static bool SeedDb()
        {
            var x = new BusinessLogicDataSeeder(new BusinessLogic(repo, phoneService, new MemoryCache(Options.Create<MemoryCacheOptions>(new MemoryCacheOptions()))), repo);
            x.Seed();

            return true;
        }

        public BusinessLogic BL { get; private set; }

        public BusinessLogicTestBase()
        {
            BL = new BusinessLogic(repo, phoneService, new MemoryCache(Options.Create<MemoryCacheOptions>(new MemoryCacheOptions())));
        }

        public static string GetCallerMemberName([CallerMemberName]string name = "")
        {
            return name;
        }

    }
}
