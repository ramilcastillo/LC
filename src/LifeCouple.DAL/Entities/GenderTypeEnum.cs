using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LifeCouple.DAL.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GenderTypeEnum
    {
        Unknown = 0,
        Female = 1,
        Male = 2,
        NotSpecified = 3,
    }
}
