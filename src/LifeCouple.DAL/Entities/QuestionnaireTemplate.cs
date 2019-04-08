using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace LifeCouple.DAL.Entities
{

    public class QuestionnaireTemplate : CosmosDbEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [Required]
        [JsonProperty("dtCreated")]
        public DateTimeOffset DTCreated { get; set; }

        [Required]
        [JsonProperty("dtLastUpdated")]
        public DateTimeOffset DTLastUpdated { get; set; }

        [JsonProperty("typeOfQuestionnaire")]
        public QuestionnaireType TypeOfQuestionnaire { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("typeOfGender")]
        public GenderType TypeOfGender { get; set; }

        [JsonProperty("questionnaireSets")]
        public List<QuestionnaireSet> QuestionnaireSets { get; set; }


        [JsonConverter(typeof(StringEnumConverter))]
        public enum QuestionnaireType
        {
            TestNotInUse = -1,
            Unknown = 0,
            OnBoarding = 1,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum GenderType
        {
            Unknown = 0,
            Male = 1,
            Female = 2,
            Any = 3,
        }

    }

    public class QuestionnaireSet
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("nextButtonText")]
        public string NextButtonText { get; set; }

        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }
    }

    public class Question
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("typeOfQuestion")]
        public QuestionType TypeOfQuestion { get; set; }

        [JsonProperty("minRange")]
        public int? MinRange { get; set; }

        [JsonProperty("maxRange")]
        public int? MaxRange { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("indexesImpacted")]
        public List<IndexImpact> IndexesImpacted { get; set; }

        [JsonProperty("answerOptions")]
        public List<AnswerOption> AnswerOptions { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum QuestionType
        {
            YesNo = 1,
            MultipleOptionsSingleChoice = 2,
            MultipleOptionsMultipleChoice = 3,
            Range = 4,
        }
    }

    public class IndexImpact
    {
        [JsonProperty("typeOfIndex")]
        public IndexTypeEnum TypeOfIndex { get; set; }

        [JsonProperty("calculationWeight")]
        public decimal CalculationWeight { get; set; }
    }

    public class AnswerOption
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("childQuestion")]
        public Question ChildQuestion { get; set; }
    }

    public static class QuestionnaireTemplateHelper
    {
        public static int GetNrOfQuestionsEx(this QuestionnaireTemplate questionnaireTemplate)
        {
            return questionnaireTemplate.QuestionnaireSets.Select(e => e.Questions.Count).Aggregate((a, b) => { return a + b; });
        }
    }
}
