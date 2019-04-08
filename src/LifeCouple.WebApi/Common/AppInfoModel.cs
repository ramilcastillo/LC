using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.Common
{
    public class AppInfoModel
    {

        private AppInfoModel() { }

        public static AppInfoModel Current { get; private set; }

        public DateTime AppInitBeginDT { get; private set; }
        public bool IsAuthADB2C_elseDev_TestJWT { get; private set; }
        public string AssemblyVersion { get; private set; }

        private IConfiguration Configuration { get; set; }
        public bool IsCosmosDbUsed { get; private set; }
        public bool DisableSSL { get; private set; }

        public DateTime? AppInitCompleteDT { get; private set; }


        public void SetAppInitiCompleteDT()
        {
            if (AppInitCompleteDT == null)
            {
                AppInitCompleteDT = new Func<DateTime>(() =>
                {
                    var dt = DateTime.UtcNow;
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, DateTimeKind.Utc);
                })();
            }
        }

        public string GetEnvironment()
        {
            return Configuration.GetValue<string>("ENVIRONMENT");
        }

        public static void Init(IConfiguration config, bool isAuthADB2C_else_DevTestJWT, bool isCosmosDbUsed)
        {
            if (Current != null)
            {
                throw new NotSupportedException("AppInfoModel already initialized, it can only be initialized once.");
            }



            Current = new AppInfoModel
            {
                AppInitBeginDT = new Func<DateTime>(() =>
                {
                    var dt = DateTime.UtcNow;
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, DateTimeKind.Utc);
                })(),
                IsCosmosDbUsed = isCosmosDbUsed,
                IsAuthADB2C_elseDev_TestJWT = isAuthADB2C_else_DevTestJWT,
                AssemblyVersion = typeof(AppInfoModel).Assembly.GetName().Version.ToString(),
                Configuration = config,
                DisableSSL = config.GetValue("DisableSSL", false),
            };

        }



    }
}
