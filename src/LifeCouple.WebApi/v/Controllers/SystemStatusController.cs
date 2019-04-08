using LifeCouple.DTO.v;
using LifeCouple.Server;
using LifeCouple.WebApi.Common;
using LifeCouple.WebApi.DomainLogic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.v.Controllers
{
    [Route("api/[controller]")]
    public class SystemStatusController : Controller
    {


        private readonly BusinessLogic _bl;
        private readonly IConfiguration _config;

        public SystemStatusController(BusinessLogic businessLogic, IConfiguration config)
        {
            _bl = businessLogic;
            _config = config;
        }

        private SystemStatusResponseInfo GetDefaultResponseInfo()
        {
            var debugInfo = string.Empty;

            //TODO: Once we get this Azure Key Vault working when deployed web app in Azure, it should be moved to the Startup.cs and load a config stuff, see https://grimskog.wordpress.com/azure-key-vault/
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var secret = new SecretBundle();
            //try
            //{
            //    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            //    secret = await keyVaultClient.GetSecretAsync("https://keyvaultlctest.vault.azure.net/secrets/TheSecret").ConfigureAwait(false);
            //}
            //catch (Exception exc)
            //{
            //    debugInfo = exc.Message;
            //}

            var response = new SystemStatusResponseInfo
            {
                AssemblyVersion = AppInfoModel.Current.AssemblyVersion,
                IsDbAccessible = _bl.IsDbAccesible(),
                AppInitBeginDT = AppInfoModel.Current.AppInitBeginDT,
                AppInitDT = AppInfoModel.Current.AppInitCompleteDT.Value,
                UpTimeSinceAppInit = new Func<TimeSpan>(() =>
                {
                    var duration = (DateTime.UtcNow - AppInfoModel.Current.AppInitCompleteDT.Value);
                    return new TimeSpan(duration.Days, duration.Hours, duration.Minutes, duration.Seconds); //ignore milliseconds
                })(),
                Environment = AppInfoModel.Current.GetEnvironment(),
                IsAuthADB2C_elseDev_TestJWT = AppInfoModel.Current.IsAuthADB2C_elseDev_TestJWT,
                IsCosmosDbUsed = AppInfoModel.Current.IsCosmosDbUsed,
                DebugInfo = $"{debugInfo}... KeyVault:'{secret?.Value}'",
            };

            return response;
        }

        [AllowAnonymous]
        [ClientKeyAuthorization("$clientAccessKey")]
        [HttpGet]
        public ActionResult<SystemStatusResponseInfo> GetBasicStatus()
        {
            return Json(GetDefaultResponseInfo());
        }
    }
}
