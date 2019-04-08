using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeCouple.DAL.Entities;
using LifeCouple.WebApi.DomainLogic;
using Xunit;

namespace LifeCouple.WebApi.BusinessLogicTests
{
    public class UserQuestionnaireTests : BusinessLogicTestBase
    {

        [Fact]
        public async Task SetAndCheckIndexes()
        {
            var extRefid = $"{GetType().Name}.{GetCallerMemberName()}";
            var testUser = await BL.CreateUserAsync($"{extRefid}@lifecouple.net", extRefid, true, "fName", "lName", null);

            var up = await BL.GetUserProfile_byExtRefIdAsync(testUser.ExternalRefId);
            up.FirstName = "John";
            up.LastName = "Doe";
            await BL.SetUserAsync(up);

            var userQ = await BL.GetUserQuestionnaire_ByUserIdAsync(testUser.Id, QuestionnaireTemplate.QuestionnaireType.OnBoarding);
            Assert.Null(userQ);

            var onboardingQuestionnaire = await BL.GetQuestionnaireTemplate_ByUserQuestionnaire(userQ, QuestionnaireTemplate.QuestionnaireType.OnBoarding, testUser.Gender);

            var answers = new BL_QuestionnaireAnswers
            {
                Answers = new List<BL_QuestionnaireAnswers.Answer>(),
                QuestionnaireTemplateId = onboardingQuestionnaire.Id,
                UserprofileId = testUser.Id
            };
            var v = 1;
            foreach (var q in onboardingQuestionnaire.QuestionnaireSets.First().Questions)
            {
                answers.Answers.Add(new BL_QuestionnaireAnswers.Answer
                {
                    QuestionId = q.Id,
                    Value = $"{v}",
                });

                v++;
                if (v > 10) v = 1;
            }
            var idUpdatedOrCreated = await BL.SetUserQuestionnaireAsync(answers);

            userQ = await BL.GetUserQuestionnaire_ByUserIdAsync(testUser.Id, QuestionnaireTemplate.QuestionnaireType.OnBoarding);
            Assert.NotNull(userQ);
            Assert.Equal(onboardingQuestionnaire.QuestionnaireSets.First().Questions.Count, userQ.Answers.Count);
            Assert.True(userQ.IsQuestionnaireTemplateComplete);

            testUser = await BL.GetUserProfile_byExtRefIdAsync(extRefid);
            Assert.Equal(up.FirstName, testUser.FirstName);
            Assert.Equal(up.LastName, testUser.LastName);
            Assert.Equal(up.PrimaryEmail, testUser.PrimaryEmail);
            Assert.Equal(up.PrimaryMobileNr, testUser.PrimaryMobileNr);
            Assert.Equal(5, testUser.Indexes?.Count);
            Assert.Equal(100, testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Communication).Value);
            Assert.Equal(30, testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.ConflictResolution).Value);
            Assert.Equal(20, testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Intimacy).Value);
            Assert.Equal(45, testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Overall).Value);
            Assert.Equal(20, testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Trust).Value);

            //Test updating user profile and ensure Indexes and other data is still intact
            var toBeUpdated_testUser = new BL_UserProfile
            {
                LastName = $"updateLastName{testUser.LastName}",
                Id = testUser.Id
            };
            await BL.SetUserAsync(toBeUpdated_testUser);
            var latestImageOf_testUser = await BL.GetUserProfile_byExtRefIdAsync(extRefid);
            Assert.Equal(latestImageOf_testUser.FirstName, testUser.FirstName);
            Assert.Equal(latestImageOf_testUser.LastName, toBeUpdated_testUser.LastName);
            Assert.Equal(latestImageOf_testUser.PrimaryEmail, testUser.PrimaryEmail);
            Assert.Equal(latestImageOf_testUser.PrimaryMobileNr, testUser.PrimaryMobileNr);
            Assert.Equal(latestImageOf_testUser.Indexes?.Count, testUser.Indexes?.Count);
            Assert.Equal(latestImageOf_testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Communication).Value, testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Communication).Value);
            Assert.Equal(latestImageOf_testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.ConflictResolution).Value, testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.ConflictResolution).Value);
            Assert.Equal(latestImageOf_testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Intimacy).Value, testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Intimacy).Value);
            Assert.Equal(latestImageOf_testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Overall).Value, testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Overall).Value);
            Assert.Equal(latestImageOf_testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Trust).Value, testUser.Indexes.Single(e => e.TypeOfIndex == IndexTypeEnum.Trust).Value);

        }


        [Fact]
        public async Task Set()
        {
            var questionnaire = await BL.GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType.OnBoarding, "m");
            var userProfileId = await BL.GetCachedUserId_byExternalReferenceIdAsync(SeededUserExtRefId);

            var userQuestionnaire = new UserQuestionnaire
            {
                UserprofileId = userProfileId,
                QuestionnaireTemplateId = questionnaire.Id,
                Answers = new List<UserQuestionnaire.Answer>
                {
                    new UserQuestionnaire.Answer
                    {
                        QuestionId = questionnaire.QuestionnaireSets.First().Questions.First().Id,
                        Value = "7"
                    }
                }
            };

            var newlyCreatedId = await BL.SetUserQuestionniareAsync(userQuestionnaire);

            //Update existing
            userQuestionnaire.Id = newlyCreatedId;
            userQuestionnaire.Answers.First().Value = "6";
            userQuestionnaire.Answers.Add(new UserQuestionnaire.Answer
            {
                QuestionId = questionnaire.QuestionnaireSets.First().Questions.Skip(1).First().Id,
                Value = "8"
            });
            var updatedId = await BL.SetUserQuestionniareAsync(userQuestionnaire);
            Assert.Equal(newlyCreatedId, updatedId);

            //Update existing again
            userQuestionnaire.Id = newlyCreatedId;
            userQuestionnaire.Answers.Clear();
            userQuestionnaire.Answers.Add(new UserQuestionnaire.Answer
            {
                QuestionId = questionnaire.QuestionnaireSets.First().Questions.Skip(1).First().Id,
                Value = "1"
            });
            userQuestionnaire.Answers.Add(new UserQuestionnaire.Answer
            {
                QuestionId = questionnaire.QuestionnaireSets.First().Questions.Skip(2).First().Id,
                Value = "2"
            });
            updatedId = await BL.SetUserQuestionniareAsync(userQuestionnaire);
            Assert.Equal(newlyCreatedId, updatedId);


            var uq = await BL.GetUserQuestionnaire_ByUserIdAsync(userProfileId, QuestionnaireTemplate.QuestionnaireType.OnBoarding);
            Assert.Equal(newlyCreatedId, uq.Id);
            Assert.Equal(QuestionnaireTemplate.QuestionnaireType.OnBoarding, uq.QuestionnaireTemplateType);
            Assert.Equal(userQuestionnaire.QuestionnaireTemplateId, uq.QuestionnaireTemplateId);
            Assert.Equal(userQuestionnaire.Answers.Count + 1, uq.Answers.Count);
            Assert.Equal(userQuestionnaire.Answers[0].QuestionId, uq.Answers[1].QuestionId);
            Assert.Equal(userQuestionnaire.Answers[1].QuestionId, uq.Answers[2].QuestionId);
            Assert.Equal(questionnaire.QuestionnaireSets[0].Questions[2].Id, uq.Answers[2].QuestionId);
            Assert.Equal(questionnaire.Id, uq.QuestionnaireTemplateId);


            var user = await BL.GetUserProfile_byExtRefIdAsync(SeededUserExtRefId);
        }

        [Fact]
        public async Task Set_Failed_Invalid_QuestionId()
        {
            var questionnaire = await BL.GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType.OnBoarding, "m");
            var userProfileId = await BL.GetCachedUserId_byExternalReferenceIdAsync(SeededUserExtRefId);

            var userQuestionnaire = new UserQuestionnaire
            {
                UserprofileId = userProfileId,
                QuestionnaireTemplateId = questionnaire.Id,
                Answers = new List<UserQuestionnaire.Answer>
                {
                    new UserQuestionnaire.Answer
                    {
                        QuestionId = "invalid question Id",// questionnaire.QuestionnaireSets.First().Questions.First().Id,
                        Value = "7"
                    }
                }
            };

            var ex = await Assert.ThrowsAsync<BusinessLogicException>(async () => await BL.SetUserQuestionniareAsync(userQuestionnaire));
            Assert.Equal(ValidationErrorCode.ValidationFailure_UserQuestionnaire_Answers, ex.ErrorCode);
            Assert.Equal(userQuestionnaire.Id, ex.EntityIdInError);
            Assert.StartsWith("Question Id:invalid question Id in UserQuestionnaire with Id: does not exist in QuestionnaireTemplate with Id", ex.DebugMessage);
        }

        [Fact]
        public async Task Set_Failed_Invalid_AnswerValueType()
        {
            var questionnaire = await BL.GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType.OnBoarding, "m");
            var userProfileId = await BL.GetCachedUserId_byExternalReferenceIdAsync(SeededUserExtRefId);

            //Check so the fist question is of expected type
            var firstQuestion = questionnaire.QuestionnaireSets.First().Questions.First();
            Assert.Equal(Question.QuestionType.Range, firstQuestion.TypeOfQuestion);

            var userQuestionnaire = new UserQuestionnaire
            {
                UserprofileId = userProfileId,
                QuestionnaireTemplateId = questionnaire.Id,
                Answers = new List<UserQuestionnaire.Answer>
                {
                    new UserQuestionnaire.Answer
                    {
                        QuestionId = firstQuestion.Id,
                        Value = "abc"
                    }
                }
            };

            var ex = await Assert.ThrowsAsync<BusinessLogicException>(async () => await BL.SetUserQuestionniareAsync(userQuestionnaire));
            Assert.Equal(ValidationErrorCode.ValidationFailure_UserQuestionnaire_Answers_InvalidValue, ex.ErrorCode);
            Assert.Equal(userQuestionnaire.Id, ex.EntityIdInError);
            Assert.StartsWith("The value 'abc' for Question Id", ex.DebugMessage);
            Assert.EndsWith(" is invalid, valid range is '0' to '10'", ex.DebugMessage);
        }

        [Fact]
        public async Task Set_Failed_Invalid_AnswerRange()
        {
            var questionnaire = await BL.GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType.OnBoarding, "m");
            var userProfileId = await BL.GetCachedUserId_byExternalReferenceIdAsync(SeededUserExtRefId);

            //Check so the fist question is of expected type
            var firstQuestion = questionnaire.QuestionnaireSets.First().Questions.First();
            Assert.Equal(Question.QuestionType.Range, firstQuestion.TypeOfQuestion);

            var userQuestionnaire = new UserQuestionnaire
            {
                UserprofileId = userProfileId,
                QuestionnaireTemplateId = questionnaire.Id,
                Answers = new List<UserQuestionnaire.Answer>
                {
                    new UserQuestionnaire.Answer
                    {
                        QuestionId = firstQuestion.Id,
                        Value = "99"
                    }
                }
            };

            var ex = await Assert.ThrowsAsync<BusinessLogicException>(async () => await BL.SetUserQuestionniareAsync(userQuestionnaire));
            Assert.Equal(ValidationErrorCode.ValidationFailure_UserQuestionnaire_Answers_InvalidValue, ex.ErrorCode);
            Assert.Equal(userQuestionnaire.Id, ex.EntityIdInError);
            Assert.StartsWith("The value '99' for Question Id", ex.DebugMessage);
            Assert.EndsWith(" is out of range, valid range is '0' to '10'", ex.DebugMessage);

        }


        [Fact]
        public async Task Set_Failed_QuestionnaireTemplateId()
        {
            var questionnaire = await BL.GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType.OnBoarding, "m");
            var questionnaireTemplateId = "123-invalidQuestionnaireTemplateId";
            var userProfileId = "123-invalidUserProfileId";

            var userQuestionnaire = new UserQuestionnaire
            {
                UserprofileId = userProfileId,
                QuestionnaireTemplateId = questionnaireTemplateId,
                Answers = new List<UserQuestionnaire.Answer>
                {
                    new UserQuestionnaire.Answer
                    {
                        QuestionId = questionnaire.QuestionnaireSets.First().Questions.First().Id,
                        Value = "7"
                    }
                }
            };

            var ex = await Assert.ThrowsAsync<BusinessLogicException>(async () => await BL.SetUserQuestionniareAsync(userQuestionnaire));
            Assert.Equal(ValidationErrorCode.QuestionnaireTemplateIdNotFound, ex.ErrorCode);
            Assert.Equal(questionnaireTemplateId, ex.EntityIdInError);
        }

        [Fact]
        public async Task Set_Failed_UserIdNull()
        {
            var questionnaireTemplateId = "123-invalidQuestionnaireId";

            var userQuestionnaire = new UserQuestionnaire
            {
                QuestionnaireTemplateId = questionnaireTemplateId,
            };

            var ex = await Assert.ThrowsAsync<BusinessLogicException>(async () => await BL.SetUserQuestionniareAsync(userQuestionnaire));
            Assert.Equal(ValidationErrorCode.IdIsNull_UserProfileId, ex.ErrorCode);
            Assert.Equal(userQuestionnaire.Id, ex.EntityIdInError);
            Assert.Equal("One or more Ids in instance of type 'LifeCouple.DAL.Entities.UserQuestionnaire' is null.", ex.DebugMessage);
            Assert.Equal(ex.EntityJsonRepresentation, ex.Result.Failures.First().EntityJsonRepresentation);
        }


        [Fact]
        public async Task Set_Failed_UserId()
        {
            var v = new Validator(phoneService);
            var questionnaire = await BL.GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType.OnBoarding, "m");
            var userProfileId = "123-invalidUserProfileId";

            var userQuestionnaire = new UserQuestionnaire
            {
                QuestionnaireTemplateId = questionnaire.Id,
                UserprofileId = userProfileId,
                Answers = new List<UserQuestionnaire.Answer>
                {
                    new UserQuestionnaire.Answer
                    {
                        QuestionId = questionnaire.QuestionnaireSets.First().Questions.First().Id,
                        Value = "7"
                    }
                }
            };

            var ex = await Assert.ThrowsAsync<BusinessLogicException>(async () => await BL.SetUserQuestionniareAsync(userQuestionnaire));
            Assert.Equal(ValidationErrorCode.UserProfileIdNotFound, ex.ErrorCode);
            Assert.Equal(userProfileId, ex.EntityIdInError);


        }


        [Fact]
        public async Task Set_Failed_Validation_NoAnswers()
        {
            var v = new Validator(phoneService);
            var questionnaireTemplateId = await BL.GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType.OnBoarding, "m");
            var userProfileId = "123-invalidUserProfileId";

            var userQuestionnaire = new UserQuestionnaire
            {
                QuestionnaireTemplateId = questionnaireTemplateId.Id,
                UserprofileId = userProfileId,
            };

            var ex = await Assert.ThrowsAsync<BusinessLogicException>(async () => await BL.SetUserQuestionniareAsync(userQuestionnaire));
            Assert.Equal(ValidationErrorCode.ValidationFailure_UserQuestionnaire_Answers, ex.ErrorCode);
            Assert.Null(ex.EntityIdInError);
            Assert.Equal("Need at least one Answer for instance of type 'LifeCouple.DAL.Entities.UserQuestionnaire'.", ex.DebugMessage);


        }

    }
}
