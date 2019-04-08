using LifeCouple.DTO.v;
using LifeCouple.WebApi.Common;
using LifeCouple.WebApi.DomainLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.v.Controllers
{
    [Produces("application/json")]
    [Route("api/userprofiles/me/registration")]
    public class UserProfilesRegistrationController : Controller
    {
        private readonly BusinessLogic _bl;
        private readonly IConfiguration _config;

        public UserProfilesRegistrationController(BusinessLogic businessLogic, IConfiguration config)
        {
            _bl = businessLogic;
            _config = config;
        }

        [HttpGet("aboutyou")]
        public async Task<IActionResult> GetAboutYou()
        {
            try
            {
                var jwtPayloadInfo = this.GetJwtPayloadInfo();

                var userProfileId = await _bl.GetCachedUserId_byExternalReferenceIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (userProfileId == null)
                {
                    return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(jwtPayloadInfo.ExtReferenceId);
                }

                var userReg = await _bl.GetUserProfile_byExtRefIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (userReg == null)
                {
                    return this.ApiErrorMessage404NotFound($"No data found for UserId {jwtPayloadInfo.ExtReferenceId}");
                }

                var response = new UserProfileRegAboutYouResponseInfo
                {
                    DateOfBirth = userReg.DateOfBirth,
                    Email = userReg.PrimaryEmail,
                    FirstName = userReg.FirstName,
                    Gender = userReg.Gender,
                    NotificationOption = userReg.NotificationOption,
                    LastName = userReg.LastName,
                    MobilePhone = userReg.PrimaryMobileNr
                };
                return Json(response);
            }
            catch (Exception exc)
            {
                return this.ApiErrorMessage400BadRequest(exc);
            }
        }

        [HttpPut("aboutyou")]
        public async Task<IActionResult> SetAboutYou([FromBody] UserProfileRegAboutYouRequestInfo usrProRegAboYouReqInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.ApiErrorMessage400BadRequest(this.ModelState);
                }

                var jwtPayloadInfo = this.GetJwtPayloadInfo();

                var userProfileId = await _bl.GetCachedUserId_byExternalReferenceIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (userProfileId == null)
                {
                    return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(jwtPayloadInfo.ExtReferenceId);
                }



                var bl_usrProReg = new BL_UserProfile
                {
                    Id = userProfileId,
                    PrimaryEmail = usrProRegAboYouReqInfo.Email,
                    DateOfBirth = usrProRegAboYouReqInfo.DateOfBirth,
                    FirstName = usrProRegAboYouReqInfo.FirstName,
                    Gender = usrProRegAboYouReqInfo.Gender,
                    NotificationOption = usrProRegAboYouReqInfo.NotificationOption,
                    LastName = usrProRegAboYouReqInfo.LastName,
                    PrimaryMobileNr = usrProRegAboYouReqInfo.MobilePhone,
                };
                await _bl.SetUserAsync(bl_usrProReg);
                return this.ApiPutMessage204NotContent();
            }
            catch (Exception exc)
            {
                return this.ApiErrorMessage400BadRequest(exc);
            }
        }

        [HttpGet("aboutyourpartner")]
        public IActionResult GetAboutYourPartner()
        {
            return this.ApiMessage501NotImplemented();

            //try
            //{
            //    var externalRefId = this.GetExternalUserIdFromTokenExt();
            //    var existingUserProfile = await _bl.GetUserProfile_byExtRefIdAsync(externalRefId);
            //    if (existingUserProfile == null)
            //    {
            //        return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(externalRefId);
            //    }

            //    var userReg = await _bl.GetUserProfileRegistrationAsync(existingUserProfile.Id);
            //    if (userReg == null)
            //    {
            //        return this.ApiErrorMessage404NotFound($"No data found for UserId {externalRefId}");
            //    }

            //    var response = new UserProfileRegAboutYourPartnerResponseInfo
            //    {
            //        PartnerDateOfBirth = userReg.PartnerDateOfBirth,
            //        PartnerEmail = userReg.PartnerEmail,
            //        PartnerFirstName = userReg.PartnerFirstName,
            //        PartnerGender = userReg.PartnerGender,
            //        PartnerLastName = userReg.PartnerLastName,
            //        PartnerMobilePhone = userReg.PartnerMobilePhone,
            //    };
            //    return Json(response);
            //}
            //catch (Exception exc)
            //{
            //    return this.ApiErrorMessage400BadRequest(exc);
            //}
        }

        [HttpPut("aboutyourpartner")]
        public IActionResult SetAboutYourPartner([FromBody] UserProfileRegAboutYourPartnerRequestInfo u)
        {
            return this.ApiMessage501NotImplemented();

            //try
            //{
            //    if (!ModelState.IsValid)
            //    {
            //        return this.ApiErrorMessage400BadRequest(this.ModelState);
            //    }

            //    var externalRefId = this.GetExternalUserIdFromTokenExt();
            //    var existingUserProfile = await _bl.GetUserProfile_byExtRefIdAsync(externalRefId);
            //    if (existingUserProfile == null)
            //    {
            //        return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(externalRefId);
            //    }

            //    var bl_usrProReg = new BL_UserProfileRegistration
            //    {
            //        PartnerDateOfBirth = u.PartnerDateOfBirth,
            //        PartnerEmail = u.PartnerEmail,
            //        PartnerFirstName = u.PartnerFirstName,
            //        PartnerGender = u.PartnerGender,
            //        PartnerLastName = u.PartnerLastName,
            //        PartnerMobilePhone = u.PartnerMobilePhone,

            //        UserProfile_Id = existingUserProfile.Id,
            //    };
            //    await _bl.SetUserProfileRegistrationAsync(bl_usrProReg);
            //    return this.ApiPutMessage204NotContent();
            //}
            //catch (Exception exc)
            //{
            //    return this.ApiErrorMessage400BadRequest(exc);
            //}
        }

        [HttpGet("aboutyourrelationship")]
        public async Task<IActionResult> GetAboutYourRelationship()
        {
            try
            {
                var jwtPayloadInfo = this.GetJwtPayloadInfo();

                var existingUserProfile = await _bl.GetUserProfile_byExtRefIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (existingUserProfile == null)
                {
                    return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(jwtPayloadInfo.ExtReferenceId);
                }


                var usrReg = await _bl.GetRelationshipAsync(existingUserProfile.Id);
                if (usrReg == null)
                {
                    return this.ApiErrorMessage404NotFound($"No data found for UserId {jwtPayloadInfo.ExtReferenceId}");
                }

                var response = new UserProfileRegAboutYourRelationshipResponseInfo
                {
                    //BeenToCounselorOrTherapist = ??? => set in below switch
                    HasMoreThanOneMarriage = null, //set in below switch
                    IsMarried = null, //set in below switch
                    LastWeddingDate = usrReg.CurrentWeddingDate,
                    NrOfChildren = usrReg.NrOfChildren,
                    NrOfStepChildren = usrReg.NrOfStepChildren,
                };
                switch (usrReg.BeenToCounseler)
                {
                    case null:
                        response.BeenToCounselorOrTherapist = BeenToCounselorOrTherapistType.Unknown;
                        break;
                    case BL_Relationship.CounselerStatusEnum.NoNever:
                        response.BeenToCounselorOrTherapist = BeenToCounselorOrTherapistType.NoNever;
                        break;
                    case BL_Relationship.CounselerStatusEnum.Unknown:
                        response.BeenToCounselorOrTherapist = BeenToCounselorOrTherapistType.Unknown;
                        break;
                    case BL_Relationship.CounselerStatusEnum.YesCurrently:
                        response.BeenToCounselorOrTherapist = BeenToCounselorOrTherapistType.YesCurrently;
                        break;
                    case BL_Relationship.CounselerStatusEnum.YesInThePast:
                        response.BeenToCounselorOrTherapist = BeenToCounselorOrTherapistType.YesInThePast;
                        break;
                    default:
                        throw new ApplicationException($"unknown value '{usrReg.MarriageStatus.ToString()}'");
                }
                switch (usrReg.MarriageStatus)
                {
                    case null:
                        response.HasMoreThanOneMarriage = null;
                        break;
                    case BL_Relationship.MarriageStatusEnum.FirstMarriage:
                        response.HasMoreThanOneMarriage = false;
                        break;
                    case BL_Relationship.MarriageStatusEnum.MoreThanOneMarriage:
                        response.HasMoreThanOneMarriage = true;
                        break;
                    case BL_Relationship.MarriageStatusEnum.Unknown:
                        response.HasMoreThanOneMarriage = null;
                        break;
                    default:
                        throw new ApplicationException($"unknown value '{usrReg.MarriageStatus.ToString()}'");
                }
                switch (usrReg.RelationshipStatus)
                {
                    case null:
                        response.IsMarried = null;
                        break;
                    case BL_Relationship.RelationshipStatusEnum.Married:
                        response.IsMarried = true;
                        break;
                    case BL_Relationship.RelationshipStatusEnum.NotMarried:
                        response.IsMarried = false;
                        break;
                    case BL_Relationship.RelationshipStatusEnum.Unknown:
                        response.IsMarried = null;
                        break;
                    default:
                        throw new ApplicationException($"unknown value '{usrReg.RelationshipStatus.ToString()}'");
                }



                return Json(response);
            }
            catch (Exception exc)
            {
                return this.ApiErrorMessage400BadRequest(exc);
            }
        }

        [HttpPut("aboutyourrelationship")]
        public async Task<IActionResult> SetAboutYourRelationship([FromBody] UserProfileRegAboutYourRelationshipRequestInfo u)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.ApiErrorMessage400BadRequest(this.ModelState);
                }

                var jwtPayloadInfo = this.GetJwtPayloadInfo();

                var existingUserProfileId = await _bl.GetCachedUserId_byExternalReferenceIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (existingUserProfileId == null)
                {
                    return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(jwtPayloadInfo.ExtReferenceId);
                }

                var bl_relationship = new BL_Relationship
                {
                    BeenToCounseler = null, //Set in Switch statement below
                    CurrentWeddingDate = u.LastWeddingDate,
                    RegisteredPartner_Id = null,
                    MarriageStatus = null, //Set in Switch statement below
                    RelationshipStatus = null,  //Set in Switch statement below
                    NrOfChildren = u.NrOfChildren,
                    Id = null,
                    NrOfStepChildren = u.NrOfStepChildren,
                };

                switch (u.BeenToCounselorOrTherapist)
                {
                    case BeenToCounselorOrTherapistType.NoNever:
                        bl_relationship.BeenToCounseler = BL_Relationship.CounselerStatusEnum.NoNever;
                        break;
                    case BeenToCounselorOrTherapistType.Unknown:
                        bl_relationship.BeenToCounseler = BL_Relationship.CounselerStatusEnum.Unknown;
                        break;
                    case BeenToCounselorOrTherapistType.YesCurrently:
                        bl_relationship.BeenToCounseler = BL_Relationship.CounselerStatusEnum.YesCurrently;
                        break;
                    case BeenToCounselorOrTherapistType.YesInThePast:
                        bl_relationship.BeenToCounseler = BL_Relationship.CounselerStatusEnum.YesInThePast;
                        break;
                    default:
                        bl_relationship.BeenToCounseler = null;
                        break;

                }

                switch (u.HasMoreThanOneMarriage)
                {
                    case null:
                        bl_relationship.MarriageStatus = null;
                        break;
                    case true:
                        bl_relationship.MarriageStatus = BL_Relationship.MarriageStatusEnum.MoreThanOneMarriage;
                        break;
                    case false:
                        bl_relationship.MarriageStatus = BL_Relationship.MarriageStatusEnum.FirstMarriage;
                        break;
                }
                switch (u.IsMarried)
                {
                    case null:
                        bl_relationship.RelationshipStatus = null;
                        break;
                    case true:
                        bl_relationship.RelationshipStatus = BL_Relationship.RelationshipStatusEnum.Married;
                        break;
                    case false:
                        bl_relationship.RelationshipStatus = BL_Relationship.RelationshipStatusEnum.NotMarried;
                        break;
                }


                await _bl.SetRelationshipAsync(bl_relationship, existingUserProfileId);

                //var bl_usrProReg = new BL_UserProfileRegistration
                //{
                //    HasMoreThanOneMarriage = u.HasMoreThanOneMarriage,
                //    IsMarried = u.IsMarried,
                //    LastWeddingDate = u.LastWeddingDate,
                //    NrOfChildren = u.NrOfChildren,
                //    NrOfStepChildren = u.NrOfStepChildren,

                //    UserProfile_Id = existingUserProfileId,
                //};
                //await _dl.SetUserProfileRegistrationAsync(bl_usrProReg);
                return this.ApiPutMessage204NotContent();
            }
            catch (Exception exc)
            {
                return this.ApiErrorMessage400BadRequest(exc);
            }
        }

        [HttpGet("paymentandterms")]
        public async Task<IActionResult> GetPaymentAndTerms()
        {
            try
            {
                var jwtPayloadInfo = this.GetJwtPayloadInfo();

                var existingUserProfile = await _bl.GetUserProfile_byExtRefIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (existingUserProfile == null)
                {
                    return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(jwtPayloadInfo.ExtReferenceId);
                }

                var response = new UserProfileRegPaymentResponseInfo
                {
                    HasAgreedToPrivacyPolicy = existingUserProfile.HasAgreedToPrivacyPolicy,
                    HasAgreedToRefundPolicy = existingUserProfile.HasAgreedToRefundPolicy,
                    HasAgreedToTandC = existingUserProfile.HasAgreedToTandC,
                };
                return Json(response);
            }
            catch (Exception exc)
            {
                return this.ApiErrorMessage400BadRequest(exc);
            }
        }

        [HttpPut("paymentandterms")]
        public async Task<IActionResult> SetPaymentAndTerms([FromBody] UserProfileRegPaymentRequestInfo u)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.ApiErrorMessage400BadRequest(this.ModelState);
                }

                var jwtPayloadInfo = this.GetJwtPayloadInfo();

                var userId = await _bl.GetCachedUserId_byExternalReferenceIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (userId == null)
                {
                    return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(jwtPayloadInfo.ExtReferenceId);
                }

                var bl_usrProReg = new BL_UserProfile
                {
                    HasAgreedToPrivacyPolicy = u.HasAgreedToPrivacyPolicy,
                    HasAgreedToRefundPolicy = u.HasAgreedToRefundPolicy,
                    HasAgreedToTandC = u.HasAgreedToTandC,

                    Id = userId,
                };
                await _bl.SetUserAsync(bl_usrProReg);
                return this.ApiPutMessage204NotContent();
            }
            catch (Exception exc)
            {
                return this.ApiErrorMessage400BadRequest(exc);
            }
        }

    }
}
