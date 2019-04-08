namespace LifeCouple.WebApi.Common
{
    /// <summary>
    /// Based on https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#710-response-formats
    /// </summary>
    public class ErrorResponseInfo
    {
        public Error Error { get; set; }
    }

    public partial class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string DebugMessage { get; set; }
    }

}

