using System;
using Newtonsoft.Json;

namespace LifeCouple.DAL.Entities
{
    public class AppCenterDeviceDetail : CosmosDbEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("dtCreated")]
        public DateTimeOffset DTCreated { get; set; }

        [JsonProperty("dtLastUpdated")]
        public DateTimeOffset DTLastUpdated { get; set; }

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("typeOfDeviceOs")]
        public DeviceOsTypeEnum TypeOfDeviceOs { get; set; }

        [JsonProperty("userprofileId")]
        public string UserprofileId { get; set; }
    }
}
