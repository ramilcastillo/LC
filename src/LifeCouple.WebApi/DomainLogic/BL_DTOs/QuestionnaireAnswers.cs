using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.DomainLogic
{
    public class BL_QuestionnaireAnswers
    {
        public string UserprofileId { get; set; }

        public string QuestionnaireTemplateId { get; set; }

        public List<Answer> Answers { get; set; }

        public class Answer
        {
            public string QuestionId { get; set; }

            public string Value { get; set; }

            public Answer ChildAnswer { get; set; }
        }
    }
}
