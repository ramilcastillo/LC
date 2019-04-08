using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using Xunit;
using Xunit.Abstractions;

namespace LifeCouple.WebApi.Test.FunctionalTests
{
    public class OnBoardingController : BaseWebApiTest
    {

        public OnBoardingController(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async Task Get_Template_Ok()
        {
            var response = await _clientWithToken.GetAsync("api/userprofiles/me/onboarding");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal(18, body.Split("typeOfQuestion").Count());

            var payload = Deserialize<OnBoardingTemplateResponseInfo>(body);
            Assert.Single(payload.QuestionnaireSets);
            Assert.Equal(17, payload.QuestionnaireSets.First().Questions.Count);

        }

        [Fact]
        public async Task Set_Template_Ok()
        {
            //First get the payload
            var getResponse = await _clientWithToken.GetAsync("api/userprofiles/me/onboarding");
            getResponse.EnsureSuccessStatusCode();
            var getResponseBody = await getResponse.Content.ReadAsStringAsync();
            var getResponseInstance = Deserialize<OnBoardingTemplateResponseInfo>(getResponseBody);

            //Create Request payload and update all the answers with a Value
            var putRequest1 = new QuestionnaireAnswersRequestInfo
            {
                QuestionnaireTemplateId = getResponseInstance.Id,
                AnswerToSet = null, // new QuestionnaireAnswersRequestInfo.Answer()
                AnswersToSet = new List<QuestionnaireAnswersRequestInfo.Answer>()
            };

            var v = 1;
            foreach (var q in getResponseInstance.QuestionnaireSets.First().Questions)
            {
                if (v <= 9)
                {
                    v++;
                }
                else
                {
                    v = 1;
                }
                putRequest1.AnswersToSet.Add(new QuestionnaireAnswersRequestInfo.Answer
                {
                    QuestionId = q.Id,
                    Value = $"{v}"
                });
            }

            //Remove Last Answer
            var lastAnswer = putRequest1.AnswersToSet.Last();
            putRequest1.AnswersToSet.Remove(lastAnswer);

            //Set the answers
            var putResponse = await _clientWithToken.PutAsync("api/userprofiles/me/onboarding", GetJsonContent(putRequest1));
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
            var putResponseBody = await putResponse.Content.ReadAsStringAsync();
            Assert.Equal("", putResponseBody);

            //Set the last answer
            var putRequest2 = new QuestionnaireAnswersRequestInfo
            {
                QuestionnaireTemplateId = getResponseInstance.Id,
                AnswerToSet = lastAnswer,
                AnswersToSet = null
            };
            var putResponse2 = await _clientWithToken.PutAsync("api/userprofiles/me/onboarding", GetJsonContent(putRequest2));
            Assert.Equal(HttpStatusCode.NoContent, putResponse2.StatusCode);
            var putResponse2Body = await putResponse2.Content.ReadAsStringAsync();
            Assert.Equal("", putResponse2Body);

            //Set the last answer again - should fail
            var putRequest3 = new QuestionnaireAnswersRequestInfo
            {
                QuestionnaireTemplateId = getResponseInstance.Id,
                AnswerToSet = lastAnswer,
                AnswersToSet = null
            };
            var putResponse3 = await _clientWithToken.PutAsync("api/userprofiles/me/onboarding", GetJsonContent(putRequest3));
            //var stringValue = Newtonsoft.Json.JsonConvert.SerializeObject(putRequest3); //Only used to update onboardingPUT.http sample
            Assert.Equal(HttpStatusCode.BadRequest, putResponse3.StatusCode);
            var putResponse3Body = await putResponse3.Content.ReadAsStringAsync();
            Assert.StartsWith("{\"error\":{\"code\":\"ValidationFailure_UserQuestionnaire\",\"message\":\"BusinessLogicException\",\"debugMessage\":\"UserQuestionnaire with Id:", putResponse3Body);
            Assert.EndsWith(" can not be updated since it is already Completed\"}}", putResponse3Body);


            //First get the payload
            var getResponse2 = await _clientWithToken.GetAsync("api/userprofiles/me/onboarding");
            getResponse2.EnsureSuccessStatusCode();
            var getResponseBody2 = await getResponse2.Content.ReadAsStringAsync();
            var getResponseInstance2 = Deserialize<OnBoardingTemplateResponseInfo>(getResponseBody2);
            Assert.True(getResponseInstance2.IsComplete);
            Assert.Equal(17, getResponseInstance2.QuestionnaireSets.First().Questions.Count);
            foreach (var q in getResponseInstance2.QuestionnaireSets.First().Questions)
            {
                Assert.NotNull(q.UserValue);
            }

        }

    }
}


