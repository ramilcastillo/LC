namespace LifeCouple.Server.Messaging.DTOs.AppCenter
{
    /// <summary>
    /// This is the message that is being published to the Azure Storage Queue
    /// </summary>
    public class AppCenterPushNotificationDto :  MsgBaseDto
    {
        /// <summary>
        /// The AppCenter endpoint Url, which is unique for each application and different for the Android vs Ios Application.
        /// e.g. https://api.appcenter.ms/v0.1/apps/LifeCouple/LifeCouple.Mobile-1/push/notifications
        /// </summary>
        public string AppCenterEndpoint { get; set; }

        public AppCenterPushNotificationMessage AppCenterNotification { get; set; }
    }

}
