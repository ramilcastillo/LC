using Microsoft.IdentityModel.Tokens;

namespace LifeCouple.WebApi.Common
{
    public interface IJWTGenerator
    {
        JWTModel Create(string userId, string emailAddress, string firstName, string lastName);
        TokenValidationParameters Parameters { get; }
    }
}
