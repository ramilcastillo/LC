using System;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using LifeCouple.WebApi.Common;
using LifeCouple.WebApi.DomainLogic;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LifeCouple.WebApi.v.Controllers.me
{
    [Produces("application/json")]
    [Route("api/userprofiles/me/appcenter")]
    public class AppCenterController : Controller
    {
        private readonly BusinessLogicAppCenter _bl;
        private readonly IConfiguration _config;

        public AppCenterController(BusinessLogicAppCenter businessLogic, IConfiguration config)
        {
            _bl = businessLogic;
            _config = config;
        }


        [HttpPut]
        public async Task<IActionResult> Post([FromBody] AppCenterRequestInfo request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.ApiErrorMessage400BadRequest(ModelState);
                }

                var jwtPayloadInfo = this.GetJwtPayloadInfo();
                var userProfileId = await _bl.GetCachedUserId_byExternalReferenceIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (userProfileId == null)
                {
                    return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(jwtPayloadInfo.ExtReferenceId);
                }

                var bl_dto = new BL_AppCenterDetails
                {
                    DeviceId = request.DeviceId,
                    DTUpdated = DateTimeOffset.MinValue,
                    TypeOfOs = EnumMapper.From(request.TypeDeviceOs),
                    UserprofileId = userProfileId
                };

                await _bl.SetAppCenterDetailsAsync(bl_dto);

                return this.ApiPutMessage204NotContent();

            }
            catch (BusinessLogicException ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return this.ApiErrorMessage400BadRequest(ex);
            }
            catch (Exception ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
