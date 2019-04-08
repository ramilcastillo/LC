namespace LifeCouple.WebApi.DomainLogic.BL_DTOs
{
    public enum BL_PartnerInvitationStatusTypeEnum
    {
        Unknown = 0,

        /// <summary>
        /// Submitted by partner A
        /// </summary>
        Submitted = 10,

        /// <summary>
        /// Sent to 3rd party messaging provider by backend
        /// </summary>
        Sent = 20,

        /// <summary>
        /// Declined by partner B
        /// </summary>
        Declined = 30,

        /// <summary>
        /// Accepted by Partner B
        /// </summary>
        Accepted = 40,

        /// <summary>
        /// Completed - Partner B registered
        /// </summary>
        Completed = 50,

        /// <summary>
        /// Not applicable since this status is set for the Invitee when they registered 
        /// </summary>
        NotApplicable = 60,
    }
}
