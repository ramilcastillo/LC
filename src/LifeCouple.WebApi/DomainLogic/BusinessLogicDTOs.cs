using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LifeCouple.DAL.Entities;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;

/// <summary>
/// DTOs that is exposed from and to the Business Logic layer
/// </summary>
namespace LifeCouple.WebApi.DomainLogic
{

    public class BL_UserProfile
    {
        public string Id { get; set; }

        //[Required]
        //public DateTimeOffset DTCreated { get; set; }

        //[Required]
        //public DateTimeOffset DTLastUpdated { get; set; }

        [Required]
        [MaxLength(254)]
        public string PrimaryEmail { get; set; }

        /// <summary>
        /// To include the full nr including Country Code
        /// </summary>
        [MaxLength(30)]
        public string PrimaryMobileNr { get; set; }

        [MaxLength(2)]
        public string Country_Code { get; set; }

        /// <summary>
        /// Typically 'oid' (or objectidentifier) from external IdP like Active Directory B2C, see also  https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-token-and-claims
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ExternalRefId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NotificationOption { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(50)]
        public string Relationship_Id { get; set; }

        //Payment
        public bool? HasAgreedToTandC { get; set; }
        public bool? HasAgreedToPrivacyPolicy { get; set; }
        public bool? HasAgreedToRefundPolicy { get; set; }

        public List<Index> Indexes { get; set; }

        public class Index
        {
            public IndexTypeEnum TypeOfIndex { get; set; }
            public int Value { get; set; }
        }

        public void Validate()
        {
            if (!new EmailAddressAttribute().IsValid(this.PrimaryEmail))
            {
                throw new ApplicationException("Email address is not a valid format");
            }

        }

        public BL_PartnerInvitationStatusTypeEnum PartnerInvitationStatus { get; set; }

        public bool IsRegistrationCompleted() => (HasAgreedToPrivacyPolicy == true && HasAgreedToTandC == true);

        public bool IsOnboardingQuesitonnaireCompleted() => (Indexes != null && Indexes.Count == 5);
    }


    public class BL_Relationship
    {
        public string Id { get; set; }

        [Required]
        public RelationshipStatusEnum? RelationshipStatus { get; set; }

        [Required]
        public MarriageStatusEnum? MarriageStatus { get; set; }

        public DateTime? CurrentWeddingDate { get; set; }

        public int? NrOfChildren { get; set; }

        public int? NrOfStepChildren { get; set; }

        [Required]
        public CounselerStatusEnum? BeenToCounseler { get; set; }

        [Required]
        public string RegisteredPartner_Id { get; set; }

        public enum RelationshipStatusEnum
        {
            Unknown = 0,
            Married = 1,
            NotMarried = 2,
        }

        public enum MarriageStatusEnum
        {
            Unknown = 0,
            FirstMarriage = 1,
            MoreThanOneMarriage = 99,
        }

        public enum CounselerStatusEnum
        {
            Unknown = 0,
            NoNever = 1,
            YesInThePast = 2,
            YesCurrently = 3,
        }
    }


    public class BL_UserProfileRegistration
    {
        //About You
        public string UserProfile_Id { get; set; }

        //About Your Partner
        public string PartnerFirstName { get; set; }
        public string PartnerLastName { get; set; }
        public string PartnerEmail { get; set; }
        public string PartnerMobilePhone { get; set; }
        public string PartnerGender { get; set; }
        public DateTime? PartnerDateOfBirth { get; set; }


    }
}
