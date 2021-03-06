﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace LifeCouple.Server.Messaging.DTOs.AppCenter
{
    public class NotificationContent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("custom_data")]
        public Dictionary<string, string> CustomData { get; set; }
    }
}