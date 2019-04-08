using System.Threading.Tasks;
using LifeCouple.Server.Instrumentation;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace LifeCouple.WebApi.Common
{
    public class JWTEvents : JwtBearerEvents
    {
        public override Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            if (arg.Exception is System.InvalidOperationException)
            {
                LCLog.Fatal(arg.Exception, "Failed to handle JWT token, probably some missing or incorrect ADB2C settings.");
            }
            else
            {
                LCLog.Fatal(arg.Exception, "Failed to authenticate JWT token");
            }

            return base.AuthenticationFailed(arg);
        }

    }
}
