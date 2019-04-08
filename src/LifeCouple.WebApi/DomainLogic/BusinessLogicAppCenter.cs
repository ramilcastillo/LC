using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeCouple.DAL;
using LifeCouple.DAL.Entities;
using LifeCouple.Server.Instrumentation;
using LifeCouple.Server.Messaging;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace LifeCouple.WebApi.DomainLogic
{
    public class BusinessLogicAppCenter : BusinessLogic
    {
        protected readonly AppCenterService _appCenterService;

        public BusinessLogicAppCenter(IRepository repository, PhoneService phoneService, IMemoryCache memoryCache, AppCenterService appCenterService) : base(repository, phoneService, memoryCache)
        {
            _appCenterService = appCenterService;
        }

        public async Task PublishNotificationMessage(BL_PushNotificationMessageTypeEnum pushNoticationType, string userprofileId, string entitityId)
        {
            try
            {
                var blsettings = await GetBusinessLogicSettings();

                var deviceDetails = await _repo.FindAppCenterDeviceDetail_ByUserIdAsync(userprofileId);


                if (deviceDetails.Count == 0)
                {
                    LCLog.Information($"PushNotifications - for UserId:{userprofileId} - Unable to publish PushNotification of type '{pushNoticationType}' since no deviceId(s) found.");
                }
                else
                {
                    var firstName = await GetCachedFirstname_byUserIdAsync(userprofileId);
                    foreach (var item in blsettings.AppCenterPushNotificationApiUrls)
                    {
                        var endpoint = item.Value;
                        var os = map.From(item.Key);

                        var deviceIds = deviceDetails.Where(e => e.TypeOfDeviceOs == os).Select(e1 => e1.DeviceId).ToList();
                        if (deviceIds != null && deviceIds.Count > 0)
                        {
                            var notificationDetails = blsettings.AppCenterPushNotificationMessageTypes.SingleOrDefault(e => e.Key == pushNoticationType).Value;

                            if (notificationDetails != null)
                            {
                                var languageLocaleKey = "";
                                if (notificationDetails.TemplatePerLanguageLocale.TryGetValue(languageLocaleKey, out var messageTemplate))
                                {
                                    await _appCenterService.PublishPushNotificationDtoAsync(
                                        endpoint
                                        , deviceIds
                                        , pushNoticationType.ToString()
                                        , messageTemplate.TitleTemplate
                                        , messageTemplate.BodyTemplate
                                        , new Dictionary<string, string> {
                                            { "Page", notificationDetails.CustomData_Page },
                                            { "UserData", BL_Settings.ParsePushNotificationCustomData_UserData(notificationDetails.CustomData_UserData, firstName) },
                                            { "EntityId", entitityId ?? "" } //For some reason App Center does not allow null here and responds with a 400
                                        }
                                        );
                                    LCLog.Information($"PushNotifications - for UserId:{userprofileId} - Succeeded to publish PushNotification of type '{pushNoticationType}' to '{endpoint}' for OS '{os}' for {deviceIds.Count} deviceid(s):{string.Join(" | ", deviceIds)}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LCLog.Fatal(ex, $"PushNotifications - for UserId:{userprofileId} - Failed to fail publish PushNotification of type '{pushNoticationType}'");
            }

        }

        public async Task<string> SetAppCenterDetailsAsync(BL_AppCenterDetails bl_dto)
        {
            var utcNow = DateTimeOffset.UtcNow;

            //Read Existing one
            var existingItem = await _repo.GetAppCenterDeviceDetails_DeviceId(bl_dto.DeviceId);
            if (existingItem == null)
            {
                existingItem = new AppCenterDeviceDetail
                {
                    DTCreated = utcNow,
                    DeviceId = bl_dto.DeviceId
                };
            }

            //Map incoming data to existing entity (need to be mapped this way to not overwrite new properties not part of incoming BL_UserProfile)
            existingItem.DTLastUpdated = utcNow;
            existingItem.TypeOfDeviceOs = map.From(bl_dto.TypeOfOs);
            existingItem.UserprofileId = bl_dto.UserprofileId;

            //Validate - done after mapping to ensure the mapping logic not broken
            if (validator.CanSetAppCenterDetails(existingItem, out var validationResult))
            {
                var idUpdatedOrCreated = await _repo.SetAppCenterDeviceDetailsAsync(existingItem);
                return idUpdatedOrCreated;
            }
            else
            {
                throw new BusinessLogicException(validationResult);
            }

        }
    }
}
