using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LifeCouple.DAL.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndexTypeEnum
    {
        Unknown = 0,
        Overall = 1,
        Intimacy = 2,
        Communication = 3,
        Trust = 4,
        ConflictResolution = 5,
    }
}
