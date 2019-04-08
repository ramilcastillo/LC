using System;
using System.Threading.Tasks;
using LifeCouple.DAL.Entities;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;

namespace LifeCouple.WebApi.DomainLogic
{
    public partial class BusinessLogic
    {

        public async Task<BL_PartnerInvitation> GetPartnerInvitations_byExtRefIdAsync(string externalRefId)
        {
            var userProfile = await _repo.GetUserProfile_byExtRefIdAsync(externalRefId);
            return map.From_ForPartnerInvitation(userProfile);
        }

        public async Task<BL_PartnerInvitation> GetPartnerInvitation_byInvitationIdAsync(string invitationId)
        {
            var userProfile = await _repo.GetUserProfile_byInvitationId(invitationId);
            return map.From_ForPartnerInvitation(userProfile);
        }

        /// <summary>
        /// to be used within web api controller(s)
        /// </summary>
        /// <param name="bL_QuestionnaireAnswers"></param>
        /// <returns></returns>
        public async Task<string> SetPartnerInvitationsAsync(BL_PartnerInvitation bl_dto)
        {
            if (string.IsNullOrWhiteSpace(bl_dto.InvitedByUserId))
            {
                throw new BusinessLogicException(ValidationErrorCode.EntityIsNull, bl_dto.InvitedByUserId);
            }

            if (false == string.IsNullOrWhiteSpace(bl_dto.InviterFirstName))
            {
                throw new BusinessLogicException(ValidationErrorCode.ValidationFailure_PartnerInvitation, bl_dto.InvitedByUserId, $"Invitation cannot be Set since the InviteerFirstname is set to '{bl_dto.InviterFirstName}', which cannot be set");
            }

            var partnerInvitation = new PartnerInvitation
            {
                InvitationId = Guid.NewGuid().ToString("D").ToUpper(),
                DateOfBirth = bl_dto.DateOfBirth,
                FirstName = bl_dto.FirstName,
                InvitationStatus = PartnerInvitationStatusTypeEnum.Unknown, //Set below
                LastName = bl_dto.LastName,
                MobilePhone = new Phone { UserEnteredNr = bl_dto.MobilePhone },
                Gender = GenderTypeEnum.Unknown, //set below
            };

            //set/map enums
            partnerInvitation.InvitationStatus = PartnerInvitationStatusTypeEnum.Submitted; //since we only support creating new ones
            switch (bl_dto.TypeOfGender)
            {
                case BL_GenderTypeEnum.Female:
                    partnerInvitation.Gender = GenderTypeEnum.Female;
                    break;
                case BL_GenderTypeEnum.Male:
                    partnerInvitation.Gender = GenderTypeEnum.Male;
                    break;
                case BL_GenderTypeEnum.NotSpecified:
                    partnerInvitation.Gender = GenderTypeEnum.NotSpecified;
                    break;
                default:
                    partnerInvitation.Gender = GenderTypeEnum.Unknown;
                    break;
            }

            var existingUser = await _repo.GetUserProfile_byIdAsync(bl_dto.InvitedByUserId);
            if (existingUser == null)
            {
                throw new BusinessLogicException(ValidationErrorCode.EntityIsNull, bl_dto.InvitedByUserId);
            }

            if (validator.CanInvitePartner(existingUser, out var validationResult))
            {
                existingUser.PartnerInvitation = partnerInvitation;
                validationResult = validator.Validate(existingUser); //do full validation
            }

            if (validationResult.IsValid)
            {
                await GenerateAndCreateInvitationSmsAsync(existingUser);
                var idUpdatedOrCreated = await _repo.SetUserProfileAsync(existingUser);
                return idUpdatedOrCreated;
            }
            else
            {
                throw new BusinessLogicException(validationResult);
            }

        }

        public async Task GenerateAndCreateInvitationSmsAsync(string idOfUserInvitingPartner)
        {
            var invitingUserProfile = await _repo.GetUserProfile_byIdAsync(idOfUserInvitingPartner);
            await GenerateAndCreateInvitationSmsAsync(invitingUserProfile);
        }


        public async Task GenerateAndCreateInvitationSmsAsync(UserProfile invitingUserProfile)
        {
            //CanSendSmsInvitationToPartner
            if (validator.CanSendSmsInvitationToPartner(invitingUserProfile, out var validationResult))
            {
                var smsMsg = await GetPartnerInvitationSmsBodyAsync(invitingUserProfile.FirstName, invitingUserProfile.PartnerInvitation.InvitationId, map.FromGenderCharacter(invitingUserProfile.Gender));
                await _phoneService.PublishSmsDtoAsync(invitingUserProfile.PartnerInvitation.MobilePhone.PhoneNr, smsMsg);
            }
            else
            {
                throw new BusinessLogicException(validationResult);
            }

        }

        public async Task SetPartnerInvitationStatus(string invitationId, BL_PartnerInvitationStatusTypeEnum newInvitationStatus)
        {
            var userProfile = await _repo.GetUserProfile_byInvitationId(invitationId);
            if (validator.CanSetPartnerInviteStatus(userProfile, newInvitationStatus, out var validationResult))
            {
                switch (newInvitationStatus)
                {
                    case BL_PartnerInvitationStatusTypeEnum.Accepted:
                        userProfile.PartnerInvitation.InvitationStatus = PartnerInvitationStatusTypeEnum.Accepted;
                        break;
                    case BL_PartnerInvitationStatusTypeEnum.Declined:
                        userProfile.PartnerInvitation.InvitationStatus = PartnerInvitationStatusTypeEnum.Declined;
                        break;
                    case BL_PartnerInvitationStatusTypeEnum.Completed:
                        userProfile.PartnerInvitation.InvitationStatus = PartnerInvitationStatusTypeEnum.Completed;
                        break;
                    default:
                        validationResult.Failures.Add(new ValidationFailure
                        {
                            Code = ValidationErrorCode.ValidationFailure_PartnerInvitationCannotSetStatus,
                            DebugMessage = $"Invitation Status cannot be set. No mapping exists for Invitation Status from '{userProfile.PartnerInvitation.InvitationStatus}' to '{newInvitationStatus}'",
                            EntityJsonRepresentation = Validator.GetJson(userProfile),
                            EntityIdInError = userProfile.Id,
                        });
                        break;
                }
            }

            if (validationResult.IsValid)
            {
                userProfile.DTLastUpdated = DateTimeOffset.UtcNow;
                await _repo.SetUserProfileAsync(userProfile);
            }
            else
            {
                throw new BusinessLogicException(validationResult);
            }

        }

        public async Task<string> GetPartnerInvitationSmsBodyAsync(string firstName, string invitationId, BL_GenderTypeEnum gender)
        {
            var settings = await GetBusinessLogicSettings();
            string msg;
            switch (gender)
            {
                case BL_GenderTypeEnum.Female:
                    msg = settings.SmsTemplateForPartnerInvitationFemale;
                    break;
                case BL_GenderTypeEnum.Male:
                    msg = settings.SmsTemplateForPartnerInvitationMale;
                    break;
                case BL_GenderTypeEnum.NotSpecified:
                    msg = settings.SmsTemplateForPartnerInvitationNeutral;
                    break;
                default:
                    msg = settings.SmsTemplateForPartnerInvitationNeutral;
                    break;
            }

            return BL_Settings.ParseSmsTemplateForPartnerInvitation(msg, settings.PartnerInvitationUrl, firstName, invitationId);
        }


        public async Task<string> CreateInviteeUserAsync(string primaryEmail, string externalRefId, string firstName, string lastName, string invitationId, bool? hasAgreedToPrivacyPolicy, bool? hasAgreedToTandC)
        {
            var existingInvitation = await GetPartnerInvitation_byInvitationIdAsync(invitationId);
            if (existingInvitation == null)
            {
                throw new BusinessLogicException(ValidationErrorCode.NotFound, invitationId, $"Unable to find Invitation with Id '{invitationId}' for user with and Email: '{primaryEmail}'");
            }

            if (existingInvitation.InvitationStatus != BL_PartnerInvitationStatusTypeEnum.Accepted)
            {
                throw new BusinessLogicException(ValidationErrorCode.ValidationFailure_PartnerInvitation, invitationId, $"Invalid Invitation Status, cannot create user for Invitation with Id '{invitationId}' and Email '{primaryEmail}'");
            }

            if (hasAgreedToPrivacyPolicy != true)
            {
                throw new BusinessLogicException(ValidationErrorCode.ValidationFailure_UserProfile_PrivacyPolicy, invitationId, $"Privacy policy must be set to true for Invitation with Id '{invitationId}' and Email '{primaryEmail}'");
            }
            if (hasAgreedToTandC != true)
            {
                throw new BusinessLogicException(ValidationErrorCode.ValidationFailure_UserProfile_TermsOfService, invitationId, $"TandC policy must be set to true for Invitation with Id '{invitationId}' and Email '{primaryEmail}'");
            }

            //Get Inviter profile
            var inviter = await _repo.GetUserProfile_byInvitationId(invitationId);

            if (true == string.IsNullOrWhiteSpace(inviter.Relationship_Id))
            {
                throw new BusinessLogicException(ValidationErrorCode.ValidationFailure_UserProfile_RelationshipId, invitationId, $"Relationship must be set for Inviter with Id '{inviter.Id}' Invitation with Id '{invitationId}' and Email '{primaryEmail}'");
            }

            //Create new invitee user
            var newInviteeUser = await CreateUserAsync(primaryEmail, externalRefId, false, firstName, lastName, inviter.Relationship_Id);

            // Set details for the Invitee as entered by the Invitee in the web app (terms of use and privacy) and, 
            // details from the existingInvitation (the details entered by the Inviter) such as DoB and gender and, 
            // also firstname and last name if the invitee didn't enter them in during th ADB2C registration
            var bl_usrProReg = new BL_UserProfile
            {
                HasAgreedToPrivacyPolicy = hasAgreedToPrivacyPolicy,
                HasAgreedToTandC = hasAgreedToTandC,
                DateOfBirth = existingInvitation.DateOfBirth,
                FirstName = (string.IsNullOrEmpty(firstName) ? existingInvitation.FirstName : firstName),
                LastName = (string.IsNullOrEmpty(lastName) ? existingInvitation.LastName : lastName),
                PrimaryMobileNr = existingInvitation.MobilePhone,
                Gender = map.From(existingInvitation.TypeOfGender),

                Id = newInviteeUser.Id,
            };
            await SetUserAsync(bl_usrProReg);

            //Update invitationStatus of inviter  
            await SetPartnerInvitationStatus(invitationId, BL_PartnerInvitationStatusTypeEnum.Completed);

            return newInviteeUser.Id;
        }

    }
}
