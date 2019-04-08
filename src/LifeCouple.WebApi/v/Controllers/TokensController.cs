using System.Linq;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using LifeCouple.WebApi.Common;
using LifeCouple.WebApi.DomainLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LifeCouple.WebApi.v.Controllers
{
    [Produces("application/json")]
    [Route("api/tokens")]
    public class TokensController : Controller
    {
        private readonly BusinessLogic _bl;
        private readonly IConfiguration _config;
        private readonly IJWTGenerator _jwtHandler;

        public TokensController(BusinessLogic businessLogic, IConfiguration config, IJWTGenerator jwtHandler)
        {
            _bl = businessLogic;
            _config = config;
            _jwtHandler = jwtHandler;
        }

        /// <summary>
        /// Only for DevTest s
        /// </summary>
        /// <param name="tokenInfo"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ClientKeyAuthorization]
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] TokenRequestInfo tokenInfo)
        {
            if (ModelState.IsValid)
            {
                //Authenticate credentials...
                if (string.IsNullOrWhiteSpace(tokenInfo?.Email)
                    || string.IsNullOrWhiteSpace(tokenInfo?.Password)
                    || (tokenInfo.Email?.Substring(0, tokenInfo.Email.IndexOf("@") + 1) != tokenInfo.Password)) //e.g. per@grimskog.com and per@ are ok as email and password
                {
                    return this.ApiErrorMessage404NotFound("Unable to create token. Invalid credentials.");
                }

                var r = await _bl.FindUserProfiles_byEmailAsync(tokenInfo.Email, true);
                if (r == null || r.Count == 0)
                {
                    return this.ApiErrorMessage404NotFound($"Unable to create token. No devTest user with email '{tokenInfo.Email}' found, create user first.");
                }
                if (r.Count != 1)
                {
                    return this.ApiErrorMessage404NotFound($"Unable to create token. Found more than one devTest user with '{tokenInfo.Email}'.");
                }

                var userProfile = r.First();


                var jwtToken = _jwtHandler.Create(userProfile.ExternalRefId, userProfile.PrimaryEmail, userProfile.FirstName, userProfile.LastName);
                var results = new TokenResponseInfo
                {
                    Token = jwtToken.Token,
                    Expiration = jwtToken.Expires
                };

                return Created("", results);
            }
            //    }
            //}

            return BadRequest();
        }


    }

}
