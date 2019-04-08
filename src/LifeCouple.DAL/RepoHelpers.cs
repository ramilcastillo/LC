using Newtonsoft.Json;

namespace LifeCouple.DAL
{
    public class RepoHelpers
    {
        public static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc, DateParseHandling = DateParseHandling.DateTimeOffset };
    }
}
