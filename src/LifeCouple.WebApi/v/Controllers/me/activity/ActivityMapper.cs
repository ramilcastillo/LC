using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;

namespace LifeCouple.WebApi.v.Controllers.me.activity
{
    public class ActivityMapper
    {

        public ActivityOverview GetActivityOverview(BL_ActivityDefinition activityDefinition)
        {
            var overview = new ActivityOverview
            {
                ActivityKey = activityDefinition.TypeOfActivity.ToString(),
                ActivityLevel = activityDefinition.ActivityLevel,
                Duration = activityDefinition.DurationText,
                Format = activityDefinition.Format,
                Objective = activityDefinition.Objective,
                Tip = activityDefinition.Tip,
                VideoComment = activityDefinition.VideoComment,
                VideoUrl = activityDefinition.VideoUrl,
            };

            return overview;
        }
    }
}
