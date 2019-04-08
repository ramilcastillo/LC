using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LifeCouple.DAL.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PushNotificationMessageTypeEnum
    {
        Unknown = 0,
        Type_EBA_Points_Sent = 1,
    }
}