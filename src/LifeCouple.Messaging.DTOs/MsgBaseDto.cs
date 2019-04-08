using Newtonsoft.Json;

namespace LifeCouple.Server.Messaging.DTOs
{
    public class MsgBaseDto
    {
        [JsonProperty(Order = -2)]
        public string MsgType { get; set; }

        public MsgBaseDto() => MsgType = GetType().Name;
    }
}
