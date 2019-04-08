using System;
using System.Collections.Generic;
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
    [Route("api/userprofiles")]
    public class UserProfilesController : Controller
    {
        private readonly BusinessLogic _bl;
        private readonly IConfiguration _config;

        public UserProfilesController(BusinessLogic businessLogic, IConfiguration config)
        {
            _bl = businessLogic;
            _config = config;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var jwtPayloadInfo = this.GetJwtPayloadInfo();

                var existingUserProfile = await _bl.GetUserProfile_byExtRefIdAsync(jwtPayloadInfo.ExtReferenceId);

                if (existingUserProfile == null)
                {
                    return this.ApiErrorMessage404NotFound($"UserId {jwtPayloadInfo.ExtReferenceId} not found");
                }
                else
                {
                    var response = new UserProfileResponseInfo
                    {
                        Email = existingUserProfile.PrimaryEmail,
                        EnrolledModules = new string[] { "LC90Module" },
                        FirstName = _bl.GetStringWithFirstCharacterUpper(existingUserProfile.FirstName),
                        LastName = existingUserProfile.LastName,
                        Status = getStatus(),
                        Indexes = getIndexes(),
                        PartnerInvitationStatus = EnumMapper.From(existingUserProfile.PartnerInvitationStatus)
                    };

                    UserProfileStatusType getStatus()
                    {
                        if (existingUserProfile.IsOnboardingQuesitonnaireCompleted())
                        {
                            return UserProfileStatusType.Complete;
                        }

                        if (existingUserProfile.IsRegistrationCompleted())
                        {
                            return UserProfileStatusType.Pending;
                        }
                        else
                        {
                            return UserProfileStatusType.New;
                        }
                    }

                    List<UserIndex> getIndexes()
                    {
                        var r = new List<UserIndex>();
                        if (existingUserProfile?.Indexes != null)
                        {
                            foreach (var i in existingUserProfile.Indexes)
                            {
                                var index = IndexType.Unknown;
                                if (!Enum.TryParse<IndexType>(i.TypeOfIndex.ToString(), out index))
                                {
                                    //TODO: Log the fact that converting this enum value failed, as a Warning
                                }
                                r.Add(new UserIndex { TypeOfIndex = index, Value = i.Value });
                            }
                        }
                        return r;
                    }

                    return Json(response);
                }
            }
            catch (Exception exc)
            {
                return BadRequest(exc);
            }

        }


        /// <summary>
        /// User Created in ADB2C, create user and use oid from token
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUserFromJwtToken()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var jwtPayloadInfo = this.GetJwtPayloadInfo();

            var existingUserProfile = await _bl.GetUserProfile_byExtRefIdAsync(jwtPayloadInfo.ExtReferenceId);
            if (existingUserProfile == null)
            {
                var newUserProfile = await _bl.CreateUserAsync(jwtPayloadInfo.EmailAddress, jwtPayloadInfo.ExtReferenceId, false, jwtPayloadInfo.FirstName, jwtPayloadInfo.LastName, null);
                return Created("me", "");
            }
            else
            {
                return this.ApiErrorMessage404NotFound($"Unable to create user with ExtRefenceNr: '{jwtPayloadInfo.ExtReferenceId}' and Email:'{jwtPayloadInfo.EmailAddress}' since it already exists.");
            }

        }

        

        /// <summary>
        /// Create user now, for devtest scenario and generate oid using guid
        /// </summary>
        /// <param name="userProfileRequestInfo"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ClientKeyAuthorization]
        [HttpPost("devuser")]
        public async Task<IActionResult> CreateUser([FromBody] UserProfileRequestInfo userProfileRequestInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (AppInfoModel.Current.IsAuthADB2C_elseDev_TestJWT)
            {
                return this.ApiErrorMessage404NotFound("");
            }

            var existingUserProfiles = await _bl.FindUserProfiles_byEmailAsync(userProfileRequestInfo.Email, true);

            if (existingUserProfiles == null || existingUserProfiles.Count == 0)
            {
                var newUserProfile = await _bl.CreateUserAsync(userProfileRequestInfo.Email, Guid.NewGuid().ToString("N").ToUpper(), true, userProfileRequestInfo.Email.ToLower(), userProfileRequestInfo.Email.ToUpper(), null);
                return Created("me", "");
            }
            else
            {
                return this.ApiErrorMessage404NotFound($"Unable to create user, '{userProfileRequestInfo.Email}' already exists.");
            }
        }
    }
}
