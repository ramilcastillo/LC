namespace LifeCouple.WebApi.DomainLogic
{
    public enum ValidationErrorCode
    {
        Empty = 0,

        UnhandledException = 1,

        /// <summary>
        /// Given Id not found
        /// </summary>
        NotFound = 4000,

        /// <summary>
        /// Given User Profile Id not found in the repo.
        /// </summary>
        UserProfileIdNotFound = 4001,

        /// <summary>
        /// Given QuestionnaireTemplate Id not found in the repo.
        /// </summary>
        QuestionnaireTemplateIdNotFound = 4002,



        EntityIsNull = 5000,
        /// <summary>
        /// Required Entity Id or foreign Id is null.
        /// </summary>
        IdIsNull = 5001,
        
        IdIsNull_UserProfileId = 5002,
        IdIsNull_QuestionnaireTemplateId = 5003,
        LastNameIsNull = 5004,
        FirstNameIsNull = 5005,
        DateOfBirthIsNull = 5006,
        MobilePhoneIsNull = 5007,
        EmailIsNull = 5008,


        ValidationFailure = 6000,

        /// <summary>
        /// Validation of UserQuestionnaire failed.
        /// </summary>
        ValidationFailure_UserQuestionnaire = 6100,
        /// <summary>
        /// Validation of UserQuestionnaire related to Answers failed.
        /// </summary>
        ValidationFailure_UserQuestionnaire_Answers = 6101,
        /// <summary>
        /// Validation of UserQuestionnaire related to Answers failed. Value of Answer is not valid.
        /// </summary>
        ValidationFailure_UserQuestionnaire_Answers_InvalidValue = 6102,

        ValidationFailure_PartnerInvitation = 6200,
        ValidationFailure_PartnerInvitationCannotSendSms = 6201,
        ValidationFailure_PartnerInvitationCannotSetStatus = 6203,
        Gender_ValidationError = 6300,
        Email_ValidationError = 6400,
        MobilePhone_ValidationError = 6500,
        DateOfBirth_ValidationError = 6600,

        ValidationFailure_UserProfile_AlreadyExists = 6701,
        ValidationFailure_UserProfile_PrivacyPolicy = 6702,
        ValidationFailure_UserProfile_TermsOfService = 6703,
        ValidationFailure_UserProfile_RelationshipId = 6704,
        EbaPointValueNotWithinAllowedSet = 6705,
        EbaPointPostingDTInvalid = 6706,
        EbaPointCommentExceedMaxLength = 6707,
        EbaPointCommentEmpty = 6708,
        EbaPointCommentShorterThanMinLength = 6709,

        ValidationCannotBeCompleted = 9000,
        
    }
}
