using System.Collections.Generic;

namespace LifeCouple.WebApi.DomainLogic.BL_DTOs
{
    public class BL_EbaResponse
    {
        /// <summary>
        /// EBA point balance for the current user. Total points received
        /// </summary>
        public int EbaPointsBalance { get; set; }

        /// <summary>
        /// EBA points user have deposited to his/her partners EBA
        /// </summary>
        public int EbaPointsDeposited { get; set; }

        /// <summary>
        /// Most recent transactions. Limited to just a few
        /// </summary>
        public List<BL_EbaTransactionDetail> RecentTransactions { get; set; }

        /// <summary>
        /// To be used for the drop down UI when user is about deposit
        /// </summary>
        public List<BL_EbaPointOption> EbaPointOptions { get; set; }
    }
}
