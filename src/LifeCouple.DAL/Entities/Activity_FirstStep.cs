using Newtonsoft.Json;

namespace LifeCouple.DAL.Entities
{
    /// <summary>
    /// Part of 'Activity' entity and unique to the first step Activity
    /// </summary>
    public class Activity_FirstStep
    {
        /// <summary>
        /// Areas Type selected by the user
        /// </summary>
        [JsonProperty("areaType")]
        public string AreaType { get; set; }

        /// <summary>
        /// The action that the user will take as entered in the mobile app
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }

        /// <summary>
        /// If user agreed to take the action entered
        /// </summary>
        [JsonProperty("isAgreed")]
        public bool? IsAgreed { get; set; }

        [JsonProperty("actionStatusType")]
        public string ActionStatusType { get; set; }



    }
}
