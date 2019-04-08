using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace LifeCouple.Server.Messaging.Tests
{
    public class PushNotificationServiceTests : MessagingTestBase
    {
        private static AppCenterService svc = new AppCenterService(settingsOptions);

        [Fact]
        public void GenerateNotificationRequest_Ok()
        {
            var azureStorageQueueMessageDto = svc.GenerateAppCenterPushNotificationDto("https://api.appcenter.ms/v0.1/apps/LifeCouple/LifeCouple.Mobile-1/push/notifications",
                new List<string> {
                    { "a707a9d4-7151-4884-ac17-a9d8c2463b62" },
                    { "a707a9d4-7151-4884-ac17-a9d8c2463b63" }
                },
                "Type-EBA",
                null,
                "You have received a deposit in your Emotional Bank Account.",
                new Dictionary<string, string> {
                    { "Page", "EmotionalBankAccountPage" },
                    { "ForUserID", "c999a9d4-7151-4884-ac17-a9d8c2463b12"}
                });

            var completeDtoAsString = JsonConvert.SerializeObject(azureStorageQueueMessageDto, Formatting.None);
            var appCenterPayload = JsonConvert.SerializeObject(azureStorageQueueMessageDto.AppCenterNotification, Formatting.None);

            //Used https://www.freeformatter.com/java-dotnet-escape.html#ad-output to get the string from above escaped properly
            Assert.Equal("{\"MsgType\":\"AppCenterPushNotificationDto\",\"AppCenterEndpoint\":\"https://api.appcenter.ms/v0.1/apps/LifeCouple/LifeCouple.Mobile-1/push/notifications\",\"AppCenterNotification\":{\"notification_content\":{\"name\":\"Type-EBA\",\"title\":null,\"body\":\"You have received a deposit in your Emotional Bank Account.\",\"custom_data\":{\"Page\":\"EmotionalBankAccountPage\",\"ForUserID\":\"c999a9d4-7151-4884-ac17-a9d8c2463b12\"}},\"notification_target\":{\"type\":\"devices_target\",\"audiences\":null,\"devices\":[\"a707a9d4-7151-4884-ac17-a9d8c2463b62\",\"a707a9d4-7151-4884-ac17-a9d8c2463b63\"]}}}", completeDtoAsString);
            Assert.Equal("{\"notification_content\":{\"name\":\"Type-EBA\",\"title\":null,\"body\":\"You have received a deposit in your Emotional Bank Account.\",\"custom_data\":{\"Page\":\"EmotionalBankAccountPage\",\"ForUserID\":\"c999a9d4-7151-4884-ac17-a9d8c2463b12\"}},\"notification_target\":{\"type\":\"devices_target\",\"audiences\":null,\"devices\":[\"a707a9d4-7151-4884-ac17-a9d8c2463b62\",\"a707a9d4-7151-4884-ac17-a9d8c2463b63\"]}}", appCenterPayload);

        }

        [Fact]
        public async Task PublishNotificationRequest_Ok()
        {
            await svc.PublishPushNotificationDtoAsync("https://api.appcenter.ms/v0.1/apps/LifeCouple/LifeCouple.Mobile-1/push/notifications",
                new List<string> {
                    { "a707a9d4-7151-4884-ac17-a9d8c2463b62" },
                    { "a707a9d4-7151-4884-ac17-a9d8c2463b63" }
                },
                "Type-EBA",
                null,
                "Body",
                new Dictionary<string, string> {
                    { "Page", "EmotionalBankAccountPage" },
                    { "ForUserID", "c999a9d4-7151-4884-ac17-a9d8c2463b12"}
                });


        }

    }
}
