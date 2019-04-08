using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LifeCouple.DAL.Entities
{
    public class UserQuestionnaire : CosmosDbEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("dtCreated")]
        public DateTimeOffset DTCreated { get; set; }

        [JsonProperty("dtLastUpdated")]
        public DateTimeOffset DTLastUpdated { get; set; }

        [JsonProperty("userprofileId")]
        public string UserprofileId { get; set; }

        [JsonProperty("questionnaireTemplateId")]
        public string QuestionnaireTemplateId { get; set; }

        /// <summary>
        /// To make it easier to search...
        /// </summary>
        [JsonProperty("questionnaireTemplateType")]
        public QuestionnaireTemplate.QuestionnaireType QuestionnaireTemplateType { get; set; }

        /// <summary>
        /// Set when all the questions in the QuestionnaireTemplate have been set(answered)
        /// </summary>
        [JsonProperty("isQuestionnaireTemplateComplete")]
        public bool IsQuestionnaireTemplateComplete { get; set; }

        [JsonProperty("answers")]
        public List<Answer> Answers { get; set; }

        public class Answer
        {
            [JsonProperty("questionId")]
            public string QuestionId { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("childAnswer")]
            public Answer ChildAnswer { get; set; }
        }
    }

}

