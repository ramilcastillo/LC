using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LifeCouple.DAL.Entities
{
    public class UserIndexHistory : CosmosDbEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("dtCreated")]
        public DateTimeOffset DTCreated { get; set; }

        [JsonProperty("userprofileId")]
        public string UserprofileId { get; set; }

        [JsonProperty("sourceEntityType")]
        public string SourceEntityType { get; set; }

        [JsonProperty("sourceEntityId")]
        public string SourceEntityId { get; set; }

        [JsonProperty(PropertyName = "indexes")]
        public List<Index> Indexes { get; set; }

        public class Index
        {
            [JsonProperty(PropertyName = "typeOfIndex")]
            public IndexTypeEnum TypeOfIndex { get; set; }

            [JsonProperty(PropertyName = "value")]
            public int Value { get; set; }
        }

    }
}
