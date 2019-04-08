using Newtonsoft.Json;

namespace LifeCouple.Server.Messaging.DTOs.AppCenter
{
    /// <summary>
    /// This is the actual payload that is being sent to AppCenter Push Notification POST api. 
    /// </summary>
    public class AppCenterPushNotificationMessage
    {
        [JsonProperty("notification_content")]
        public NotificationContent Content { get; set; }

        [JsonProperty("notification_target")]
        public NotificationTarget Target { get; set; }

    }
}
