using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

/// <summary>
/// This attribute is used to secure a web api using a client key
/// Based on https://docs.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-2.1&tabs=aspnetcore2x and
/// https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core
/// </summary>
namespace LifeCouple.WebApi.Common
{
    public class ClientKeyAuthorizationAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// By default it will use the Authorization header but if queryParam is set it will use that first
        /// </summary>
        /// <param name="queryParam"></param>
        public ClientKeyAuthorizationAttribute(string queryParam = null) : base(typeof(ClientKeyAuthorizationFilter))
        {
            Arguments = new object[] { queryParam ?? "" };
        }

    }

    public class ClientKeyAuthorizationFilter : IAuthorizationFilter
    {
        readonly string _queryParam;
        readonly SystemStatusSettingsModel _settings;

        public ClientKeyAuthorizationFilter(IOptions<SystemStatusSettingsModel> systemStatusKeysSettings, string queryParam)
        {
            _queryParam = queryParam;
            _settings = systemStatusKeysSettings.Value;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string clientKey = null;

            clientKey = context.HttpContext.Request.Headers.FirstOrDefault(e => e.Key == "Authorization").Value.FirstOrDefault();

            //Query param takes precedence
            if (context.HttpContext.Request.Query.TryGetValue(_queryParam, out var values))
            {
                clientKey = values.First();
            }

            if (false == _settings.IsValidKey(clientKey))
            {
                context.Result = new UnauthorizedResult();
            }
        }

    }
}

