using System.Collections.Generic;
using Newtonsoft.Json;

namespace LifeCouple.DAL.Entities
{
    public class BusinessLogicSettings : CosmosDbEntity
    {

        [JsonProperty("version")]
        public int? Version { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("smsTemplateForPartnerInvitationFemale")]
        public string SmsTemplateForPartnerInvitationFemale { get; set; }

        [JsonProperty("smsTemplateForPartnerInvitationMale")]
        public string SmsTemplateForPartnerInvitationMale { get; set; }

        [JsonProperty("smsTemplateForPartnerInvitationNeutral")]
        public string SmsTemplateForPartnerInvitationNeutral { get; set; }

        [JsonProperty("partnerInvitationUrl")]
        public string PartnerInvitationUrl { get; set; }

        /// <summary>
        /// Dictionary of dictionaries with first level handling the Point Value and second level handling the Language-Locale and the Description, blank language-locale = neutral/fallback, and initially only what is supported
        /// </summary>
        [JsonProperty("emotionalBankAccontPointOptions")]
        public Dictionary<int, Dictionary<string, string>> EmotionalBankAccontPointOptions { get; set; }

        [JsonProperty("appCenterPushNotificationApiUrls")]
        public Dictionary<DeviceOsTypeEnum, string> AppCenterPushNotificationApiUrls { get; set; }

        [JsonProperty("appCenterPushNotificationMessageTypes")]
        public Dictionary<PushNotificationMessageTypeEnum, PushtNotificationDetails> AppCenterPushNotificationMessageTypes { get; set; }

        public class PushtNotificationDetails
        {
            public string CustomData_Page { get; set; }
            public string CustomData_UserData { get; set; }

            /// <summary>
            /// Dictionary with the Language-Locale and MessageTemplate, blank language-locale = neutral/fallback, and initially only what is supported
            /// </summary>
            public Dictionary<string, PushNotificationMessageTemplate> TemplatePerLanguageLocale { get; set; }
        }

        public class PushNotificationMessageTemplate
        {
            public string BodyTemplate { get; set; }
            public string TitleTemplate { get; set; }
        }
    }
}
