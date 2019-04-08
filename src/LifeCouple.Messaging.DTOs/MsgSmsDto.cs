namespace LifeCouple.Server.Messaging.DTOs
{
    public class MsgSmsDto : MsgBaseDto
    {
        public string ToMobileNr { get; set; }

        public string SmsMessageBody { get; set; }

        public string FromPhoneNr { get; set; }
    }
}
