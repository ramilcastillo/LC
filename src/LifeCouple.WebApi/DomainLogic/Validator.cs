using System;
using System.Linq;
using LifeCouple.DAL.Entities;
using LifeCouple.Server.Messaging;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;

namespace LifeCouple.WebApi.DomainLogic
{
    public class Validator
    {
        private readonly PhoneService _phoneService;

        public Validator(PhoneService phoneService)
        {
            _phoneService = phoneService;
        }

        /// <summary>
        /// Helper to get json representation of any object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetJson<T>(T entity) => Newtonsoft.Json.JsonConvert.SerializeObject(entity, Newtonsoft.Json.Formatting.Indented);

        public bool IsValid<T>(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            if (entity is QuestionnaireTemplate)
            {
                return ValidateQuestionnaireTemplate(entity as QuestionnaireTemplate);
            }

            throw new NotSupportedException($"No validation logic implented for {entity}");
        }

        public bool ValidateQuestionnaireTemplate(QuestionnaireTemplate questionnaireTemplate)
        {
            if (questionnaireTemplate == null)
            {
                throw new ArgumentNullException("newQuestionnaire");
            }

            if (!string.IsNullOrWhiteSpace(questionnaireTemplate.Id))
            {
                throw new ArgumentOutOfRangeException($"id of entity must be empty, but it was '{questionnaireTemplate.Id}'");
            }

            if (questionnaireTemplate.QuestionnaireSets == null || questionnaireTemplate.QuestionnaireSets.Count == 0)
            {
                throw new ArgumentOutOfRangeException($"must have at least 1 QuestionnaireSets");
            }


            foreach (var qSet in questionnaireTemplate.QuestionnaireSets)
            {
                if (!string.IsNullOrWhiteSpace(qSet?.Id))
                {
                    throw new ArgumentOutOfRangeException($"id of entity must be empty, but it was '{qSet?.Id}'");
                }

                if (qSet.Questions == null || qSet.Questions.Count == 0)
                {
                    throw new ArgumentOutOfRangeException($"each QuestionnaireSets needs at least 1 Question");
                }

                foreach (var q in qSet.Questions)
                {
                    if (!string.IsNullOrWhiteSpace(q?.Id))
                    {
                        throw new ArgumentOutOfRangeException($"id of entity must be empty, but it was '{q?.Id}'");
                    }

                    if (q.TypeOfQuestion != Question.QuestionType.Range && (q.AnswerOptions == null || q.AnswerOptions.Count == 0))
                    {
                        throw new ArgumentOutOfRangeException($"each Question needs at least 1 AnswerOption");
                    }

                    if (q.TypeOfQuestion == Question.QuestionType.Range && (q.MinRange == null || q.MaxRange == null))
                    {
                        throw new ArgumentOutOfRangeException($"each Question of type { q.TypeOfQuestion.ToString()} needs to have a Min and Max range");
                    }

                    if (q.TypeOfQuestion == Question.QuestionType.Range && q.AnswerOptions != null)
                    {
                        throw new ArgumentOutOfRangeException($"each Question of type { q.TypeOfQuestion.ToString()} may NOT have any Answers");
                    }

                    if (q.TypeOfQuestion != Question.QuestionType.Range)
                    {
                        foreach (var answer in q.AnswerOptions)
                        {
                            if (string.IsNullOrWhiteSpace(answer.Text) || string.IsNullOrWhiteSpace(answer.Value))
                            {
                                throw new ArgumentOutOfRangeException($"each AnswerOption require both Text and Value to be set, question {q.Title} failed this validation");
                            }

                            if (answer.ChildQuestion != null)
                            {
                                if (!string.IsNullOrWhiteSpace(answer.ChildQuestion.Id))
                                {
                                    throw new ArgumentOutOfRangeException($"id of entity must be empty, but it was '{answer.ChildQuestion.Id}'");
                                }

                                if (answer.ChildQuestion.AnswerOptions == null || answer.ChildQuestion.AnswerOptions.Count == 0)
                                {
                                    throw new ArgumentOutOfRangeException($"each Question needs at least 1 AnswerOption");
                                }
                            }
                        }
                    }
                }

            }

            if (questionnaireTemplate.TypeOfGender == QuestionnaireTemplate.GenderType.Unknown)
            {
                throw new ArgumentOutOfRangeException($"'{questionnaireTemplate.TypeOfGender} is not a valid gender");
            }

            if (questionnaireTemplate.TypeOfQuestionnaire == QuestionnaireTemplate.QuestionnaireType.Unknown)
            {
                throw new ArgumentOutOfRangeException($"'{questionnaireTemplate.TypeOfQuestionnaire} is not a valid type");
            }
            return true;
        }


        /// <summary>
        /// Generic method to do basic validation that does not require any prior access to repo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ValidationResult Validate<T>(T entity) where T : CosmosDbEntity
        {
            var result = new ValidationResult();

            if (entity == null)
            {
                result.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EntityIsNull,
                    DebugMessage = "entity is not null and cannot be validated any further."
                });
                return ReturnResult();
            }
            else if (entity is UserQuestionnaire)
            {
                ValidateUserQuestionnaire(entity as UserQuestionnaire, result);
                return ReturnResult();
            }
            else if (entity is UserProfile)
            {
                Validate_UserProfile(entity as UserProfile, result);
                return result;
            }
            else
            {
                result.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.ValidationCannotBeCompleted,
                    DebugMessage = $"{entity.GetType()} cannot be validated since there is not Validate method implemented for that type."
                });
                return ReturnResult();
            }

            ValidationResult ReturnResult()
            {
                return result;
            }
        }


        public void ValidateUserQuestionnaire(UserQuestionnaire entity, QuestionnaireTemplate questionnaireTemplate, ValidationResult validation)
        {
            if (validation.IsValid == false)
            {
                return;
            }

            if (questionnaireTemplate == null)
            {
                validation.Failures.Add(new ValidationFailure { Code = ValidationErrorCode.QuestionnaireTemplateIdNotFound, EntityIdInError = entity.QuestionnaireTemplateId, EntityJsonRepresentation = Validator.GetJson(entity) });
                return;
            }

            if (entity.IsQuestionnaireTemplateComplete)
            {
                validation.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.ValidationFailure_UserQuestionnaire,
                    EntityIdInError = entity.Id,
                    DebugMessage = $"UserQuestionnaire with Id:{entity.Id} for QuestionnaireTemplate with Id:{questionnaireTemplate.Id} can not be updated since it is already Completed",
                    EntityJsonRepresentation = Validator.GetJson(entity)
                });
            }

            if (questionnaireTemplate.QuestionnaireSets.Count > 1)
            {
                validation.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.ValidationCannotBeCompleted,
                    EntityIdInError = entity.Id,
                    DebugMessage = $"UserQuestionnaire with Id:{entity.Id} for QuestionnaireTemplate with Id:{questionnaireTemplate.Id} have more than 1 QuestionnaireSets and there is no validation implemented for that yet",
                    EntityJsonRepresentation = Validator.GetJson(entity)
                });
            }

            if (entity.Answers != null)
            {

                foreach (var answer in entity.Answers)
                {
                    var question = questionnaireTemplate.QuestionnaireSets.First().Questions.SingleOrDefault(e => e.Id == answer.QuestionId);

                    //check if questionid exists in template
                    if (question == null)
                    {
                        validation.Failures.Add(new ValidationFailure
                        {
                            Code = ValidationErrorCode.ValidationFailure_UserQuestionnaire_Answers,
                            EntityIdInError = entity.Id,
                            DebugMessage = $"Question Id:{answer.QuestionId} in UserQuestionnaire with Id:{entity.Id} does not exist in QuestionnaireTemplate with Id:{questionnaireTemplate.Id}",
                            EntityJsonRepresentation = Validator.GetJson(entity)
                        });
                    }
                    //exists, so check if value is valid
                    else
                    {
                        switch (question.TypeOfQuestion)
                        {
                            case Question.QuestionType.Range:
                                int intValue;
                                if (int.TryParse(answer.Value, out intValue))
                                {
                                    if (false == (intValue >= question.MinRange.Value && intValue <= question.MaxRange.Value))
                                    {
                                        validation.Failures.Add(new ValidationFailure
                                        {
                                            Code = ValidationErrorCode.ValidationFailure_UserQuestionnaire_Answers_InvalidValue,
                                            EntityIdInError = entity.Id,
                                            DebugMessage = $"The value '{answer.Value}' for Question Id:{question.Id} in UserQuestionnaire with Id:{entity.Id} for QuestionnaireTemplate with Id:{questionnaireTemplate.Id} is out of range, valid range is '{question.MinRange}' to '{question.MaxRange}'",
                                            EntityJsonRepresentation = Validator.GetJson(entity)
                                        });
                                    }
                                }
                                else
                                {
                                    validation.Failures.Add(new ValidationFailure
                                    {
                                        Code = ValidationErrorCode.ValidationFailure_UserQuestionnaire_Answers_InvalidValue,
                                        EntityIdInError = entity.Id,
                                        DebugMessage = $"The value '{answer.Value}' for Question Id:{question.Id} in UserQuestionnaire with Id:{entity.Id} for QuestionnaireTemplate with Id:{questionnaireTemplate.Id} is invalid, valid range is '{question.MinRange}' to '{question.MaxRange}'",
                                        EntityJsonRepresentation = Validator.GetJson(entity)
                                    });
                                }

                                break;
                            default:
                                validation.Failures.Add(new ValidationFailure
                                {
                                    Code = ValidationErrorCode.ValidationCannotBeCompleted,
                                    EntityIdInError = entity.Id,
                                    DebugMessage = $"Question Id:{question.Id} is of type '{question.TypeOfQuestion}' in QuestionnaireTemplate with Id:{questionnaireTemplate.Id} and there is no validation implemented for that yet",
                                    EntityJsonRepresentation = Validator.GetJson(entity)
                                });
                                break;
                        }

                    }
                }
            }



        }

        public void ValidateUserQuestionnaire(UserQuestionnaire entity, ValidationResult validation)
        {
            var idIsNullMsg = $"One or more Ids in instance of type '{entity.GetType()}' is null.";

            if (string.IsNullOrWhiteSpace(entity.UserprofileId))
            {
                validation.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.IdIsNull_UserProfileId,
                    DebugMessage = idIsNullMsg,
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (string.IsNullOrWhiteSpace(entity.QuestionnaireTemplateId))
            {
                validation.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.IdIsNull_QuestionnaireTemplateId,
                    DebugMessage = idIsNullMsg,
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (entity.Answers == null || entity.Answers.Count == 0)
            {
                validation.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.ValidationFailure_UserQuestionnaire_Answers,
                    DebugMessage = $"Need at least one Answer for instance of type '{entity.GetType()}'.",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

        }

        public bool CanUpdateUserprofile(UserProfile existingUserprofile, BL_UserProfile bl_newUserprofile, out ValidationResult results)
        {
            results = new ValidationResult();

            if (existingUserprofile == null)
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EntityIsNull,
                    DebugMessage = @"Existing userprofile is null. Cannot update userprofile",
                    EntityJsonRepresentation = null,
                    EntityIdInError = bl_newUserprofile.Id,
                });
                return false; //no need to do any further validations
            }

            if (existingUserprofile.HasAgreedToPrivacyPolicy == true && bl_newUserprofile.HasAgreedToPrivacyPolicy == false)
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.ValidationFailure_UserProfile_PrivacyPolicy,
                    DebugMessage = $"Cannot set value from '{existingUserprofile.HasAgreedToPrivacyPolicy}' to '{bl_newUserprofile.HasAgreedToPrivacyPolicy}'",
                    EntityJsonRepresentation = GetJson(existingUserprofile),
                    EntityIdInError = bl_newUserprofile.Id,
                });
            }

            if (existingUserprofile.HasAgreedToTandC == true && bl_newUserprofile.HasAgreedToTandC == false)
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.ValidationFailure_UserProfile_PrivacyPolicy,
                    DebugMessage = $"Cannot set value from '{existingUserprofile.HasAgreedToTandC}' to '{bl_newUserprofile.HasAgreedToTandC}'",
                    EntityJsonRepresentation = GetJson(existingUserprofile),
                    EntityIdInError = bl_newUserprofile.Id,
                });
            }

            return results.IsValid;
        }

        public bool CanInvitePartner(UserProfile entity, out ValidationResult results)
        {
            results = new ValidationResult();

            if (entity.PartnerInvitation == null)
            {
                return true;
            }
            else
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.ValidationFailure_PartnerInvitation,
                    DebugMessage = @"Partner invitation already exists, invitation cannot be submitted",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
                return false;
            }
        }


        public bool CanSendSmsInvitationToPartner(UserProfile entity, out ValidationResult results)
        {
            results = new ValidationResult();

            if (entity.PartnerInvitation != null && entity.PartnerInvitation.InvitationStatus != PartnerInvitationStatusTypeEnum.Accepted && entity.PartnerInvitation.MobilePhone.IsValidated)
            {
                return true;
            }
            else
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.ValidationFailure_PartnerInvitationCannotSendSms,
                    DebugMessage = @"Invitation Sms cannot be sent. Status of invitation is not valid or phone nr is not valid",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
                return false;
            }
        }

        public bool CanSetPartnerInviteStatus(UserProfile entity, BL_PartnerInvitationStatusTypeEnum newStatus, out ValidationResult results)
        {
            results = new ValidationResult();

            if (entity == null)
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EntityIsNull,
                    DebugMessage = $"Invitation Status cannot be set since the UserProfile is null",
                });
                return false;
            }


            if (entity.PartnerInvitation == null)
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.ValidationFailure_PartnerInvitationCannotSetStatus,
                    DebugMessage = $"Invitation Status cannot be set since the PartnerInvitation is null",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
                return false;
            }

            if (newStatus == BL_PartnerInvitationStatusTypeEnum.Accepted &&
                (
                entity.PartnerInvitation.InvitationStatus == PartnerInvitationStatusTypeEnum.Accepted
                || entity.PartnerInvitation.InvitationStatus == PartnerInvitationStatusTypeEnum.Declined
                || entity.PartnerInvitation.InvitationStatus == PartnerInvitationStatusTypeEnum.Sent
                || entity.PartnerInvitation.InvitationStatus == PartnerInvitationStatusTypeEnum.Submitted
                ))
            {
                return true;
            }

            if (newStatus == BL_PartnerInvitationStatusTypeEnum.Declined &&
                (
                entity.PartnerInvitation.InvitationStatus == PartnerInvitationStatusTypeEnum.Accepted
                || entity.PartnerInvitation.InvitationStatus == PartnerInvitationStatusTypeEnum.Declined
                || entity.PartnerInvitation.InvitationStatus == PartnerInvitationStatusTypeEnum.Sent
                || entity.PartnerInvitation.InvitationStatus == PartnerInvitationStatusTypeEnum.Submitted
                ))
            {
                return true;
            }

            if (newStatus == BL_PartnerInvitationStatusTypeEnum.Completed &&
                (
                entity.PartnerInvitation.InvitationStatus == PartnerInvitationStatusTypeEnum.Accepted
                ))
            {
                return true;
            }

            results.Failures.Add(new ValidationFailure
            {
                Code = ValidationErrorCode.ValidationFailure_PartnerInvitationCannotSetStatus,
                DebugMessage = $"Invitation Status cannot be set. Status of invitation is not valid and cannot be set from {entity.PartnerInvitation.InvitationStatus} to {newStatus}",
                EntityJsonRepresentation = GetJson(entity),
                EntityIdInError = entity.Id,
            });

            return false;

        }

        public void Validate_UserProfile(UserProfile entity, ValidationResult validation)
        {
            var idIsNullMsg = $"One or more Ids in instance of type '{entity.GetType()}' is null.";

            if (string.IsNullOrWhiteSpace(entity.PrimaryEmail))
            {
                validation.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EmailIsNull,
                    DebugMessage = "Email cannot be cannot be empty",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (!entity.PrimaryEmail.Contains("@"))
            {
                validation.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.Email_ValidationError,
                    DebugMessage = "Email is not valid",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }


            if (entity.PartnerInvitation != null && entity.PartnerInvitation.InvitationStatus != PartnerInvitationStatusTypeEnum.NotApplicable)
            {
                if (entity.PartnerInvitation.DateOfBirth == null)
                {
                    validation.Failures.Add(new ValidationFailure
                    {
                        Code = ValidationErrorCode.DateOfBirthIsNull,
                        DebugMessage = "Date of birth cannot be empty",
                        EntityJsonRepresentation = GetJson(entity),
                        EntityIdInError = entity.Id,
                    });
                }
                else
                {
                    if (entity.PartnerInvitation.DateOfBirth.Value.Kind != DateTimeKind.Unspecified)
                    {
                        validation.Failures.Add(new ValidationFailure
                        {
                            Code = ValidationErrorCode.DateOfBirth_ValidationError,
                            DebugMessage = $"Partner date of birth is invalid, DateTime.Kind needs to be 'Unspecified'",
                            EntityJsonRepresentation = GetJson(entity),
                            EntityIdInError = entity.Id,
                        });
                    }

                    if (entity.PartnerInvitation.DateOfBirth.Value.Hour != 0 || entity.PartnerInvitation.DateOfBirth.Value.Minute != 0 || entity.PartnerInvitation.DateOfBirth.Value.Second != 0)
                    {
                        validation.Failures.Add(new ValidationFailure
                        {
                            Code = ValidationErrorCode.DateOfBirth_ValidationError,
                            DebugMessage = $"Partner date of birth is invalid, may only contain Year, Month and Day",
                            EntityJsonRepresentation = GetJson(entity),
                            EntityIdInError = entity.Id,
                        });
                    }

                    var minAge = 16;
                    if (entity.PartnerInvitation.DateOfBirth > DateTime.Now.AddYears(-minAge))
                    {
                        validation.Failures.Add(new ValidationFailure
                        {
                            Code = ValidationErrorCode.ValidationFailure_PartnerInvitation,
                            DebugMessage = $"Partner must be at least {minAge} years old",
                            EntityJsonRepresentation = GetJson(entity),
                            EntityIdInError = entity.Id,
                        });
                    }
                }

                if (string.IsNullOrWhiteSpace(entity.PartnerInvitation.FirstName))
                {
                    validation.Failures.Add(new ValidationFailure
                    {
                        Code = ValidationErrorCode.FirstNameIsNull,
                        DebugMessage = "FirstName cannot be empty",
                        EntityJsonRepresentation = GetJson(entity),
                        EntityIdInError = entity.Id,
                    });
                }

                if (false == (entity.PartnerInvitation.Gender == GenderTypeEnum.Male || entity.PartnerInvitation.Gender == GenderTypeEnum.Female))
                {
                    validation.Failures.Add(new ValidationFailure
                    {
                        Code = ValidationErrorCode.Gender_ValidationError,
                        DebugMessage = "Gender is invalid, must be set",
                        EntityJsonRepresentation = GetJson(entity),
                        EntityIdInError = entity.Id,
                    });
                }

                if (entity.PartnerInvitation.InvitationStatus == PartnerInvitationStatusTypeEnum.Unknown)
                {
                    validation.Failures.Add(new ValidationFailure
                    {
                        Code = ValidationErrorCode.ValidationFailure_PartnerInvitation,
                        DebugMessage = "Partner invitation status is invalid",
                        EntityJsonRepresentation = GetJson(entity),
                        EntityIdInError = entity.Id,
                    });
                }

                if (string.IsNullOrWhiteSpace(entity.PartnerInvitation.LastName))
                {
                    validation.Failures.Add(new ValidationFailure
                    {
                        Code = ValidationErrorCode.LastNameIsNull,
                        DebugMessage = "Lastname cannot be empty",
                        EntityJsonRepresentation = GetJson(entity),
                        EntityIdInError = entity.Id,
                    });
                }

                if (string.IsNullOrWhiteSpace(entity.PartnerInvitation.MobilePhone?.UserEnteredNr))
                {
                    validation.Failures.Add(new ValidationFailure
                    {
                        Code = ValidationErrorCode.MobilePhoneIsNull,
                        DebugMessage = "Mobile phone cannot be empty",
                        EntityJsonRepresentation = GetJson(entity),
                        EntityIdInError = entity.Id,
                    });
                }

                if (false == string.IsNullOrWhiteSpace(entity.PartnerInvitation.MobilePhone?.UserEnteredNr))
                {
                    if (false == entity.PartnerInvitation.MobilePhone.IsValidated)
                    {
                        var task = _phoneService.GetPhoneNrInfo(entity.PartnerInvitation.MobilePhone.UserEnteredNr);
                        var phoneInfo = task.Result;
                        if (false == phoneInfo.IsValid || phoneInfo.TypeOfNumber != PhoneNrInfo.TypeOfNumberEnum.Mobile)
                        {
                            validation.Failures.Add(new ValidationFailure
                            {
                                Code = ValidationErrorCode.MobilePhone_ValidationError,
                                DebugMessage = $"Mobile phone nr '{entity.PartnerInvitation.MobilePhone?.UserEnteredNr}' is not valid",
                                EntityJsonRepresentation = GetJson(entity),
                                EntityIdInError = entity.Id,
                            });
                        }
                        else
                        {
                            entity.PartnerInvitation.MobilePhone.CountryCode = phoneInfo.CountryCode;
                            entity.PartnerInvitation.MobilePhone.IsValidated = true;
                            entity.PartnerInvitation.MobilePhone.NationalFormat = phoneInfo.NationalFormat;
                            entity.PartnerInvitation.MobilePhone.PhoneNr = phoneInfo.PhoneNr;
                            switch (phoneInfo.TypeOfNumber)
                            {
                                case PhoneNrInfo.TypeOfNumberEnum.Mobile:
                                    entity.PartnerInvitation.MobilePhone.TypeOfNumber = Phone.TypeOfNumberEnum.Mobile;
                                    break;
                                case PhoneNrInfo.TypeOfNumberEnum.Other:
                                    entity.PartnerInvitation.MobilePhone.TypeOfNumber = Phone.TypeOfNumberEnum.Other;
                                    break;
                                default:
                                    entity.PartnerInvitation.MobilePhone.TypeOfNumber = Phone.TypeOfNumberEnum.Unknown;
                                    break;
                            }

                        }

                    }

                }

            }



        }

        public bool CanSetEbaPoints(EbaPointsSent entity, BL_Settings settings, out ValidationResult results)
        {
            results = new ValidationResult();

            if (entity == null)
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EntityIsNull,
                    DebugMessage = $"Entity: {nameof(entity)} is null",
                });
                return false;
            }

            if (string.IsNullOrEmpty(entity?.UserprofileId))
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.IdIsNull_UserProfileId,
                    DebugMessage = $"Entity: {entity.GetType().Name} have Userprofile Id that is null",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (string.IsNullOrEmpty(entity.RelationshipId))
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.IdIsNull,
                    DebugMessage = $"Entity: {entity.GetType().Name} have Relationship that is null",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            var mostRecentTransaction = entity?.Transactions?.FirstOrDefault();

            if (mostRecentTransaction?.PostedDT < DateTimeOffset.UtcNow.AddSeconds(-5) || mostRecentTransaction?.PostedDT > DateTimeOffset.UtcNow.AddSeconds(5))
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EbaPointPostingDTInvalid,
                    DebugMessage = $"Entity: {entity.GetType().Name} have a transaction posting DT that is off more than 5 seconds",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (string.IsNullOrWhiteSpace(mostRecentTransaction?.Comment))
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EbaPointCommentEmpty,
                    DebugMessage = $"Entity: {entity.GetType().Name} have a transaction Comment that is empty",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (mostRecentTransaction?.Comment?.Length < 3)
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EbaPointCommentShorterThanMinLength,
                    DebugMessage = $"Entity: {entity.GetType().Name} have a transaction Comment that is less than 3 characters",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (mostRecentTransaction?.Comment?.Length > 1500)
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EbaPointCommentExceedMaxLength,
                    DebugMessage = $"Entity: {entity.GetType().Name} have a transaction Comment that is longer than 1500",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (string.IsNullOrWhiteSpace(mostRecentTransaction?.Key) || mostRecentTransaction?.Key != mostRecentTransaction?.Key?.ToUpper())
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.Empty,
                    DebugMessage = $"Entity: {entity.GetType().Name} have a transaction Key which is invalid",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (mostRecentTransaction?.Points == null || false == settings.EmotionalBankAccontPointOptions.ContainsKey(mostRecentTransaction.Points))
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EbaPointValueNotWithinAllowedSet,
                    DebugMessage = $"Entity: {entity.GetType().Name} have a transaction point value that is not within allowed set of valid points",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }


            return results.IsValid;
        }

        public bool CanSetAppCenterDetails(AppCenterDeviceDetail entity, out ValidationResult results)
        {
            results = new ValidationResult();

            if (entity == null)
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.EntityIsNull,
                    DebugMessage = $"Entity: {nameof(entity)} is null",
                });
                return false;
            }

            if (string.IsNullOrEmpty(entity.UserprofileId))
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.IdIsNull_UserProfileId,
                    DebugMessage = $"Entity: {entity.GetType().Name} have Userprofile Id that is null",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (string.IsNullOrEmpty(entity.DeviceId))
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.IdIsNull,
                    DebugMessage = $"Entity: {entity.GetType().Name} have DeviceId that is null",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            if (entity.TypeOfDeviceOs == DeviceOsTypeEnum.Unknown)
            {
                results.Failures.Add(new ValidationFailure
                {
                    Code = ValidationErrorCode.IdIsNull,
                    DebugMessage = $"Entity: {entity.GetType().Name} have Unknown DeviceOsType",
                    EntityJsonRepresentation = GetJson(entity),
                    EntityIdInError = entity.Id,
                });
            }

            return results.IsValid;

        }
    }
}
