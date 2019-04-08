using System;

namespace LifeCouple.WebApi.DomainLogic.BL_DTOs
{
    public class BL_EbaTransactionDetail
    {
        public string Id { get; set; }

        public string Comment { get; set; }

        public int Point { get; set; }

        public DateTimeOffset Posted { get; set; }

        /// <summary>
        /// Firstname of the user that made the deposit
        /// </summary>
        public string FirstName { get; set; }

        public BL_EbaTransactionTypeEnum TypeOfTransaction { get; set; }
    }
}
