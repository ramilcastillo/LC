using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LifeCouple.Server.Messaging.Tests
{
    public abstract class MessagingTestBase
    {
        internal readonly static IOptions<LCMessagingSettingsModel> settingsOptions = SetOptions();

        public static StringContent GetJsonContent(string jsonString) => new StringContent(jsonString, Encoding.UTF8, "application/json");

        public static StringContent GetJsonContent(object entity) => GetJsonContent(JsonConvert.SerializeObject(entity));

        public static string GetCallerMemberName([CallerMemberName]string name = "")
        {
            return name;
        }

        private static IOptions<LCMessagingSettingsModel> SetOptions()
        {
            var dir = AppContext.BaseDirectory;
            var config = new ConfigurationBuilder().SetBasePath(dir).AddJsonFile("appsettings.json", true).AddEnvironmentVariables().AddUserSecrets("ac948920-353d-45bc-8520-906312b08889").Build();
            var settings = config.GetSection(LCMessagingSettingsModel.SettingsSection).Get<LCMessagingSettingsModel>();
            return Options.Create<LCMessagingSettingsModel>(settings);
        }


    }
}
