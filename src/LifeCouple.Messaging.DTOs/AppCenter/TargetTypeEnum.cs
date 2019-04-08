using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LifeCouple.Server.Messaging.DTOs.AppCenter
{

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TargetTypeEnum
    {
        audiences_target, //this value is the actual value part of the payload to App Center
        devices_target, //this value is the actual value part of the payload to App Center
    }
}
