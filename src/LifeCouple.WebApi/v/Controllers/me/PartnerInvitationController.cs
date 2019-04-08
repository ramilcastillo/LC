using LifeCouple.DTO.v;
using LifeCouple.WebApi.Common;
using LifeCouple.WebApi.DomainLogic;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.v.Controllers.me
{
    [Produces("application/json")]
    [Route("api/userprofiles/me/partnerinvitation")]
    public class PartnerInvitationController : Controller
    {
        private readonly BusinessLogic _bl;
        private readonly IConfiguration _config;

        public PartnerInvitationController(BusinessLogic businessLogic, IConfiguration config)
        {
            _bl = businessLogic;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<PartnerInvitationResponseInfo>> Get()
        {
            try
            {
                var jwtPayloadInfo = this.GetJwtPayloadInfo();
                var partnerInvitation = await _bl.GetPartnerInvitations_byExtRefIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (partnerInvitation == null)
                {
                    return this.ApiErrorMessage404NotFound($"No data found for UserId {jwtPayloadInfo.ExtReferenceId}");
                }

                var response = new PartnerInvitationResponseInfo
                {
                    DateOfBirth = partnerInvitation.DateOfBirth,
                    FirstName = partnerInvitation.FirstName,
                    InvitationId = partnerInvitation.InvitationId,
                    InvitationStatus = EnumMapper.From(partnerInvitation.InvitationStatus),
                    LastName = partnerInvitation.LastName,
                    MobilePhone = partnerInvitation.MobilePhone,
                    TypeOfGender = EnumMapper.From(partnerInvitation.TypeOfGender),
                };

                return Json(response);

            }
            catch (Exception ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PartnerInvitationRequestInfo request)
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

                var bl_partnerInvitation = new BL_PartnerInvitation
                {
                    DateOfBirth = request.DateOfBirth,
                    FirstName = request.FirstName,
                    InvitedByUserId = userProfileId,
                    LastName = request.LastName,
                    MobilePhone = request.MobilePhone,
                    TypeOfGender = EnumMapper.From(request.TypeOfGender),
                };

                await _bl.SetPartnerInvitationsAsync(bl_partnerInvitation);

                return this.ApiPostMessage204NotContent();

            }
            catch(BusinessLogicException ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return this.ApiErrorMessage400BadRequest(ex);
            }
            catch (Exception ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return this.ApiErrorMessage400BadRequest(ex);
            }
        }


        [HttpPost("resendinvite")]
        public async Task<IActionResult> Post_ResendInvite()
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

                await _bl.GenerateAndCreateInvitationSmsAsync(userProfileId);

                return this.ApiPostMessage204NotContent();

            }
            catch (BusinessLogicException ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return this.ApiErrorMessage400BadRequest(ex);
            }
            catch (Exception ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return this.ApiErrorMessage400BadRequest(ex);
            }
        }
    }
}
