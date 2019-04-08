using System;
using System.Collections.Generic;
using LifeCouple.DAL.Entities;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;

namespace LifeCouple.WebApi.DomainLogic
{
    public class Map
    {
        public BL_UserProfile From(UserProfile userProfile)
        {
            if (userProfile == null)
            {
                return null;
            }

            var r = new BL_UserProfile
            {
                Id = userProfile.Id,
                Country_Code = userProfile.Country_Code,
                DateOfBirth = userProfile.DateOfBirth,
                ExternalRefId = userProfile.ExternalRefId,
                FirstName = userProfile.FirstName,
                Gender = userProfile.Gender,
                HasAgreedToPrivacyPolicy = userProfile.HasAgreedToPrivacyPolicy,
                HasAgreedToRefundPolicy = userProfile.HasAgreedToRefundPolicy,
                HasAgreedToTandC = userProfile.HasAgreedToTandC,
                LastName = userProfile.LastName,
                NotificationOption = userProfile.NotificationOption,
                PrimaryEmail = userProfile.PrimaryEmail,
                PrimaryMobileNr = userProfile.PrimaryMobileNr,
                Relationship_Id = userProfile.Relationship_Id,
                Indexes = getIndexes(),
                PartnerInvitationStatus = getInvitationStatus()
            };

            List<BL_UserProfile.Index> getIndexes()
            {
                if (userProfile.Indexes?.Count > 0)
                {
                    var index = new List<BL_UserProfile.Index>();
                    foreach (var i in userProfile.Indexes)
                    {
                        index.Add(new BL_UserProfile.Index { TypeOfIndex = i.TypeOfIndex, Value = i.Value });
                    }
                    return index;
                }
                else
                {
                    return null;
                }

            }

            BL_PartnerInvitationStatusTypeEnum getInvitationStatus()
            {
                if (userProfile.PartnerInvitation == null)
                {
                    return BL_PartnerInvitationStatusTypeEnum.Unknown;
                }
                else
                {
                    switch (userProfile.PartnerInvitation.InvitationStatus)
                    {
                        case PartnerInvitationStatusTypeEnum.Accepted:
                            return BL_PartnerInvitationStatusTypeEnum.Accepted;
                        case PartnerInvitationStatusTypeEnum.Completed:
                            return BL_PartnerInvitationStatusTypeEnum.Completed;
                        case PartnerInvitationStatusTypeEnum.Declined:
                            return BL_PartnerInvitationStatusTypeEnum.Declined;
                        case PartnerInvitationStatusTypeEnum.NotApplicable:
                            return BL_PartnerInvitationStatusTypeEnum.NotApplicable;
                        case PartnerInvitationStatusTypeEnum.Sent:
                            return BL_PartnerInvitationStatusTypeEnum.Sent;
                        case PartnerInvitationStatusTypeEnum.Submitted:
                            return BL_PartnerInvitationStatusTypeEnum.Submitted;
                        case PartnerInvitationStatusTypeEnum.Unknown:
                            return BL_PartnerInvitationStatusTypeEnum.Unknown;
                        default:
                            throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum {userProfile.PartnerInvitation.InvitationStatus} to BL value");
                    }
                }

            }

            return r;
        }

        internal BL_DeviceOsTypeEnum From(DeviceOsTypeEnum key)
        {
            switch (key)
            {
                case DeviceOsTypeEnum.Android:
                    return BL_DeviceOsTypeEnum.Android;
                case DeviceOsTypeEnum.IOs:
                    return BL_DeviceOsTypeEnum.IOs;
                case DeviceOsTypeEnum.Unknown:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type {nameof(DeviceOsTypeEnum)} with with value '{key}' to value of type {nameof(BL_DeviceOsTypeEnum)}");
                default:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type {nameof(DeviceOsTypeEnum)} with with value '{key}' to value of type {nameof(BL_DeviceOsTypeEnum)}");
            }
        }

        internal BL_PushNotificationMessageTypeEnum From(PushNotificationMessageTypeEnum key)
        {
            switch (key)
            {
                case PushNotificationMessageTypeEnum.Type_EBA_Points_Sent:
                    return BL_PushNotificationMessageTypeEnum.Type_EBA_Points_Sent;
                case PushNotificationMessageTypeEnum.Unknown:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type {nameof(PushNotificationMessageTypeEnum)} with with value '{key}' to value of type {nameof(BL_PushNotificationMessageTypeEnum)}");
                default:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type {nameof(PushNotificationMessageTypeEnum)} with with value '{key}' to value of type {nameof(BL_PushNotificationMessageTypeEnum)}");
            }
        }


        public BL_PartnerInvitation From_ForPartnerInvitation(UserProfile userProfile)
        {
            if (userProfile?.PartnerInvitation == null)
            {
                return null;
            }

            var r = new BL_PartnerInvitation
            {
                InvitedByUserId = userProfile.Id,
                MobilePhone = userProfile.PartnerInvitation.MobilePhone.UserEnteredNr,
                DateOfBirth = userProfile.PartnerInvitation.DateOfBirth,
                FirstName = userProfile.PartnerInvitation.FirstName,
                TypeOfGender = getGender(),
                LastName = userProfile.PartnerInvitation.LastName,
                InvitationStatus = getInvitationStatus(),
                InvitationId = userProfile.PartnerInvitation.InvitationId,
                InviterFirstName = userProfile.FirstName
            };

            BL_GenderTypeEnum getGender()
            {
                switch (userProfile.PartnerInvitation.Gender)
                {
                    case GenderTypeEnum.Female:
                        return BL_GenderTypeEnum.Female;
                    case GenderTypeEnum.Male:
                        return BL_GenderTypeEnum.Male;
                    case GenderTypeEnum.NotSpecified:
                        return BL_GenderTypeEnum.NotSpecified;
                    default:
                        return BL_GenderTypeEnum.Unknown;
                }
            }

            BL_PartnerInvitationStatusTypeEnum getInvitationStatus()
            {
                switch (userProfile.PartnerInvitation.InvitationStatus)
                {
                    case PartnerInvitationStatusTypeEnum.Accepted:
                        return BL_PartnerInvitationStatusTypeEnum.Accepted;
                    case PartnerInvitationStatusTypeEnum.Declined:
                        return BL_PartnerInvitationStatusTypeEnum.Declined;
                    case PartnerInvitationStatusTypeEnum.Sent:
                        return BL_PartnerInvitationStatusTypeEnum.Sent;
                    case PartnerInvitationStatusTypeEnum.Submitted:
                        return BL_PartnerInvitationStatusTypeEnum.Submitted;
                    default:
                        return BL_PartnerInvitationStatusTypeEnum.Unknown;
                }

            }

            return r;
        }

        internal PushNotificationMessageTypeEnum From(BL_PushNotificationMessageTypeEnum key)
        {
            switch (key)
            {
                case BL_PushNotificationMessageTypeEnum.Type_EBA_Points_Sent:
                    return PushNotificationMessageTypeEnum.Type_EBA_Points_Sent;
                case BL_PushNotificationMessageTypeEnum.Unknown:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type {nameof(BL_PushNotificationMessageTypeEnum)} with with value '{key}' to value of type {nameof(PushNotificationMessageTypeEnum)}");
                default:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type {nameof(BL_PushNotificationMessageTypeEnum)} with with value '{key}' to value of type {nameof(PushNotificationMessageTypeEnum)}");
            }
        }

        internal DeviceOsTypeEnum From(BL_DeviceOsTypeEnum key)
        {
            switch (key)
            {
                case BL_DeviceOsTypeEnum.Android:
                    return DeviceOsTypeEnum.Android;
                case BL_DeviceOsTypeEnum.IOs:
                    return DeviceOsTypeEnum.IOs;
                case BL_DeviceOsTypeEnum.Unknown:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type {nameof(BL_DeviceOsTypeEnum)} with with value '{key}' to value of type {nameof(DeviceOsTypeEnum)}");
                default:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type {nameof(BL_DeviceOsTypeEnum)} with with value '{key}' to value of type {nameof(DeviceOsTypeEnum)}");
            }
        }

        public BL_Relationship From(Relationship relationship)
        {
            if (relationship == null)
            {
                return null;
            }

            return new BL_Relationship
            {
                BeenToCounseler = Enum.Parse<BL_Relationship.CounselerStatusEnum>(relationship.BeenToCounseler.ToString()),
                CurrentWeddingDate = relationship.CurrentWeddingDate,
                Id = relationship.Id,
                MarriageStatus = Enum.Parse<BL_Relationship.MarriageStatusEnum>(relationship.MarriageStatus.ToString()),
                NrOfChildren = relationship.NrOfChildren,
                NrOfStepChildren = relationship.NrOfStepChildren,
                RegisteredPartner_Id = relationship.RegisteredPartner_Id,
                RelationshipStatus = Enum.Parse<BL_Relationship.RelationshipStatusEnum>(relationship.RelationshipStatus.ToString()),
            };
        }

        public Relationship From(BL_Relationship e)
        {
            if (e == null)
            {
                return null;
            }

            return new Relationship
            {
                BeenToCounseler = (e.BeenToCounseler.HasValue ? Enum.Parse<CounselerStatusEnum>(e.BeenToCounseler?.ToString()) : CounselerStatusEnum.Unknown),
                CurrentWeddingDate = e.CurrentWeddingDate,
                Id = e.Id,
                MarriageStatus = (e.MarriageStatus.HasValue ? Enum.Parse<MarriageStatusEnum>(e.MarriageStatus?.ToString()) : MarriageStatusEnum.Unknown),
                NrOfChildren = e.NrOfChildren,
                NrOfStepChildren = e.NrOfStepChildren,
                RegisteredPartner_Id = e.RegisteredPartner_Id,
                RelationshipStatus = (e.RelationshipStatus.HasValue ? Enum.Parse<RelationshipStatusEnum>(e.RelationshipStatus?.ToString()) : RelationshipStatusEnum.Unknown),
            };
        }

        public BL_UserProfileRegistration From(UserProfileRegistration userProfileRegistration)
        {
            if (userProfileRegistration == null)
            {
                return null;
            }

            return new BL_UserProfileRegistration
            {
                PartnerDateOfBirth = userProfileRegistration.PartnerDateOfBirth,
                PartnerEmail = userProfileRegistration.PartnerEmail,
                PartnerFirstName = userProfileRegistration.PartnerFirstName,
                PartnerGender = userProfileRegistration.PartnerGender,
                PartnerLastName = userProfileRegistration.PartnerLastName,
                PartnerMobilePhone = userProfileRegistration.PartnerMobilePhone,
                UserProfile_Id = userProfileRegistration.UserProfile_Id,
            };
        }

        public string From(BL_GenderTypeEnum genderTypeEnum)
        {
            switch (genderTypeEnum)
            {
                case BL_GenderTypeEnum.Female:
                    return "f";
                case BL_GenderTypeEnum.Male:
                    return "m";
                case BL_GenderTypeEnum.NotSpecified:
                    return null;
                case BL_GenderTypeEnum.Unknown:
                    return null;
                default:
                    throw new NotSupportedException($"{nameof(BL_GenderTypeEnum)} with value '{genderTypeEnum}' can not be mapped to a string representation");
            }
        }

        public BL_GenderTypeEnum FromGenderCharacter(string genderCharacter)
        {
            if (genderCharacter == "f")
            {
                return BL_GenderTypeEnum.Female;
            }
            else if (genderCharacter == "m")
            {
                return BL_GenderTypeEnum.Male;
            }
            else
            {
                return BL_GenderTypeEnum.NotSpecified;
            }

        }


    }
}
