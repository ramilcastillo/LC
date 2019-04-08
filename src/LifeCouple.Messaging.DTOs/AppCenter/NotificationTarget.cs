using System.Collections.Generic;
using Newtonsoft.Json;

namespace LifeCouple.Server.Messaging.DTOs.AppCenter
{

    public class NotificationTarget
    {
        [JsonProperty("type")]
        public TargetTypeEnum Type { get; set; }

        [JsonProperty("audiences")]
        public IEnumerable<string> Audiences { get; set; }

        /// <summary>
        /// Collection of device ids, typically guids
        /// </summary>
        [JsonProperty("devices")]
        public IEnumerable<string> Devices { get; set; }
    }
}
