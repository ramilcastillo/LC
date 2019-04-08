using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeCouple.DAL.Entities;
using LifeCouple.WebApi.DomainLogic;
using Xunit;

namespace LifeCouple.WebApi.BusinessLogicTests
{
    public class UserProfileTests : BusinessLogicTestBase
    {
        [Fact]
        public async Task UserProfile_CreateUserAsync()
        {
            var extRefId = "UserProfile_CreateUserAsync";
            var email = "UserProfile_CreateUserAsync@lifecouple.net";
            var existingUsr = await BL.GetUserProfile_byExtRefIdAsync(extRefId);
            Assert.Null(existingUsr);

            var newUsr = await BL.CreateUserAsync(email, extRefId, false, "fName", "lName", null);
            Assert.Equal(newUsr.PrimaryEmail, email);

            //var newQtemplate = new DAL.Entities.QuestionnaireTemplate { IsActive = true, TypeOfGender = QuestionnaireTemplate.GenderType.Any, TypeOfQuestionnaire = QuestionnaireTemplate.QuestionnaireType.OnBoarding };
            //newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { Description = "Let's start with something basic...", Text = "what is 1 + 2?", Title = "Simple Math Quiz", MaxRange = 10, MinRange = 0, TypeOfIndexImpacted = Question.IndexType.Communication, TypeOfQuestion = Question.QuestionType.MultipleOptionsSingleChoice, AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { Value = "1", Text = "One" }, new DAL.Entities.AnswerOption { Value = "2", Text = "Two" }, new DAL.Entities.AnswerOption { Value = "3", Text = "Three" } } } } } };
            //var newId = await bl.CreateQuestionnaireTemplateAsync(newQtemplate);

            //var newlyCreatedQtemplate = await bl.GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType.OnBoarding, "m");
            //Assert.NotNull(newlyCreatedQtemplate);
            //Assert.Equal("QuestionnaireTemplate", newlyCreatedQtemplate.Type);
            //Assert.Equal(newQtemplate.TypeOfGender, newlyCreatedQtemplate.TypeOfGender);
            //Assert.Equal(newQtemplate.TypeOfQuestionnaire, newlyCreatedQtemplate.TypeOfQuestionnaire);
            //Assert.Equal(newQtemplate.QuestionnaireSets.Count, newlyCreatedQtemplate.QuestionnaireSets.Count);
            //Assert.Equal(newQtemplate.QuestionnaireSets.First().NextButtonText, newlyCreatedQtemplate.QuestionnaireSets.First().NextButtonText);
            //Assert.Equal(newQtemplate.QuestionnaireSets.First().Text, newlyCreatedQtemplate.QuestionnaireSets.First().Text);
            //Assert.Equal(newQtemplate.QuestionnaireSets.First().Title, newlyCreatedQtemplate.QuestionnaireSets.First().Title);
            //Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().Description, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().Description);
            //Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().Text, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().Text);
            //Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().Title, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().Title);
            //Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().TypeOfIndexImpacted, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().TypeOfIndexImpacted);
            //Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().TypeOfQuestion, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().TypeOfQuestion);
            //Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().AnswerOptions.Count, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().AnswerOptions.Count);
            //Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().AnswerOptions.First().Value, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().AnswerOptions.First().Value);
        }


        [Fact]
        public async Task UserProfile_Set_Validation()
        {
            var extRefId = "UserProfile_Set_Validation";
            var email = "UserProfile_Set_Validation@lifecouple.net";
            var newUsr = await BL.CreateUserAsync(email, extRefId, false, "fName", "lName", null);
            Assert.Equal(newUsr.PrimaryEmail, email);

            newUsr.Indexes = new List<BL_UserProfile.Index> { new BL_UserProfile.Index { TypeOfIndex = DAL.Entities.IndexTypeEnum.Overall, Value = 10 } };
            await BL.SetUserAsync(newUsr);
            var updatedUserProfile = await BL.GetUserProfile_byExtRefIdAsync(extRefId);
            Assert.Single(updatedUserProfile.Indexes);
            var firstIndex = updatedUserProfile.Indexes.First();
            Assert.Equal(IndexTypeEnum.Overall, firstIndex.TypeOfIndex);
            Assert.Equal(10, firstIndex.Value);

            firstIndex.Value = 11;
            updatedUserProfile.Indexes.Add(new BL_UserProfile.Index { TypeOfIndex = IndexTypeEnum.ConflictResolution, Value = 99 });
            await BL.SetUserAsync(updatedUserProfile);
            var updatedUserProfile2 = await BL.GetUserProfile_byExtRefIdAsync(updatedUserProfile.ExternalRefId);
            Assert.Equal(2, updatedUserProfile2.Indexes.Count);
            Assert.Equal(IndexTypeEnum.Overall, updatedUserProfile2.Indexes[0].TypeOfIndex);
            Assert.Equal(11, updatedUserProfile2.Indexes[0].Value);
            Assert.Equal(IndexTypeEnum.ConflictResolution, updatedUserProfile2.Indexes[1].TypeOfIndex);
            Assert.Equal(99, updatedUserProfile2.Indexes[1].Value);


            //TODO: B 1 Add new Validator to return empty list when no validation errors and collection of errors when there are validation errors - type in collection need to contain user friendly error as well as error code system error, use constants + text field
            //TODO: B 2 Add validation for Phone nr to be US (10 or 11 with first one being 1), also when setting values and new value need to match validation when current value is set (ok nr cannot be overriden with invalid nr)

        }
        [Fact]
        public async Task UserProfile_GetCachedPartnerUserId()
        {
            System.Diagnostics.Debug.WriteLine("UserProfile_GetCachedPartnerUserId() - .");
            //pgrimskog@lifecouple.net
            //pgrimskogPartner@lifecouple.net should be associated with another user with email pgrimskog@lifecouple.net
            var existingUsersA = await BL.FindUserProfiles_byEmailAsync("pgrimskog@lifecouple.net", true);
            var existingUsersB = await BL.FindUserProfiles_byEmailAsync("pgrimskogPartner@lifecouple.net", true);
            System.Diagnostics.Debug.WriteLine("UserProfile_GetCachedPartnerUserId() - ..");
            var otherUserId = await BL.GetCachedPartnerUserId(existingUsersA.Single().Id);
            Assert.Equal(otherUserId, existingUsersB.Single().Id);
        }
    }
}
