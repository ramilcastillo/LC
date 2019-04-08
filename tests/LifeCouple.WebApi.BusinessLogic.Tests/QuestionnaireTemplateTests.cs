using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LifeCouple.DAL;
using LifeCouple.WebApi.DomainLogic;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using LifeCouple.DAL.Entities;

namespace LifeCouple.WebApi.BusinessLogicTests
{
    public class QuestionnaireTemplateTests : BusinessLogicTestBase
    {
        [Fact]
        public void QuestionnaireTemplate_Validate()
        {
            var v = new Validator(phoneService);
            var bl = BL;

            Exception exc = Assert.Throws<ArgumentNullException>(() => v.IsValid<QuestionnaireTemplate>(null));

            var newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { Id = "id.." };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet>();
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Id = "2" } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { Id = "question" } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { AnswerOptions = new List<DAL.Entities.AnswerOption> { } } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { } } } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { Text = "text" } } } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { Value = "value" } } } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { Value = "value", Text = "text", ChildQuestion = new DAL.Entities.Question { } } } } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { Value = "value", Text = "text", ChildQuestion = new DAL.Entities.Question { Id = "childquestion" } } } } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { Value = "value", Text = "text" } } } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { TypeOfQuestionnaire = QuestionnaireTemplate.QuestionnaireType.OnBoarding };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { Value = "value", Text = "text" } } } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { TypeOfGender = QuestionnaireTemplate.GenderType.Any };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { Value = "value", Text = "text" } } } } } };
            exc = Assert.Throws<ArgumentOutOfRangeException>(() => v.IsValid(newQtemplate));

            newQtemplate = new DAL.Entities.QuestionnaireTemplate { TypeOfQuestionnaire = QuestionnaireTemplate.QuestionnaireType.OnBoarding, TypeOfGender = QuestionnaireTemplate.GenderType.Any };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { Value = "value", Text = "text" } } } } } };
            Assert.True(v.IsValid(newQtemplate));

            bl.AssignIdsForQuestionnaire(newQtemplate);
            Assert.IsType<Guid>(Guid.Parse(newQtemplate.QuestionnaireSets.First().Id));
            Assert.IsType<Guid>(Guid.Parse(newQtemplate.QuestionnaireSets.First().Questions.First().Id));

            Assert.Equal(newQtemplate.QuestionnaireSets.First().Id.ToUpper(), newQtemplate.QuestionnaireSets.First().Id);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().Id.ToUpper(), newQtemplate.QuestionnaireSets.First().Questions.First().Id);
        }

        [Fact]
        public async Task QuestionnaireTemplate_Create()
        {
            var bl = BL;

            var newQtemplate = new DAL.Entities.QuestionnaireTemplate { IsActive = true, TypeOfGender = QuestionnaireTemplate.GenderType.Female, TypeOfQuestionnaire = QuestionnaireTemplate.QuestionnaireType.TestNotInUse };
            newQtemplate.QuestionnaireSets = new List<DAL.Entities.QuestionnaireSet> { new DAL.Entities.QuestionnaireSet { Questions = new List<DAL.Entities.Question> { new DAL.Entities.Question { Description = "Let's start with something basic...", Text = "what is 1 + 2?", Title = "Simple Math Quiz", MaxRange = 10, MinRange = 0,
                IndexesImpacted = new List<IndexImpact>{ new IndexImpact { CalculationWeight =1, TypeOfIndex = IndexTypeEnum.Communication } },
                TypeOfQuestion = Question.QuestionType.MultipleOptionsSingleChoice, AnswerOptions = new List<DAL.Entities.AnswerOption> { new DAL.Entities.AnswerOption { Value = "1", Text = "One" }, new DAL.Entities.AnswerOption { Value = "2", Text = "Two" }, new DAL.Entities.AnswerOption { Value = "3", Text = "Three" } } } } } };
            var newId = await bl.CreateQuestionnaireTemplateAsync(newQtemplate);

            var newlyCreatedQtemplate = await bl.GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType.TestNotInUse, "f");
            Assert.NotNull(newlyCreatedQtemplate);
            Assert.Equal("QuestionnaireTemplate", newlyCreatedQtemplate.Type);
            Assert.Equal(newQtemplate.TypeOfGender, newlyCreatedQtemplate.TypeOfGender);
            Assert.Equal(newQtemplate.TypeOfQuestionnaire, newlyCreatedQtemplate.TypeOfQuestionnaire);
            Assert.Equal(newQtemplate.QuestionnaireSets.Count, newlyCreatedQtemplate.QuestionnaireSets.Count);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().NextButtonText, newlyCreatedQtemplate.QuestionnaireSets.First().NextButtonText);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().Text, newlyCreatedQtemplate.QuestionnaireSets.First().Text);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().Title, newlyCreatedQtemplate.QuestionnaireSets.First().Title);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().Description, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().Description);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().Text, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().Text);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().Title, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().Title);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().IndexesImpacted.First().TypeOfIndex, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().IndexesImpacted.First().TypeOfIndex);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().TypeOfQuestion, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().TypeOfQuestion);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().AnswerOptions.Count, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().AnswerOptions.Count);
            Assert.Equal(newQtemplate.QuestionnaireSets.First().Questions.First().AnswerOptions.First().Value, newlyCreatedQtemplate.QuestionnaireSets.First().Questions.First().AnswerOptions.First().Value);
        }
    }
}
