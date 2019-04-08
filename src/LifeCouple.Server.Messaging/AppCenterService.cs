using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeCouple.Server.Messaging.DTOs.AppCenter;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace LifeCouple.Server.Messaging
{

    public class AppCenterService
    {
        private readonly LCMessagingSettingsModel _settings;

        private static readonly object lockObject = new object();

        private static bool? isInitializedSuccessfully;

        public AppCenterService(IOptions<LCMessagingSettingsModel> settings)
        {
            _settings = settings.Value;
            if (isInitializedSuccessfully != true)
            {
                lock (lockObject)
                {
                    if (isInitializedSuccessfully == null)
                    {
                        isInitializedSuccessfully = false;

                        if (_settings == null || JsonConvert.SerializeObject(_settings).Contains("null"))
                        {
                            throw new ArgumentNullException($"Unable to read/find one or more values for properties in the '{nameof(LCMessagingSettingsModel)}' model that needs to be set in application settings in the '{LCMessagingSettingsModel.SettingsSection}' section which is required to initiate the application. If you are running it locally check secrets.json and if in a hosted environment check Application Settings variables.");
                        }

                        try
                        {
                            //to ensure it fails fast
                            var storageAccount = CloudStorageAccount.Parse(_settings.AzureQueueConnection);
                            // Create the queue client.
                            var queueClient = storageAccount.CreateCloudQueueClient();
                            // Retrieve a reference to a container.
                            var queue = queueClient.GetQueueReference(_settings.AzurePushNotificationQueue);
                            // Create the queue if it doesn't already exist
                            var exists = queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();

                            isInitializedSuccessfully = true;

                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentNullException($"Unable to complete Initialzing AppCenterService. Probably invalid or missing values for properties in the '{nameof(LCMessagingSettingsModel)}' model that needs to be set in application settings in the '{LCMessagingSettingsModel.SettingsSection}' section which is required to initiate the application. If you are running it locally check secrets.json and if in a hosted environment check Application Settings variables.", ex);
                        }
                    }
                }
            }

            if (isInitializedSuccessfully == false)
            {
                throw new ArgumentNullException($"AppCenterService was not successfully initialized. Probably invalid or missing values for properties in the '{nameof(LCMessagingSettingsModel)}' model that needs to be set in application settings in the '{LCMessagingSettingsModel.SettingsSection}' section which is required to initiate the application. If you are running it locally check secrets.json and if in a hosted environment check Application Settings variables.");
            }
        }

        public async Task PublishPushNotificationDtoAsync(string appCenterEndpoint, List<string> deviceIds, string name, string title, string body, Dictionary<string, string> custom_data)
        {
            var messageObject = GenerateAppCenterPushNotificationDto(appCenterEndpoint, deviceIds, name, title, body, custom_data);
            await PublishPushNotificationDtoAsync(messageObject);
        }

        public async Task PublishPushNotificationDtoAsync(AppCenterPushNotificationDto appCenterPushNotificationDto)
        {
            //NOTE: None of the Azure Storage objects are thread safe = https://github.com/Azure/azure-storage-net/issues/270
            var storageAccount = CloudStorageAccount.Parse(_settings.AzureQueueConnection);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(_settings.AzurePushNotificationQueue);

            var message = new CloudQueueMessage(JsonConvert.SerializeObject(appCenterPushNotificationDto));
            await queue.AddMessageAsync(message);
        }

        public AppCenterPushNotificationDto GenerateAppCenterPushNotificationDto(string appCenterEndpoint, List<string> deviceIds, string name, string title, string body, Dictionary<string, string> custom_data)
        {
            var request = new AppCenterPushNotificationDto
            {
                AppCenterEndpoint = appCenterEndpoint,

                AppCenterNotification = new AppCenterPushNotificationMessage
                {
                    Content = new NotificationContent
                    {
                        Body = body,
                        Name = name,
                        Title = title,
                        CustomData = custom_data
                    },
                    Target = new NotificationTarget
                    {
                        Audiences = null,
                        Devices = deviceIds,
                        Type = TargetTypeEnum.devices_target
                    }
                }
            };

            return request;
        }
    }
}
