using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeCouple.DAL.Entities;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;

namespace LifeCouple.WebApi.DomainLogic
{
    public partial class BusinessLogic
    {
        private static BL_Settings bl_settingsCached;
        private static DateTime bl_settingsLastLoaded = DateTime.Now;

        public async Task<BL_Settings> GetBusinessLogicSettings()
        {
            if (bl_settingsCached == null || bl_settingsLastLoaded.Add(cacheExpiration) < DateTime.Now)
            {
                var businessLogicSettings = await _repo.GetBusinessLogicSettingsAsync();

                if (businessLogicSettings == null)
                {
                    bl_settingsCached = null;
                }
                else
                {
                    bl_settingsCached = new BL_Settings
                    {
                        AppCenterPushNotificationApiUrls = mapAppCenterUrls(businessLogicSettings.AppCenterPushNotificationApiUrls),
                        AppCenterPushNotificationMessageTypes = mapAppCenterPushNotificationMessageType(businessLogicSettings.AppCenterPushNotificationMessageTypes),
                        EmotionalBankAccontPointOptions = businessLogicSettings.EmotionalBankAccontPointOptions,
                        PartnerInvitationUrl = businessLogicSettings.PartnerInvitationUrl,
                        SmsTemplateForPartnerInvitationFemale = businessLogicSettings.SmsTemplateForPartnerInvitationFemale,
                        SmsTemplateForPartnerInvitationMale = businessLogicSettings.SmsTemplateForPartnerInvitationMale,
                        SmsTemplateForPartnerInvitationNeutral = businessLogicSettings.SmsTemplateForPartnerInvitationNeutral,
                        Version = businessLogicSettings.Version
                    };
                }
            }

            return bl_settingsCached;

            //local methods:
            Dictionary<BL_DeviceOsTypeEnum, string> mapAppCenterUrls(Dictionary<DeviceOsTypeEnum, string> appCenterUrls)
            {
                var r = new Dictionary<BL_DeviceOsTypeEnum, string>();

                if (appCenterUrls == null)
                {
                    return null;
                }

                foreach (var item in appCenterUrls)
                {
                    r.Add(map.From(item.Key), item.Value);
                }

                return r;
            }

            Dictionary<BL_PushNotificationMessageTypeEnum, BL_Settings.BL_PushtNotificationDetails> mapAppCenterPushNotificationMessageType(Dictionary<PushNotificationMessageTypeEnum, BusinessLogicSettings.PushtNotificationDetails> appCenterPushNotifcationMessageTypes)
            {
                var r = new Dictionary<BL_PushNotificationMessageTypeEnum, BL_Settings.BL_PushtNotificationDetails>();

                if(appCenterPushNotifcationMessageTypes == null)
                {
                    return null;
                }

                foreach (var item in appCenterPushNotifcationMessageTypes)
                {
                    r.Add(map.From(item.Key), new BL_Settings.BL_PushtNotificationDetails
                    {
                        CustomData_UserData = item.Value.CustomData_UserData,
                        CustomData_Page = item.Value.CustomData_Page,
                        TemplatePerLanguageLocale = mapPushNotificationTemplate(item.Value.TemplatePerLanguageLocale),
                    });
                }
                return r;
            }

            Dictionary<string, BL_Settings.BL_PushNotificationMessageTemplate> mapPushNotificationTemplate(Dictionary<string, BusinessLogicSettings.PushNotificationMessageTemplate> appCenterPushNotificationTemplates)
            {
                var r = new Dictionary<string, BL_Settings.BL_PushNotificationMessageTemplate>();

                foreach (var item in appCenterPushNotificationTemplates)
                {
                    r.Add(item.Key, new BL_Settings.BL_PushNotificationMessageTemplate { BodyTemplate = item.Value.BodyTemplate, TitleTemplate = item.Value.TitleTemplate });
                }

                return r;
            }

        }

        public async Task<string> SetSettings(BL_Settings bL_Settings)
        {
            var existingSettings = await _repo.GetBusinessLogicSettingsAsync();

            if (existingSettings == null)
            {
                existingSettings = new DAL.Entities.BusinessLogicSettings();
            }

            existingSettings.AppCenterPushNotificationApiUrls = mapAppCenterUrls(bL_Settings.AppCenterPushNotificationApiUrls) ?? existingSettings.AppCenterPushNotificationApiUrls;
            existingSettings.AppCenterPushNotificationMessageTypes = mapAppCenterPushNotificationMessageType(bL_Settings.AppCenterPushNotificationMessageTypes) ?? existingSettings.AppCenterPushNotificationMessageTypes;
            existingSettings.EmotionalBankAccontPointOptions = bL_Settings.EmotionalBankAccontPointOptions ?? existingSettings.EmotionalBankAccontPointOptions;
            existingSettings.PartnerInvitationUrl = bL_Settings.PartnerInvitationUrl ?? existingSettings.PartnerInvitationUrl;
            existingSettings.SmsTemplateForPartnerInvitationFemale = bL_Settings.SmsTemplateForPartnerInvitationFemale ?? existingSettings.SmsTemplateForPartnerInvitationFemale;
            existingSettings.SmsTemplateForPartnerInvitationMale = bL_Settings.SmsTemplateForPartnerInvitationMale ?? existingSettings.SmsTemplateForPartnerInvitationMale;
            existingSettings.SmsTemplateForPartnerInvitationNeutral = bL_Settings.SmsTemplateForPartnerInvitationNeutral ?? existingSettings.SmsTemplateForPartnerInvitationNeutral;
            existingSettings.Version = bL_Settings.Version ?? existingSettings.Version;

            var id = await _repo.SetBusinessLogicSettingsAsync(existingSettings);

            //Clear cache
            bl_settingsCached = null;

            return id;

            //local methods:
            Dictionary<DeviceOsTypeEnum, string> mapAppCenterUrls(Dictionary<BL_DeviceOsTypeEnum, string> appCenterUrls)
            {
                if(appCenterUrls == null)
                {
                    return null;
                }

                var r = new Dictionary<DeviceOsTypeEnum, string>();

                foreach (var item in appCenterUrls)
                {
                    r.Add(map.From(item.Key), item.Value);
                }

                return r;
            }

            Dictionary<PushNotificationMessageTypeEnum, BusinessLogicSettings.PushtNotificationDetails> mapAppCenterPushNotificationMessageType(Dictionary<BL_PushNotificationMessageTypeEnum, BL_Settings.BL_PushtNotificationDetails> appCenterPushNotifcationMessageTypes)
            {
                var r = new Dictionary<PushNotificationMessageTypeEnum, BusinessLogicSettings.PushtNotificationDetails>();

                foreach (var item in appCenterPushNotifcationMessageTypes)
                {
                    r.Add(map.From(item.Key), new BusinessLogicSettings.PushtNotificationDetails
                    {
                        CustomData_UserData = item.Value.CustomData_UserData,
                        CustomData_Page = item.Value.CustomData_Page,
                        TemplatePerLanguageLocale = mapPushNotificationTemplate(item.Value.TemplatePerLanguageLocale),
                    });
                }
                return r;
            }

            Dictionary<string, BusinessLogicSettings.PushNotificationMessageTemplate> mapPushNotificationTemplate(Dictionary<string, BL_Settings.BL_PushNotificationMessageTemplate> appCenterPushNotificationTemplates)
            {
                var r = new Dictionary<string, BusinessLogicSettings.PushNotificationMessageTemplate>();

                foreach (var item in appCenterPushNotificationTemplates)
                {
                    r.Add(item.Key, new BusinessLogicSettings.PushNotificationMessageTemplate { BodyTemplate = item.Value.BodyTemplate, TitleTemplate = item.Value.TitleTemplate });
                }

                return r;
            }
        }

    }
}
