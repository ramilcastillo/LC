using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.DomainLogic.BL_DTOs
{
    public class BL_PartnerInvitation
    {

        public string InvitedByUserId { get; set; }

        public string InviterFirstName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MobilePhone { get; set; }

        public BL_GenderTypeEnum TypeOfGender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public BL_PartnerInvitationStatusTypeEnum InvitationStatus { get; set; }

        public string InvitationId { get; set; }

    }
}
