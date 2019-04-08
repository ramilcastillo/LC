using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifeCouple.DAL.Entities;

namespace LifeCouple.DAL
{
    public partial class CosmosDBRepo
    {
        public async Task<string> SetActivityAsync(Activity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Activity> GetActivitiy_ByUserIdAsync(string userId, string activityType, string activityStatusType)
        {
            throw new NotImplementedException();
        }


    }
}
