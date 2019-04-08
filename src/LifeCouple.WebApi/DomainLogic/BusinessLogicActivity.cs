using System.Threading.Tasks;
using LifeCouple.DAL;
using LifeCouple.Server.Messaging;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace LifeCouple.WebApi.DomainLogic
{
    public class BusinessLogicActivity : BusinessLogic
    {
        protected readonly AppCenterService _appCenterService;

        public BusinessLogicActivity(IRepository repository, PhoneService phoneService, IMemoryCache memoryCache, AppCenterService appCenterService) : base(repository, phoneService, memoryCache)
        {
            _appCenterService = appCenterService;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary>
        /// Retrieves the common Activity definition. 
        /// NOTE: In the future this method will be enhanced to retrieve some of the information from configuration to support different languages, so that is why it is why CS1998 is suppressed 
        /// </summary>
        /// <param name="activityTypeEnum"></param>
        /// <returns></returns>
        public async Task<BL_ActivityDefinition> GetActivityDefinition(BL_ActivityTypeEnum activityTypeEnum)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            BL_ActivityDefinition definition = null;
            switch (activityTypeEnum)
            {
                case BL_ActivityTypeEnum.FirstStep:
                    definition = new BL_ActivityDefinition
                    {
                        ActivityLevel = "Level 1",
                        DurationText = "This activity is intended to be completed within 1 week.",
                        Format = "1 Part",
                        Objective = "Is a fun and playful way to work on an area in your relationship to get notice and recognition by your partner.",
                        TypeOfActivity = BL_ActivityTypeEnum.FirstStep,
                        Tip = "LifeCouple Tip: Be genuine in your actions as your partner will be asked about what you decided to focus in this activity",
                        VideoUrl = "https://medialc-uswe.streaming.media.azure.net/ff6da407-cbb8-492b-903f-eb32eb4d24d7/LifeCouple%20Promo%20Gavin_180619.ism/manifest",
                        VideoComment = "Let's start with something easy. Watch this short overview video."
                    };
                    break;
                default:
                    throw new BusinessLogicException(ValidationErrorCode.NotFound, "", $"Unsupported enum value '{activityTypeEnum}' of type {nameof(BL_ActivityTypeEnum)}");
            }

            return definition;

        }
    }
}
