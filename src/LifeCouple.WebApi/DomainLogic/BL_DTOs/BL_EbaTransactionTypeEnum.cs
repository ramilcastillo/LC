namespace LifeCouple.WebApi.DomainLogic.BL_DTOs
{
    public enum BL_EbaTransactionTypeEnum
    {
        /// <summary>
        /// This value should never occur nor be used
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Points Received from Partner
        /// </summary>
        Received = 1,

        /// <summary>
        /// Points Sent (or Deposited) to Partner
        /// </summary>
        Sent = 2,
    }
}
