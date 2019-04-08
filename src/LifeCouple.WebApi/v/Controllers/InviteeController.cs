using System;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using LifeCouple.WebApi.Common;
using LifeCouple.WebApi.DomainLogic;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LifeCouple.WebApi.v.Controllers
{
    [Produces("application/json")]
    [Route("api/pi")]
    public class InviteeController : Controller
    {
        private readonly BusinessLogic _bl;
        private readonly IConfiguration _config;

        public InviteeController(BusinessLogic businessLogic, IConfiguration config)
        {
            _bl = businessLogic;
            _config = config;
        }

        [AllowAnonymous]
        [ClientKeyAuthorization]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get_Invitation(string id)
        {
            try
            {
                var partnerInvitation = await _bl.GetPartnerInvitation_byInvitationIdAsync(id);

                if (partnerInvitation == null)
                {
                    return this.ApiErrorMessage404NotFound($"Invitation Id '{id}' not found.");
                }
                else
                {
                    var response = new InviteeResponseInfo
                    {
                        FirstName = partnerInvitation.FirstName,
                        InvitationStatus = EnumMapper.From(partnerInvitation.InvitationStatus),
                        InviterFirstName = partnerInvitation.InviterFirstName,
                        TypeOfGender = EnumMapper.From(partnerInvitation.TypeOfGender),
                        TypeOfInvitation = InvitationType.Partner,
                    };

                    return Json(response);
                }

            }
            catch (Exception ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [ClientKeyAuthorization]
        [HttpPut("{id}/decline")]
        public async Task<IActionResult> Put_DeclineInvite(string id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.ApiErrorMessage400BadRequest(ModelState);
                }

                await _bl.SetPartnerInvitationStatus(id, BL_PartnerInvitationStatusTypeEnum.Declined);

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

        [AllowAnonymous]
        [ClientKeyAuthorization]
        [HttpPut("{id}/accept")]
        public async Task<IActionResult> Put_AcceptInvite(string id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.ApiErrorMessage400BadRequest(ModelState);
                }

                await _bl.SetPartnerInvitationStatus(id, BL_PartnerInvitationStatusTypeEnum.Accepted);

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

        /// <summary>
        /// User Created in ADB2C, create invitee user passing JWT token in Authorization header (use oid from token), 
        /// and use the 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateInviteeUserFromJwtToken([FromBody] InviteeRequestInfo inviteeRequestInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.ApiErrorMessage400BadRequest(ModelState);
                }

                var jwtPayloadInfo = this.GetJwtPayloadInfo();

                await _bl.CreateInviteeUserAsync(jwtPayloadInfo.EmailAddress, jwtPayloadInfo.ExtReferenceId, jwtPayloadInfo.FirstName, jwtPayloadInfo.LastName, inviteeRequestInfo.InvitationId, inviteeRequestInfo.HasAgreedToPrivacyPolicy, inviteeRequestInfo.HasAgreedToTandC);

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