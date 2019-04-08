using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LifeCouple.DAL.Entities
{
    public class Phone
    {
        [JsonProperty("userEnteredNr")]
        public string UserEnteredNr { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("nationalFormat")]
        public string NationalFormat { get; set; }

        [JsonProperty("phoneNr")]
        public string PhoneNr { get; set; }

        [JsonProperty("typeOfNumber")]
        public TypeOfNumberEnum TypeOfNumber { get; set; }

        [JsonProperty("isValidated")]
        public bool IsValidated { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum TypeOfNumberEnum
        {
            Unknown = 0,
            Other = 1,
            Mobile = 2,
        }
    }
}
