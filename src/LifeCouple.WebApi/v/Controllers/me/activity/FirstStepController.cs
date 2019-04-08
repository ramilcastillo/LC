using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using LifeCouple.WebApi.DomainLogic;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LifeCouple.WebApi.v.Controllers.me.activity
{
    //NOTE: the last segment of this endpoint should be aligned with the string values of BL_ActivityTypeEnum
    [Produces("application/json")]
    [Route("api/userprofiles/me/activity/firststep")]
    public class FirstStepController : Controller
    {
        private readonly BusinessLogicActivity _bl;
        private readonly IConfiguration _config;
        private readonly ActivityMapper _mapper;

        public FirstStepController(BusinessLogicActivity businessLogic, IConfiguration config)
        {
            _bl = businessLogic;
            _config = config;
            _mapper = new ActivityMapper();
        }


        [HttpGet("{id}")]
        public ActionResult<Activity_FirstStep_ResponseInfo> GetById(string id = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// To get a new template/id or the the one that is not Completed
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //TODO: A Add Id in response since the user will be able to do this exercise more than once....
            return Json(await GetMockResponse());
        }

        private async Task<Activity_FirstStep_ResponseInfo> GetMockResponse()
        {
            var r = new Activity_FirstStep_ResponseInfo
            {
                ActivityTypeOfStatus = ActivityStatusType.NotStarted,
                Areas = new List<Activity_FirstStep_Area> {
                    new Activity_FirstStep_Area {
                        Description ="Asking your partner their point of view or better listening",
                        Title = "Communication",
                        AreaKey = BL_Activity_FirstStepAreaTypeEnum.Communication.ToString(),
                    },
                    new Activity_FirstStep_Area {
                        Description ="Taking on things you don’t  usually do in the household",
                        Title = "Chores",
                        AreaKey = BL_Activity_FirstStepAreaTypeEnum.Chores.ToString(),
                    },
                    //
                    new Activity_FirstStep_Area {
                        Description ="Doing things that can spark romance",
                        Title = "Romance",
                        AreaKey = BL_Activity_FirstStepAreaTypeEnum.Romance.ToString(),
                    },
                    new Activity_FirstStep_Area {
                        Description ="Say \"I’m sorry\" for something that upset your partner",
                        Title = "Ownership Certificate",
                        AreaKey = BL_Activity_FirstStepAreaTypeEnum.OwnershipCertificate.ToString(),
                    },
                    new Activity_FirstStep_Area {
                        Description ="Make an effort to create quality time together",
                        Title = "Quality Time",
                        AreaKey = BL_Activity_FirstStepAreaTypeEnum.QualityTime.ToString(),
                    },
                    new Activity_FirstStep_Area {
                        Description ="When talking to your partner,  change or watch your tone",
                        Title = "Tone",
                        AreaKey = BL_Activity_FirstStepAreaTypeEnum.Tone.ToString(),
                    },
                    new Activity_FirstStep_Area {
                        Description ="Take the vibe to fun and light hearted",
                        Title = "Intensity",
                        AreaKey = BL_Activity_FirstStepAreaTypeEnum.Intensity.ToString(),
                    },

                },
                Overview = _mapper.GetActivityOverview(await _bl.GetActivityDefinition(BL_ActivityTypeEnum.FirstStep)),
                SelectedAreaKey = null,
                ActionValue = null,
                IsAgreed = null,
            };

            return r;
        }

    }
}