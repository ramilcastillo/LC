using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeCouple.DAL;
using LifeCouple.DAL.Entities;
using LifeCouple.Server.Messaging;
using Microsoft.Extensions.Caching.Memory;

namespace LifeCouple.WebApi.DomainLogic
{
    public partial class BusinessLogic
    {
        protected readonly IRepository _repo;
        protected readonly PhoneService _phoneService;
        protected readonly Map map;
        protected readonly Validator validator;
        protected readonly IMemoryCache _cache;
        protected readonly CacheService _cacheService;

        private static readonly TimeSpan cacheExpiration = new TimeSpan(0, 5, 0);

        public BusinessLogic(IRepository repository, PhoneService phoneService, IMemoryCache memoryCache)
        {
            _repo = repository;
            _phoneService = phoneService;
            map = new Map();
            validator = new Validator(phoneService);
            _cache = memoryCache;
            _cacheService = new CacheService(_cache);
        }

        public bool IsDbAccesible()
        {
            try
            {
                var r = _repo.IsDbAccessible();
                return r;
            }
            catch (Exception)
            {
                //TODO: C Add logging
                return false;
            }

        }

        public async Task<BL_UserProfile> CreateUserAsync(string primaryEmail, string externalRefId, bool isDevTest, string firstName, string lastName, string relationshipId)
        {
            var newUser = new UserProfile()
            {
                PrimaryEmail = primaryEmail,
                ExternalRefId = externalRefId,
                IsDevTest = isDevTest,
                FirstName = firstName,
                LastName = lastName,
                Relationship_Id = relationshipId
            };

            var existingUserProfile = await _repo.GetUserProfile_byExtRefIdAsync(newUser.ExternalRefId);
            if (existingUserProfile != null)
            {
                throw new BusinessLogicException(ValidationErrorCode.ValidationFailure_UserProfile_AlreadyExists, existingUserProfile.Id, $"Unable to create user with ExtRefenceNr: '{externalRefId}' and Email: '{primaryEmail}' since it already exists.");
            }

            var isUserInvitee = false;
            if (false == string.IsNullOrEmpty(newUser.Relationship_Id))
            {
                var existingUserProfilesWithSameRelatioshipId = await _repo.FindUserProfiles_byRelationshipId(newUser.Relationship_Id);
                if (existingUserProfilesWithSameRelatioshipId.Count == 1)
                {
                    isUserInvitee = true;
                }
                else
                {
                    throw new BusinessLogicException(ValidationErrorCode.ValidationFailure, newUser.Relationship_Id, $"Unable to create user with ExtRefenceNr: '{externalRefId}' and Email: '{primaryEmail}' since RelationsipId: {newUser.Relationship_Id} given needs to exists for 1 user prior to creating a new user with the same relationship id. Found {existingUserProfilesWithSameRelatioshipId.Count} other user with the same relationship Id");
                }
            }

            var validationResult = validator.Validate(newUser);

            if (validationResult.IsValid)
            {
                var userProfile = await _repo.CreateUserAsync(newUser.PrimaryEmail, newUser.ExternalRefId, newUser.IsDevTest, newUser.FirstName, newUser.LastName, newUser.Relationship_Id, isUserInvitee);
                return map.From(userProfile);
            }
            else
            {
                throw new BusinessLogicException(validationResult);
            }

        }

        public async Task<BL_UserProfile> GetUserProfile_byExtRefIdAsync(string externalRefId)
        {
            var userProfile = await _repo.GetUserProfile_byExtRefIdAsync(externalRefId);
            return map.From(userProfile);
        }



        /// <summary>
        /// to be used within web api controller(s)
        /// </summary>
        /// <param name="bL_QuestionnaireAnswers"></param>
        /// <returns></returns>
        public async Task<string> SetUserQuestionnaireAsync(BL_QuestionnaireAnswers bL_QuestionnaireAnswers)
        {
            var userQuestionnaire = new UserQuestionnaire
            {
                UserprofileId = bL_QuestionnaireAnswers.UserprofileId,
                QuestionnaireTemplateId = bL_QuestionnaireAnswers.QuestionnaireTemplateId,
                Answers = new List<UserQuestionnaire.Answer>()
            };

            foreach (var answer in bL_QuestionnaireAnswers.Answers)
            {
                if (answer.ChildAnswer != null)
                {
                    throw new BusinessLogicException(ValidationErrorCode.ValidationCannotBeCompleted, bL_QuestionnaireAnswers.QuestionnaireTemplateId,
                        $"An Answer for Question id '{answer.QuestionId}' for QuestionnaireTemplate id '{bL_QuestionnaireAnswers.QuestionnaireTemplateId}' for user id '{bL_QuestionnaireAnswers.UserprofileId}' contains a Child answer, which is not yet supported.");
                }

                userQuestionnaire.Answers.Add(new UserQuestionnaire.Answer
                {
                    QuestionId = answer.QuestionId,
                    Value = answer.Value
                });
            }

            var result = await SetUserQuestionniareAsync(userQuestionnaire);
            return result;
        }

        /// <summary>
        /// Return id updated or created
        /// </summary>
        /// <param name="userQuestionnaire"></param>
        /// <returns></returns>
        public async Task<string> SetUserQuestionniareAsync(UserQuestionnaire userQuestionnaire)
        {
            var userQ = DeepClone(userQuestionnaire); //Don't want to work with the passed in variable

            var validationResult = validator.Validate(userQ);

            UserQuestionnaire existingUserQ = null;

            //Get existing template
            var questionnaireTemplate = await _repo.GetQuestionnaireTemplate_ByIdAsync(userQ.QuestionnaireTemplateId);

            validator.ValidateUserQuestionnaire(userQ, questionnaireTemplate, validationResult);

            //Get existing user questionnaire since the incoming payload may only contain 1 question and there might already be a bunch saved, or this is the first one
            if (validationResult.IsValid)
            {
                existingUserQ = await _repo.GetUserQuestionnaire_ByUserIdAsync(userQ.UserprofileId, questionnaireTemplate.TypeOfQuestionnaire, questionnaireTemplate.Id);
                if (existingUserQ == null)
                {
                    if (await _repo.GetUserProfile_byIdAsync(userQ.UserprofileId) == null)
                    {
                        validationResult.Failures.Add(new ValidationFailure { Code = ValidationErrorCode.UserProfileIdNotFound, EntityIdInError = userQ.UserprofileId, EntityJsonRepresentation = Validator.GetJson(userQ) });
                    }
                }
                else
                {
                    validator.ValidateUserQuestionnaire(existingUserQ, questionnaireTemplate, validationResult);
                }
            }


            //Merge existing item with incoming userQuestionnaire
            if (validationResult.IsValid && existingUserQ != null)
            {
                foreach (var existingAnswer in existingUserQ.Answers)
                {
                    if (userQ.Answers.FirstOrDefault(e => e.QuestionId == existingAnswer.QuestionId) == null)
                    {
                        userQ.Answers.Add(existingAnswer);
                    }
                }
                SortQuestionsAccordingToQuestionnaireTemplate(userQ, questionnaireTemplate);
            }



            //Set/replace/create existing in Repo
            if (validationResult.IsValid)
            {
                //if all questions answered then mark as Complete, to not allow further updates
                if (questionnaireTemplate.GetNrOfQuestionsEx() == userQ.Answers.Count)
                {
                    userQ.IsQuestionnaireTemplateComplete = true;
                }

                userQ.Id = existingUserQ?.Id;

                var utcNow = DateTimeOffset.UtcNow;
                userQ.DTCreated = existingUserQ?.DTCreated ?? utcNow;
                userQ.DTLastUpdated = utcNow;

                userQ.QuestionnaireTemplateType = questionnaireTemplate.TypeOfQuestionnaire;

                var idUpdatedOrCreated = await _repo.SetUserQuestionnaireAsync(userQ);

                if (userQ.IsQuestionnaireTemplateComplete)
                {
                    await SetUserIndexes(questionnaireTemplate, userQ.UserprofileId);
                }

                return idUpdatedOrCreated;
            }
            else
            {
                throw new BusinessLogicException(validationResult);
            }


        }

        public async Task SetUserIndexes(QuestionnaireTemplate questionnaireTemplate, string userprofileId)
        {
            var uq = await GetUserQuestionnaire_ByUserIdAsync(userprofileId, questionnaireTemplate.TypeOfQuestionnaire, questionnaireTemplate.Id);
            if (!uq.IsQuestionnaireTemplateComplete)
            {
                throw new BusinessLogicException(ValidationErrorCode.ValidationFailure_UserQuestionnaire, uq.Id, $"UserQuestionnaire with Id '{uq.Id}' is not Complete so no indexes can be set for UserId '{userprofileId}'");
            }
            if (questionnaireTemplate.QuestionnaireSets?.Count != 1)
            {
                throw new BusinessLogicException(ValidationErrorCode.ValidationFailure, questionnaireTemplate.Id, $"QuestionnaireTemplate with Id '{questionnaireTemplate.Id}' is not supported since it contains more than 1 Set of Questions, so no indexes can be set for UserId '{userprofileId}'");
            }

            var bl_usrProReg = new BL_UserProfile
            {
                Id = userprofileId,
                Indexes = new List<BL_UserProfile.Index>()
            };

            var totalWeightForOverallIndex = 0.0M;
            foreach (var q in questionnaireTemplate.QuestionnaireSets.First().Questions)
            {
                foreach (var i in q.IndexesImpacted)
                {
                    var indexToUpdate = bl_usrProReg.Indexes.SingleOrDefault(e => e.TypeOfIndex == i.TypeOfIndex);
                    if (indexToUpdate == null)
                    {
                        indexToUpdate = new BL_UserProfile.Index { TypeOfIndex = i.TypeOfIndex, Value = 0 };
                        bl_usrProReg.Indexes.Add(indexToUpdate);
                    }

                    int.TryParse(uq.Answers.SingleOrDefault(e => e.QuestionId == q.Id).Value, out var answerValue);

                    indexToUpdate.Value += (int)(i.CalculationWeight * decimal.Parse(answerValue.ToString()));

                    if (indexToUpdate.TypeOfIndex == IndexTypeEnum.Overall)
                    {
                        totalWeightForOverallIndex += i.CalculationWeight;
                    }
                }
            }

            foreach (var i in bl_usrProReg.Indexes)
            {
                if (i.TypeOfIndex == IndexTypeEnum.Overall)
                {
                    var newValue = i.Value / totalWeightForOverallIndex * 10;
                    i.Value = int.Parse(Math.Round(newValue, MidpointRounding.AwayFromZero).ToString());
                }
            }

            await SetUserAsync(bl_usrProReg);
            //TODO: AAAA: persist Indexes in userIndexHistory
            //await SaveIndexHistory....

        }

        public async Task<QuestionnaireTemplate> GetQuestionnaireTemplate_ByUserQuestionnaire(UserQuestionnaire uq, QuestionnaireTemplate.QuestionnaireType questionnaireType, string gender)
        {
            QuestionnaireTemplate r;

            if (uq == null)
            {
                r = await GetQuestionnaireTemplate_ActiveByTypeAndGender(questionnaireType, gender);
            }
            else
            {
                if (string.IsNullOrEmpty(uq.QuestionnaireTemplateId))
                {
                    throw new BusinessLogicException(ValidationErrorCode.IdIsNull_QuestionnaireTemplateId, uq.Id, $"QuestionnaireTemplate Id in UserQuestionnaire with Id '{uq.Id}' is null");
                }
                else
                {
                    r = await _repo.GetQuestionnaireTemplate_ByIdAsync(uq.QuestionnaireTemplateId);
                    if (r == null)
                    {
                        throw new BusinessLogicException(ValidationErrorCode.EntityIsNull, uq.Id, $"QuestionnaireTemplate Id '{uq.QuestionnaireTemplateId}' in UserQuestionnaire with Id '{uq.Id}' can not be found in repo.");
                    }
                }
            }
            return r;
        }

        private void SortQuestionsAccordingToQuestionnaireTemplate(UserQuestionnaire _userQ, QuestionnaireTemplate _questionnaireT)
        {
            var answers = new List<UserQuestionnaire.Answer>(_userQ.Answers);
            _userQ.Answers.Clear();
            foreach (var set in _questionnaireT.QuestionnaireSets)
            {
                foreach (var q in set.Questions)
                {

                    if (answers.Exists(e => e.QuestionId == q.Id))
                    {
                        _userQ.Answers.Add(answers.Single(e1 => e1.QuestionId == q.Id));
                    }
                }
            }
            answers.Clear();
        }

        private static T DeepClone<T>(T entity)
        {
            var userQuestionnaire = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Newtonsoft.Json.JsonConvert.SerializeObject(entity, RepoHelpers.jsonSerializerSettings));
            return userQuestionnaire;
        }

        public async Task<List<BL_UserProfile>> FindUserProfiles_byEmailAsync(string email, bool? isDevTestUser)
        {
            var userProfiles = await _repo.FindUserProfiles_byPrimaryEmailAsync(email);
            if (isDevTestUser == true)
            {
                userProfiles = userProfiles.Where(e => e.IsDevTest == true).ToList();
            }
            if (isDevTestUser == false)
            {
                userProfiles = userProfiles.Where(e => e.IsDevTest == false).ToList();
            }

            return MapFrom(userProfiles);
        }

        public async Task SetUserAsync(BL_UserProfile userProfile)
        {
            var bl_entity = DeepClone(userProfile); //Don't want to work with the passed in variable

            if (string.IsNullOrWhiteSpace(userProfile.Id))
            {
                //new user, not yet support
                throw new NotImplementedException("Use CreateUserAsync instead");
            }

            var existingUser = await _repo.GetUserProfile_byIdAsync(userProfile.Id);

            //Adjust data 
            if (bl_entity.DateOfBirth != null) //Reset to only to Year, Month and Day
            {
                bl_entity.DateOfBirth = new DateTime(bl_entity.DateOfBirth.Value.Year, bl_entity.DateOfBirth.Value.Month, bl_entity.DateOfBirth.Value.Day, 0, 0, 0, DateTimeKind.Unspecified);
            }

            if (validator.CanUpdateUserprofile(existingUser, bl_entity, out var validationResult))
            {
                //Map incoming data to existing user (need to be mapped this way to not overwrite new properties not part of incoming BL_UserProfile)
                var utcNow = DateTimeOffset.UtcNow;
                existingUser.Country_Code = bl_entity.Country_Code ?? existingUser.Country_Code;
                existingUser.DateOfBirth = bl_entity.DateOfBirth ?? existingUser.DateOfBirth;
                existingUser.DTLastUpdated = utcNow;
                existingUser.ExternalRefId = existingUser.ExternalRefId; //Cannot be changed
                existingUser.FirstName = bl_entity.FirstName ?? existingUser.FirstName;
                existingUser.Gender = bl_entity.Gender ?? existingUser.Gender;
                existingUser.HasAgreedToPrivacyPolicy = bl_entity.HasAgreedToPrivacyPolicy ?? existingUser.HasAgreedToPrivacyPolicy;
                existingUser.HasAgreedToRefundPolicy = bl_entity.HasAgreedToRefundPolicy ?? existingUser.HasAgreedToRefundPolicy;
                existingUser.HasAgreedToTandC = bl_entity.HasAgreedToTandC ?? existingUser.HasAgreedToTandC;
                existingUser.Id = existingUser.Id;
                existingUser.Indexes = MapIndexes(bl_entity, existingUser, utcNow);
                existingUser.IsDevTest = existingUser.IsDevTest; //Cannot be changed
                existingUser.LastName = bl_entity.LastName ?? existingUser.LastName;
                existingUser.NotificationOption = bl_entity.NotificationOption ?? existingUser.NotificationOption;
                existingUser.PrimaryEmail = bl_entity.PrimaryEmail ?? existingUser.PrimaryEmail;
                existingUser.PrimaryMobileNr = bl_entity.PrimaryMobileNr ?? existingUser.PrimaryMobileNr;
                existingUser.Relationship_Id = existingUser.Relationship_Id;
                existingUser.Type = null; //set in repo anyway,

                validationResult = validator.Validate(existingUser);
            }

            if (validationResult.IsValid)
            {
                await _repo.SetUserProfileAsync(existingUser);
            }
            else
            {
                throw new BusinessLogicException(validationResult);
            }
        }

        List<UserProfile.Index> MapIndexes(BL_UserProfile new_bL_UserProfile, UserProfile existingUserProfile, DateTimeOffset utcNow)
        {
            if (new_bL_UserProfile?.Indexes == null)
            {
                return existingUserProfile.Indexes;
            }

            var indexes = new List<UserProfile.Index>();
            if (existingUserProfile?.Indexes != null)
            {
                indexes.AddRange(existingUserProfile.Indexes);
            }

            foreach (var i in new_bL_UserProfile.Indexes)
            {
                var existingIndex = indexes?.FirstOrDefault(e => e.TypeOfIndex == i.TypeOfIndex);
                if (existingIndex == null)
                {
                    indexes.Add(new UserProfile.Index { DTLastUpdated = utcNow, TypeOfIndex = i.TypeOfIndex, Value = i.Value });
                }
                else
                {
                    if (existingIndex.Value != i.Value)
                    {
                        existingIndex.DTLastUpdated = utcNow;
                        existingIndex.Value = i.Value;
                    }
                }
            }

            return indexes;
        }

        public async Task SetRelationshipAsync(BL_Relationship relationship, string userId)
        {
            var userProfile = await _repo.GetUserProfile_byIdAsync(userId);
            var existingRelationship = await _repo.GetRelationship_byIdAsync(userProfile.Relationship_Id);
            if (existingRelationship != null && existingRelationship.RegisteredPartner_Id != userId)
            {
                throw new ApplicationException($"Relationship with id {existingRelationship.Id} for user id {userId} exists but cannot be set since the registered partner is {existingRelationship.RegisteredPartner_Id}");
            }

            //Update
            relationship.Id = existingRelationship?.Id;
            relationship.RegisteredPartner_Id = userId;
            await _repo.SetRelationshipAsync(map.From(relationship));
        }

        public async Task<string> CreateQuestionnaireTemplateAsync(QuestionnaireTemplate newQuestionnaire)
        {
            string newId;
            if (validator.IsValid(newQuestionnaire))
            {
                AssignIdsForQuestionnaire(newQuestionnaire);
                newId = await _repo.CreateQuestionnaireTemplateAsync(newQuestionnaire);
                return newId;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void AssignIdsForQuestionnaire(QuestionnaireTemplate qTemplate)
        {
            foreach (var qSet in qTemplate.QuestionnaireSets)
            {
                qSet.Id = newId();

                foreach (var q in qSet.Questions)
                {
                    setIdsForQuestions(q);
                }
            }

            void setIdsForQuestions(Question q)
            {
                q.Id = newId();

                if (q.AnswerOptions != null)
                {
                    foreach (var a in q.AnswerOptions)
                    {
                        setIdsAnsweers(a);
                    }
                }
            }

            void setIdsAnsweers(AnswerOption a)
            {
                if (a.ChildQuestion != null)
                {
                    setIdsForQuestions(a.ChildQuestion);
                }
            }

            string newId() => Guid.NewGuid().ToString("D").ToUpper();
        }

        List<BL_UserProfile> MapFrom(List<UserProfile> userProfiles)
        {
            var bL_UserProfiles = new List<BL_UserProfile>();
            foreach (var userprofile in userProfiles)
            {
                bL_UserProfiles.Add(map.From(userprofile));
            }
            return bL_UserProfiles;
        }

        UserProfileRegistration MapFrom(BL_UserProfileRegistration usrProReg)
        {
            if (usrProReg == null)
            {
                return null;
            }

            return new UserProfileRegistration
            {
                PartnerDateOfBirth = usrProReg.PartnerDateOfBirth,
                PartnerEmail = usrProReg.PartnerEmail,
                PartnerFirstName = usrProReg.PartnerFirstName,
                PartnerGender = usrProReg.PartnerGender,
                PartnerLastName = usrProReg.PartnerLastName,
                PartnerMobilePhone = usrProReg.PartnerMobilePhone,

                UserProfile_Id = usrProReg.UserProfile_Id,
            };

        }

        public async Task<UserQuestionnaire> GetUserQuestionnaire_ByUserIdAsync(string userId, QuestionnaireTemplate.QuestionnaireType questionnaireType, string questionnaireTemplateId = null)
        {
            var userQuestionnaire = await _repo.GetUserQuestionnaire_ByUserIdAsync(userId, questionnaireType, questionnaireTemplateId);
            return userQuestionnaire;
        }

        public async Task<QuestionnaireTemplate> GetQuestionnaireTemplate_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType questionnaireType, string gender)
        {
            var genderType = QuestionnaireTemplate.GenderType.Any;

            switch (gender)
            {
                case "m":
                    genderType = QuestionnaireTemplate.GenderType.Male;
                    break;
                case "f":
                    genderType = QuestionnaireTemplate.GenderType.Female;
                    break;
                case null:
                    genderType = QuestionnaireTemplate.GenderType.Any;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"gender '${gender}' is not a valid value");
            }

            var questionnaireTemplates = await _repo.GetQuestionnaireTemplates_ActiveByTypeAndGender(questionnaireType, genderType);

            if (questionnaireTemplates.Count > 1)
            {
                throw new BusinessLogicException("Found more than 1 QuestionnaireTemplate meeting the criteria");
            }
            return questionnaireTemplates.FirstOrDefault();
        }

        public async Task<BL_Relationship> GetRelationshipAsync(string userId)
        {
            //TODO: C make use of cache, see 'GetUserProfileId_byExtRefIdAsync'
            var userProfile = await _repo.GetUserProfile_byIdAsync(userId);
            var relationship = await _repo.GetRelationship_byIdAsync(userProfile.Relationship_Id);

            var result = map.From(relationship);
            return result;
        }

        public async Task<BL_UserProfileRegistration> GetUserProfileRegistrationAsync(string userId)
        {
            var userProfileRegistration = await _repo.GetUserProfileRegistrationAsync(userId);
            var result = map.From(userProfileRegistration);
            return result;
        }

        public async Task SetUserProfileRegistrationAsync(BL_UserProfileRegistration bl_usrProReg)
        {
            var usrProReg = MapFrom(bl_usrProReg);
            //TODO: Ensure UserProfile Id exists
            var setUsrProReg = await _repo.SetUserProfileRegistrationAsync(usrProReg);
        }

        /// <summary>
        /// Returns the given string value with the first character in Upper Case. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetStringWithFirstCharacterUpper(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            else
            {
                return input.Substring(0, 1).ToUpper() + input.Substring(1, input.Length - 1);
            }
        }

        #region Cached Data
        public async Task<string> GetCachedPartnerUserId(string userId)
        {
            var r = _cacheService.GetPartnerUserId_byCurrentUserId(userId);

            if (r == null)
            {
                r = await _repo.GetUserProfileIdOfPartner_byUserId(userId);
                _cacheService.SetPartnerUserId_forCurrentUserId(userId, r);
            }

            return r;
        }

        public async Task<string> GetCachedUserId_byExternalReferenceIdAsync(string externalReferenceId)
        {
            var r = _cacheService.GetUserId_byExternalReferenceId(externalReferenceId);

            if (r == null)
            {
                var userprofile = await _repo.GetUserProfile_byExtRefIdAsync(externalReferenceId);
                r = userprofile.Id;
                _cacheService.SetUserProfileCache(userprofile);
            }

            return r;
        }

        public async Task<string> GetCachedRelationshipId_byUserIdAsync(string userId)
        {
            var r = _cacheService.GetRelationshipId_byUserId(userId);

            if (r == null)
            {
                var userprofile = await _repo.GetUserProfile_byIdAsync(userId);
                r = userprofile.Relationship_Id;
                _cacheService.SetUserProfileCache(userprofile);
            }

            return r;
        }

        public async Task<string> GetCachedFirstname_byUserIdAsync(string userId)
        {
            var r = _cacheService.GetFirstname_byUserId(userId);

            if (r == null)
            {
                var userprofile = await _repo.GetUserProfile_byIdAsync(userId);
                r = userprofile.FirstName;
                _cacheService.SetUserProfileCache(userprofile);
            }

            return r;
        }

        #endregion
    }
}

