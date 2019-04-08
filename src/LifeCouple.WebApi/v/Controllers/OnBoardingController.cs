using LifeCouple.DAL.Entities;
using LifeCouple.DTO.v;
using LifeCouple.WebApi.Common;
using LifeCouple.WebApi.DomainLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.v.Controllers
{
    [Produces("application/json")]
    [Route("api/userprofiles/me/onboarding")]
    public class OnBoardingController : Controller
    {

        private readonly BusinessLogic _bl;
        private readonly IConfiguration _config;

        public OnBoardingController(BusinessLogic businessLogic, IConfiguration config)
        {
            _bl = businessLogic;
            _config = config;
        }

        [HttpPut]
        public async Task<IActionResult> SetAnswerToQuestions([FromBody] QuestionnaireAnswersRequestInfo answerReq)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.ApiErrorMessage400BadRequest(ModelState);
                }

                var jwtPayloadInfo = this.GetJwtPayloadInfo();

                var userProfileId = await _bl.GetCachedUserId_byExternalReferenceIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (userProfileId == null)
                {
                    return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(jwtPayloadInfo.ExtReferenceId);
                }

                var request = MapFrom(answerReq, userProfileId);

                var idUpdatedOrCreated = await _bl.SetUserQuestionnaireAsync(request);

                return this.ApiPutMessage204NotContent();
            }
            catch (BusinessLogicException blexc)
            {
                return this.ApiErrorMessage400BadRequest(blexc);
            }
            catch (Exception exc)
            {
                return this.ApiErrorMessage400BadRequest(exc);
            }

        }

        private BL_QuestionnaireAnswers MapFrom(QuestionnaireAnswersRequestInfo a, string userProfileId)
        {
            var request = new BL_QuestionnaireAnswers
            {
                QuestionnaireTemplateId = a.QuestionnaireTemplateId,
                UserprofileId = userProfileId,
                Answers = new List<BL_QuestionnaireAnswers.Answer>()
            };

            //Add Answers in collection if any
            if (a.AnswersToSet != null)
            {
                foreach (var answer in a.AnswersToSet)
                {
                    var newAnswer = new BL_QuestionnaireAnswers.Answer();
                    AddAnswer(newAnswer, answer);
                    request.Answers.Add(newAnswer);
                }
            }

            //Add the single Answer if not yet in Answers collection
            if (a.AnswerToSet != null && request.Answers.FirstOrDefault(e => e.QuestionId == a.AnswerToSet.QuestionId) == null)
            {
                var newAnswer = new BL_QuestionnaireAnswers.Answer();
                AddAnswer(newAnswer, a.AnswerToSet);
                request.Answers.Add(newAnswer);
            }

            //Add actual answer recursevly to include any ChildAnswers
            void AddAnswer(BL_QuestionnaireAnswers.Answer currentAnswer, QuestionnaireAnswersRequestInfo.Answer inputAnswer)
            {
                currentAnswer.QuestionId = inputAnswer.QuestionId;
                currentAnswer.Value = inputAnswer.Value;

                if (inputAnswer.ChildAnswer != null)
                {
                    currentAnswer.ChildAnswer = new BL_QuestionnaireAnswers.Answer();
                    AddAnswer(currentAnswer.ChildAnswer, inputAnswer.ChildAnswer);
                }
            }

            return request;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var jwtPayloadInfo = this.GetJwtPayloadInfo();
                var usrProfile = await _bl.GetUserProfile_byExtRefIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (usrProfile == null)
                {
                    return this.ApiErrorMessage404NotFound($"No data found for UserId {jwtPayloadInfo.ExtReferenceId}");
                }

                var uq = await _bl.GetUserQuestionnaire_ByUserIdAsync(usrProfile.Id, QuestionnaireTemplate.QuestionnaireType.OnBoarding);
                var onboardingQuestionnaire = await _bl.GetQuestionnaireTemplate_ByUserQuestionnaire(uq, QuestionnaireTemplate.QuestionnaireType.OnBoarding, usrProfile.Gender);

                //Merge data with new entity called UserQuestionnaire
                var response = MapFrom(onboardingQuestionnaire, uq);

                return Json(response);
            }
            catch (Exception exc)
            {
                return this.ApiErrorMessage400BadRequest(exc);
            }
        }


        //TODO: remove the code below
        //[Obsolete("See the Get method")]
        //[HttpGet("template")]
        //public async Task<IActionResult> GetTemplateOnly()
        //{
        //    try
        //    {
        //        var externalRefId = this.GetExternalUserIdFromTokenExt();
        //        var usrProfile = await _bl.GetUserProfile_byExtRefIdAsync(externalRefId);
        //        if (usrProfile == null)
        //        {
        //            return this.ApiErrorMessage404NotFound($"No data found for UserId {externalRefId}");
        //        }

        //        var onboardingQuestionnaire = await _bl.GetQuestionnaireTemplate_ActiveByTypeAndGender(DAL.Entities.QuestionnaireTemplate.QuestionnaireType.OnBoarding, usrProfile.Gender);

        //        var response = MapFrom(onboardingQuestionnaire, null);
        //        return Json(response);
        //    }
        //    catch (Exception exc)
        //    {
        //        return this.ApiErrorMessage400BadRequest(exc);
        //    }
        //}

        private OnBoardingTemplateResponseInfo MapFrom(QuestionnaireTemplate input, UserQuestionnaire uq)
        {
            var r = new OnBoardingTemplateResponseInfo
            {
                Id = input.Id,
                IsComplete = (uq == null ? false : uq.IsQuestionnaireTemplateComplete),
                QuestionnaireSets = new List<QuestionnaireSetResponseInfo>()
            };

            foreach (var qset in input.QuestionnaireSets)
            {
                var qs = new QuestionnaireSetResponseInfo
                {
                    Id = qset.Id,
                    NextButtonText = qset.NextButtonText,
                    Text = qset.Text,
                    Title = qset.Title,
                    Questions = MapQuestions(qset.Questions)
                };
                r.QuestionnaireSets.Add(qs);
            }

            return r;

            List<QuestionResponseInfo> MapQuestions(List<Question> ques)
            {
                var newQs = new List<QuestionResponseInfo>();

                foreach (var question in ques)
                {
                    newQs.Add(MapQuestion(question));
                }
                return newQs;
            }

            QuestionResponseInfo MapQuestion(Question question)
            {
                return new QuestionResponseInfo
                {
                    AnswerOptions = MapAnswerOptions(question.AnswerOptions),
                    Description = question.Description,
                    Id = question.Id,
                    MaxRange = question.MaxRange,
                    MinRange = question.MinRange,
                    Text = question.Text,
                    Title = question.Title,
                    UserValue = uq?.Answers?.FirstOrDefault(e => e.QuestionId == question.Id)?.Value,
                    TypeOfQuestion = MapQuestionType(question.TypeOfQuestion),
                };
            }

            List<AnswerOptionsResponseInfo> MapAnswerOptions(List<AnswerOption> aos)
            {
                if (aos == null)
                {
                    return null;
                }

                var answerOptions = new List<AnswerOptionsResponseInfo>();
                foreach (var o in aos)
                {
                    var newO = new AnswerOptionsResponseInfo
                    {
                        ChildQuestions = MapQuestion(o.ChildQuestion),
                        Text = o.Text,
                        Value = o.Value,
                    };
                    answerOptions.Add(newO);
                }
                return answerOptions;
            }

            QuestionType MapQuestionType(Question.QuestionType qtype)
            {
                switch (qtype)
                {
                    case Question.QuestionType.MultipleOptionsMultipleChoice:
                        return QuestionType.MultipleOptionsMultipleChoice;
                    case Question.QuestionType.MultipleOptionsSingleChoice:
                        return QuestionType.MultipleOptionsSingleChoice;
                    case Question.QuestionType.Range:
                        return QuestionType.Range;
                    case Question.QuestionType.YesNo:
                        return QuestionType.YesNo;
                    default:
                        //TODO: Log
                        return QuestionType.YesNo;
                }
            }
        }
    }
}
