using System;
using Newtonsoft.Json;

namespace LifeCouple.DAL.Entities
{
    public class PartnerInvitation 
    {
        /// <summary>
        /// Used to identify a given invitation, used when generating the notification so it can be linked back to the instance of the invitation
        /// </summary>
        [JsonProperty("invitationId")]
        public string InvitationId { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "mobilePhone")]
        public Phone MobilePhone { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public GenderTypeEnum Gender { get; set; }

        [JsonProperty(PropertyName = "dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty(PropertyName = "invitationStatus")]
        public PartnerInvitationStatusTypeEnum InvitationStatus { get; set; }
    }
}
