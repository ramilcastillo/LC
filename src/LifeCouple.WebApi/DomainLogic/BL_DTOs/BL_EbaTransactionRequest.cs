namespace LifeCouple.WebApi.DomainLogic.BL_DTOs
{
    public class BL_EbaTransactionRequest
    {
        public string Comment { get; set; }
        public int PointsToDeposit { get; set; }
        public string FromUserprofileId { get; internal set; }
    }
}
