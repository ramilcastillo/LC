using System.Collections.Generic;
using LifeCouple.WebApi.Common;

namespace LifeCouple.WebApi.DomainLogic.BL_DTOs
{
    /// <summary>
    /// Used for Business Logic settings data that does not change often, and does NOT include and credentials or access keys
    /// </summary>
    public class BL_Settings
    {
        public int? Version { get; set; }

        public string SmsTemplateForPartnerInvitationFemale { get; set; }

        public string SmsTemplateForPartnerInvitationMale { get; set; }

        public string SmsTemplateForPartnerInvitationNeutral { get; set; }

        public string PartnerInvitationUrl { get; set; }

        /// <summary>
        /// Dictionary of dictionaries with first level handling the Point Value and second level handling the Language-Locale and the Description, blank language-locale = neutral/fallback, and initially only that is supported
        /// </summary>
        public Dictionary<int, Dictionary<string, string>> EmotionalBankAccontPointOptions { get; set; }

        public Dictionary<BL_DeviceOsTypeEnum, string> AppCenterPushNotificationApiUrls { get; set; }

        public Dictionary<BL_PushNotificationMessageTypeEnum, BL_PushtNotificationDetails> AppCenterPushNotificationMessageTypes { get; set; }

        public class BL_PushtNotificationDetails
        {
            public string CustomData_Page { get; set; }
            public string CustomData_UserData { get; set; }

            /// <summary>
            /// Dictionary with the Language-Locale and MessageTemplate, blank language-locale = neutral/fallback, and initially only what is supported
            /// </summary>
            public Dictionary<string, BL_PushNotificationMessageTemplate> TemplatePerLanguageLocale { get; set; }
        }

        public class BL_PushNotificationMessageTemplate
        {
            public string BodyTemplate { get; set; }
            public string TitleTemplate { get; set; }
        }

        public static readonly int CurrentVersion = 20180918;
        public static readonly string FirstNameToken = "{FirstName}";
        public static readonly string InvitationIdToken = "{InvitationId}";
        public static readonly string PushNotication_CustomData_UserData_FirstNameToken = "{FirstName}";
        public static readonly string PartnerInvitationUrlToken = "{" + nameof(PartnerInvitationUrl) + "}";

        public static BL_Settings GetDefaultValues()
        {
            var webAppServer = "toBeDefined.azurewebsites.net";
            if (AppInfoModel.Current.GetEnvironment() == "Production")
            {
                webAppServer = "webapplcprod.azurewebsites.net";
            }
            else if (AppInfoModel.Current.GetEnvironment() == "lcapicitest")
            {
                webAppServer = "webapplctest.azurewebsites.net";
            }

            var values = new BL_Settings
            {
                PartnerInvitationUrl = $"https://{webAppServer}/pi/lp?id={BL_Settings.InvitationIdToken}",
                SmsTemplateForPartnerInvitationFemale = $"{BL_Settings.FirstNameToken} joined LifeCouple and invited you to join her - {BL_Settings.PartnerInvitationUrlToken}",
                SmsTemplateForPartnerInvitationMale = $"{BL_Settings.FirstNameToken} joined LifeCouple and invited you to join him - {BL_Settings.PartnerInvitationUrlToken}",
                SmsTemplateForPartnerInvitationNeutral = $"{BL_Settings.FirstNameToken} joined LifeCouple and invited you to join - {BL_Settings.PartnerInvitationUrlToken}",
                Version = CurrentVersion,
                EmotionalBankAccontPointOptions = new Dictionary<int, Dictionary<string, string>>()
                {
                    {
                        25, new Dictionary<string, string>
                        {
                            { "", "25 Points" } //Neutral/fallback language locale
                        }
                    },
                    {
                        50, new Dictionary<string, string>
                        {
                            { "", "50 Points" } //Neutral/fallback language locale
                        }
                    },
                    {
                        100, new Dictionary<string, string>
                        {
                            { "", "100 Points" } //Neutral/fallback language locale
                        }
                    }
                }
            };

            values.AppCenterPushNotificationApiUrls = new Dictionary<BL_DeviceOsTypeEnum, string> {
                { BL_DeviceOsTypeEnum.Android, "https://api.appcenter.ms/v0.1/apps/LifeCouple/LifeCouple.Mobile-1/push/notifications" },
                { BL_DeviceOsTypeEnum.IOs, "https://api.appcenter.ms/v0.1/apps/LifeCouple/LifeCouple.Mobile/push/notifications"}
            };

            values.AppCenterPushNotificationMessageTypes = new Dictionary<BL_PushNotificationMessageTypeEnum, BL_PushtNotificationDetails>
            {
                {
                    BL_PushNotificationMessageTypeEnum.Type_EBA_Points_Sent, new BL_PushtNotificationDetails{
                        CustomData_UserData = BL_Settings.PushNotication_CustomData_UserData_FirstNameToken,
                        CustomData_Page = "EmotionalBankAccountPage",
                        TemplatePerLanguageLocale = new Dictionary<string, BL_PushNotificationMessageTemplate>
                        {
                            {
                                "", new BL_PushNotificationMessageTemplate { BodyTemplate = "You have received a deposit in your Emotional Bank Account.", TitleTemplate = null }
                            }
                        }
                    }
                }
            };

            return values;
        }

        /// <summary>
        /// Message template based on https://lifecouple.visualstudio.com/LC90/_workitems/edit/87
        /// </summary>
        /// <param name="template"></param>
        /// <param name="firstName"></param>
        /// <param name="InvitationId"></param>
        /// <returns></returns>
        public static string ParseSmsTemplateForPartnerInvitation(string template, string partnerInvitationUrl, string firstName, string InvitationId) => template.Replace("{" + nameof(PartnerInvitationUrl) + "}", partnerInvitationUrl).Replace(InvitationIdToken, InvitationId).Replace(FirstNameToken, firstName);

        public static string ParsePushNotificationCustomData_UserData(string template, string firstNameValue) => template.Replace(BL_Settings.PushNotication_CustomData_UserData_FirstNameToken, firstNameValue);
    }
}
