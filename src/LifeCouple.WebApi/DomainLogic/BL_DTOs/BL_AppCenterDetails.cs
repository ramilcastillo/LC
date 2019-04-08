using System;

namespace LifeCouple.WebApi.DomainLogic.BL_DTOs
{
    public class BL_AppCenterDetails
    {
        public string DeviceId { get; set; }
        public BL_DeviceOsTypeEnum TypeOfOs { get; set; }
        public string UserprofileId { get; internal set; }
        public DateTimeOffset DTUpdated { get; set; }
    }
}
