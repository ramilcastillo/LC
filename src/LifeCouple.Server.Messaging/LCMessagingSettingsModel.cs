namespace LifeCouple.Server.Messaging
{
    /// <summary>
    /// Populated using appSettings.json or environment variables or secrets.json when in development
    /// Add in Startup: services.Configure<LCMessagingSettingsModel>(_config.GetSection(LCMessagingSettingsModel.SettingsSection)); 
    /// and then injected into CosmosDb repo like 'IOptions<LCMessagingSettingsModel> settings'
    /// </summary>
    public class LCMessagingSettingsModel
    {
        public LCMessagingSettingsModel()
        {
            //Setting default values so setting them in app settings is optional
            AzurePushNotificationQueue = "appcenter-push-notification";
            AzureSmsQueue = "partner-invitation-sms";
        }

        public const string SettingsSection = "LCMessaging";

        public string SmsFromNr { get; set; }

        /// <summary>
        /// When set in EnvironmentVariables 'LCMessaging:TwilioLookupSid'
        /// </summary>
        public string TwilioLookupSid { get; set; }

        /// <summary>
        /// When set in EnvironmentVariables 'LCMessaging:TwilioLookupKey'
        /// </summary>
        public string TwilioLookupKey { get; set; }

        /// <summary>
        /// Connection string for the Azure Storage Account where the queue reside. For local dev use: UseDevelopmentStorage=true
        /// </summary>
        public string AzureQueueConnection { get; set; }

        /// <summary>
        /// Name of queue for SMS messages, in Azure Storage Account. Should be: partner-invitation-sms
        /// </summary>
        public string AzureSmsQueue { get; set; }

        /// <summary>
        /// Name of queue for PushNotification messages, in Azure Storage Account. Should be: appcenter-push-notification
        /// </summary>
        public string AzurePushNotificationQueue { get; set; }
    }
}
