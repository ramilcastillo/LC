using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeCouple.DAL.Entities
{
    [Table("UserProfileRegistration")]
    public class UserProfileRegistration : CosmosDbEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty(PropertyName = "id")]
        public string UserProfile_Id { get; set; }


        //About Your Partner
        [Required]
        [MaxLength(100)]
        [MinLength(1)]
        [JsonProperty(PropertyName = "partnerFirstName")]
        public string PartnerFirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(1)]
        [JsonProperty(PropertyName = "partnerLastName")]
        public string PartnerLastName { get; set; }

        [Required]
        [MaxLength(254)]
        [EmailAddress]
        [JsonProperty(PropertyName = "partnerEmail")]
        public string PartnerEmail { get; set; }

        [Required]
        [MaxLength(30)]
        [Phone]
        [JsonProperty(PropertyName = "partnerMobilePhone")]
        public string PartnerMobilePhone { get; set; }

        [Required]
        [RegularExpression(@"^[mf''-'\s]{1,1}$")]
        [MaxLength(1)]
        [JsonProperty(PropertyName = "partnerGender")]
        public string PartnerGender { get; set; }

        [Required]
        [JsonProperty(PropertyName = "partnerDateOfBirth")]
        public DateTime? PartnerDateOfBirth { get; set; }
  
    }
}
