using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.DomainLogic.BL_DTOs
{
    public enum BL_Activity_FirstStepActionStatusTypeEnum
    {
        Unknown = 0,
        NotStarted = 100,
        StartedNotCompleted = 200,
        StartedAndCompleted = 300
    }
}
