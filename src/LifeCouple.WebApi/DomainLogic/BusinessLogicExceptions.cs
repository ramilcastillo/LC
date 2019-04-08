using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.DomainLogic
{
    public class BusinessLogicException : ApplicationException
    {
        public override string Message => $"BusinessLogicException: {Result?.Failures?.Count} Validation Errors found";



        /// <summary>
        /// Shortcut to Result.FirstFailure.DebugMessage.
        /// </summary>
        public string DebugMessage => Result.FirstFailure?.DebugMessage;

        /// <summary>
        /// Shortcut to Result.FirstFailure.ErrorCode.
        /// </summary>
        public ValidationErrorCode ErrorCode
        {
            get
            {
                if (Result.FirstFailure == null)
                {
                    return ValidationErrorCode.Empty;
                }
                else
                {
                    return Result.FirstFailure.Code;
                }
            }
        }

        /// <summary>
        /// Shortcut to Result.FirstFailure.EntityIdInError.
        /// </summary>
        public string EntityIdInError => Result.FirstFailure?.EntityIdInError;

        /// <summary>
        /// Shortcut to Result.FirstFailure.EntityJsonRepresentation.
        /// </summary>
        public string EntityJsonRepresentation => Result.FirstFailure?.EntityJsonRepresentation;

        public ValidationResult Result { get; set; }

        public BusinessLogicException() : base() => Result = new ValidationResult();

        public BusinessLogicException(string debugMessage) : this(ValidationErrorCode.Empty, null, debugMessage) { }

        public BusinessLogicException(ValidationErrorCode errorCode, string entityIdInError) : this(errorCode, entityIdInError, null) { }

        public BusinessLogicException(ValidationErrorCode errorCode, string entityIdInError, string debugmessage) : this() => Result.Failures.Add(new ValidationFailure
        {
            Code = errorCode,
            EntityIdInError = entityIdInError,
            DebugMessage = debugmessage
        });

        public BusinessLogicException(ValidationResult validationResult) : this() => Result = validationResult;


    }


}
