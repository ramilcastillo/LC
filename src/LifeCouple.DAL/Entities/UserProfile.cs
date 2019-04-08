using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeCouple.DAL.Entities
{
    [Table("UserProfile")]
    public class UserProfile : CosmosDbEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty("id")]
        public string Id { get; set; }

        [Required]
        [JsonProperty("dtCreated")]
        public DateTimeOffset DTCreated { get; set; }

        [Required]
        [JsonProperty("dtLastUpdated")]
        public DateTimeOffset DTLastUpdated { get; set; }

        [MaxLength(50)]
        [JsonProperty("relationship_id")]
        public string Relationship_Id { get; set; }


        /// <summary>
        /// Note, could be more than 1 user with the same primary email...
        /// </summary>
        [Required]
        [MaxLength(254)]
        [JsonProperty("primaryEmail")]
        public string PrimaryEmail { get; set; }

        /// <summary>
        /// To include the full nr including Country Code
        /// </summary>
        [MaxLength(30)]
        [JsonProperty("primaryMobileNr")]
        public string PrimaryMobileNr { get; set; }

        [MaxLength(2)]
        [JsonProperty("countryCode")]
        public string Country_Code { get; set; }

        /// <summary>
        /// Typically 'oid' (or objectidentifier) from external IdP like Active Directory B2C, see also  https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-token-and-claims
        /// </summary>
        [Required]
        [MaxLength(50)]
        [JsonProperty("externalRefId")]
        public string ExternalRefId { get; set; }

        /// <summary>
        /// Flag to indicate if user is created for devtest purposes (created through the web api instead of in ADB2C)
        /// </summary>
        [Required]
        [JsonProperty("isDevTest")]
        public bool IsDevTest { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(1)]
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [MaxLength(100)]
        [MinLength(1)]
        [Required]
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// e=email, s=sms, p=pushNotification, e.g. "ep" = email and push, "" = no notifications, "p" = pushNotifications
        /// </summary>
        [Required]
        [MaxLength(10)]
        [JsonProperty("notificationOption")]
        public string NotificationOption { get; set; }

        /// <summary>
        /// m=male, f=female
        /// </summary>
        [Required]
        [MaxLength(1)]
        [RegularExpression(@"^[mf''-'\s]{1,1}$")]
        [JsonProperty("gender")]
        public string Gender { get; set; }

        [Required]
        [JsonProperty(PropertyName = "dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        //Payment
        [JsonProperty(PropertyName = "hasAgreedToTandC")]
        public bool? HasAgreedToTandC { get; set; }

        [JsonProperty(PropertyName = "hasAgreedToPrivacyPolicy")]
        public bool? HasAgreedToPrivacyPolicy { get; set; }

        [JsonProperty(PropertyName = "hasAgreedToRefundPolicy")]
        public bool? HasAgreedToRefundPolicy { get; set; }

        [JsonProperty(PropertyName = "indexes")]
        public List<Index> Indexes { get; set; }

        public class Index
        {
            [JsonProperty(PropertyName = "typeOfIndex")]
            public IndexTypeEnum TypeOfIndex { get; set; }

            [JsonProperty(PropertyName = "value")]
            public int Value { get; set; }

            [JsonProperty("dtLastUpdated")]
            public DateTimeOffset DTLastUpdated { get; set; }
        }

        [JsonProperty(PropertyName = "partnerInvitation")]
        public PartnerInvitation PartnerInvitation { get; set; }
        
    }
}
