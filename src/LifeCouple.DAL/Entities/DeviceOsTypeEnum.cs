using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LifeCouple.DAL.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceOsTypeEnum
    {
        Unknown = 0,
        Android = 1,
        IOs = 2,
    }
}
