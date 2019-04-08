using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using LifeCouple.WebApi.DomainLogic;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;

namespace LifeCouple.WebApi.v.Controllers
{
    public class EnumMapper
    {
        public static PartnerInvitationStatusType From(BL_PartnerInvitationStatusTypeEnum bl_PartnerInvitationStatusTypeEnum)
        {
            {
                switch (bl_PartnerInvitationStatusTypeEnum)
                {
                    case BL_PartnerInvitationStatusTypeEnum.Accepted:
                        return PartnerInvitationStatusType.Accepted;
                    case BL_PartnerInvitationStatusTypeEnum.Completed:
                        return PartnerInvitationStatusType.Completed;
                    case BL_PartnerInvitationStatusTypeEnum.Declined:
                        return PartnerInvitationStatusType.Declined;
                    case BL_PartnerInvitationStatusTypeEnum.NotApplicable:
                        return PartnerInvitationStatusType.Completed;
                    case BL_PartnerInvitationStatusTypeEnum.Sent:
                        return PartnerInvitationStatusType.Sent;
                    case BL_PartnerInvitationStatusTypeEnum.Submitted:
                        return PartnerInvitationStatusType.Submitted;
                    case BL_PartnerInvitationStatusTypeEnum.Unknown:
                        return PartnerInvitationStatusType.Unknown;
                    default:
                        throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type '{nameof(bl_PartnerInvitationStatusTypeEnum)}' with value '{bl_PartnerInvitationStatusTypeEnum}' to dto value");
                }
            }
        }

        public static EbaTransactionType From(BL_EbaTransactionTypeEnum bL_EbaTransactionType)
        {
            switch (bL_EbaTransactionType)
            {
                case BL_EbaTransactionTypeEnum.Received:
                    return EbaTransactionType.Received;
                case BL_EbaTransactionTypeEnum.Sent:
                    return EbaTransactionType.Sent;
                case BL_EbaTransactionTypeEnum.Unknown:
                    return EbaTransactionType.Unknown;
                default:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type '{nameof(bL_EbaTransactionType)}' with value '{bL_EbaTransactionType}' to dto value");

            }
        }

        public static GenderType From(BL_GenderTypeEnum bl_GenderTypeEnum)
        {
            {
                switch (bl_GenderTypeEnum)
                {
                    case BL_GenderTypeEnum.Female:
                        return GenderType.Female;
                    case BL_GenderTypeEnum.Male:
                        return GenderType.Male;
                    case BL_GenderTypeEnum.NotSpecified:
                        return GenderType.NotSpecified;
                    default:
                        return GenderType.Unknown;
                }
            }
        }

        public static BL_GenderTypeEnum From(GenderType genderType)
        {
            {
                switch (genderType)
                {
                    case GenderType.Female:
                        return BL_GenderTypeEnum.Female;
                    case GenderType.Male:
                        return BL_GenderTypeEnum.Male;
                    case GenderType.NotSpecified:
                        return BL_GenderTypeEnum.NotSpecified;
                    default:
                        return BL_GenderTypeEnum.Unknown;
                }
            }
        }

        public static BL_DeviceOsTypeEnum From(DeviceOsType deviceOsType)
        {
            switch (deviceOsType)
            {
                case DeviceOsType.Android:
                    return BL_DeviceOsTypeEnum.Android;
                case DeviceOsType.Ios:
                    return BL_DeviceOsTypeEnum.IOs;
                case DeviceOsType.Unknown:
                    return BL_DeviceOsTypeEnum.Unknown;
                default:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unable to map enum of type '{nameof(deviceOsType)}' with value '{deviceOsType}'");

            }
        }
    }
}
