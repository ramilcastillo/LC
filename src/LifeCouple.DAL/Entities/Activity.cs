using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LifeCouple.DAL.Entities
{
    /// <summary>
    /// Activity for a given user. Each user can have more than one activity for any given ActivityType
    /// </summary>
    public class Activity : CosmosDbEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty("dtCreated")]
        public DateTimeOffset DTCreated { get; set; }

        [JsonProperty("dtLastUpdated")]
        public DateTimeOffset DTLastUpdated { get; set; }

        [JsonProperty("userprofileId")]
        public string UserprofileId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("activityStatusTypeSetDT")]
        public Dictionary<string, DateTimeOffset> ActivityStatusTypeSetDT { get; set; }

        /// <summary>
        /// e.g. First Step, No Regrets, Crystal Ball
        /// </summary>
        [JsonProperty("activityType")]
        public string ActivityType { get; set; }

        /// <summary>
        /// e.g. NotStarted, InProgress, Completed
        /// </summary>
        [JsonProperty("activityStatusType")]
        public string ActitivyStatusType { get; set; }

        /// <summary>
        /// Only applicable when ActivityType is FirstStep
        /// </summary>
        [JsonProperty("a_FirstStep")]
        public Activity_FirstStep A_FirstStep { get; set; }


    }
}
