using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.DomainLogic
{
    public class ValidationResult
    {
        /// <summary>
        /// Null means it is not validated yet.
        /// </summary>
        public bool IsValid => Failures.Count == 0;

        public List<ValidationFailure> Failures { get; private set; }

        public ValidationFailure FirstFailure
        {
            get
            {
                if (Failures.Count == 0)
                {
                    return null;
                }
                else
                {
                    return Failures.First();
                }
            }
        }

        public ValidationResult()
        {
            Failures = new List<ValidationFailure>();
        }
    }

    public class ValidationFailure
    {
        public ValidationErrorCode Code { get; set; }
        public string DebugMessage { get; set; }
        public string EntityIdInError { get; set; }
        public string EntityJsonRepresentation { get; set; }
    }
}
