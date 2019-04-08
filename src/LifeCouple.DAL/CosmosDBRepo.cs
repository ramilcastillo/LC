using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LifeCouple.DAL.Entities;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LifeCouple.DAL
{
    public partial class CosmosDBRepo : IRepository
    {
        private readonly ILogger<CosmosDBRepo> _logger;
        private static CosmosDbSettingsModel _settings;

        private static DocumentClient client;
        private static bool isSuccessFullyInitialized;

        private readonly static object lockObject = new object();

        public CosmosDBRepo(ILogger<CosmosDBRepo> logger, IOptions<CosmosDbSettingsModel> settings)
        {
            _logger = logger;
            InitializeClient(settings.Value);
        }


        public async Task<UserProfile> CreateUserAsync(string primaryEmail, string externalRefId, bool isDevTest, string firstName, string lastName, string relationshipId, bool isUserInvitee = false)
        {
            var utcNow = DateTimeOffset.UtcNow;
            var user = new UserProfile()
            {
                PrimaryEmail = primaryEmail,
                ExternalRefId = externalRefId,
                IsDevTest = isDevTest,
                FirstName = firstName,
                LastName = lastName,
                Relationship_Id = relationshipId,
                DTCreated = utcNow,
                DTLastUpdated = utcNow,
                Id = null, //Set by CosmosDb
            };

            if (isUserInvitee)
            {
                user.PartnerInvitation = new PartnerInvitation { InvitationStatus = PartnerInvitationStatusTypeEnum.NotApplicable };
            }

            var up = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId), SetEntityType(user));
            return (UserProfile)(dynamic)up.Resource;
        }

        public async Task<List<UserProfile>> FindUserProfiles_byPrimaryEmailAsync(string primaryEmail)
        {
            var results = new List<UserProfile>();

            try
            {
                var q = client.CreateDocumentQuery<UserProfile>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                    new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                    .Where(e => e.PrimaryEmail == primaryEmail && e.Type == "UserProfile")
                    .AsDocumentQuery();

                while (q.HasMoreResults)
                {
                    results.AddRange(await q.ExecuteNextAsync<UserProfile>());
                }

                return results;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return results;
                }
                else
                {
                    throw;
                }
            }

        }


        public async Task<UserProfileRegistration> GetUserProfileRegistrationAsync(string userId)
        {
            try
            {
                var q = client.CreateDocumentQuery<UserProfileRegistration>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                    new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                    .Where(e => e.UserProfile_Id == userId && e.Type == "UserProfileRegistration")
                    .AsDocumentQuery();

                var results = new List<UserProfileRegistration>();
                while (q.HasMoreResults)
                {
                    results.AddRange(await q.ExecuteNextAsync<UserProfileRegistration>());
                }

                return results.FirstOrDefault();
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<UserProfile> GetUserProfile_byExtRefIdAsync(string externalRefId)
        {
            try
            {
                var query = client.CreateDocumentQuery<UserProfile>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(e => e.ExternalRefId == externalRefId && e.Type == "UserProfile")
                .AsDocumentQuery();

                var UserProfiles = await query.ExecuteNextAsync<UserProfile>();

                return UserProfiles.FirstOrDefault();
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<UserProfile> GetUserProfile_byIdAsync(string userId)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_settings.DatabaseId, _settings.DefaultCollectionId, userId));
                return (UserProfile)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<UserProfile> GetUserProfile_byInvitationId(string invitationId)
        {
            try
            {
                //Get byt id: //Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                var query = client.CreateDocumentQuery<UserProfile>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(e => e.PartnerInvitation.InvitationId == invitationId && e.Type == "UserProfile")
                .AsDocumentQuery();

                var UserProfiles = await query.ExecuteNextAsync<UserProfile>();

                return UserProfiles.FirstOrDefault();
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<string> GetUserProfileIdOfPartner_byUserId(string userId)
        {
            //TODO: Ramil: Change this to 1 SQL/query to the db and support when relationship id is null
            var userprofile = await GetUserProfile_byIdAsync(userId);

            if (userprofile?.Relationship_Id != null)
            {
                var userprofiles = await FindUserProfiles_byRelationshipId(userprofile.Relationship_Id);
                return userprofiles.SingleOrDefault(e => e.Id != userId)?.Id;
            }

            return null;
        }


        public async Task<List<UserProfile>> FindUserProfiles_byRelationshipId(string relationshipId)
        {
            var results = new List<UserProfile>();

            try
            {
                var q = client.CreateDocumentQuery<UserProfile>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                    new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                    .Where(e => e.Relationship_Id == relationshipId && e.Type == "UserProfile")
                    .AsDocumentQuery();

                while (q.HasMoreResults)
                {
                    results.AddRange(await q.ExecuteNextAsync<UserProfile>());
                }

                return results;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return results;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Relationship> GetRelationship_byIdAsync(string relationshipId)
        {
            if (string.IsNullOrEmpty(relationshipId))
            {
                return null;
            }
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_settings.DatabaseId, _settings.DefaultCollectionId, relationshipId));
                return (Relationship)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

        }


        public async Task<string> SetUserProfileAsync(UserProfile usrProfile)
        {
            //assuming all data is Validated prior to calling this method
            string idUpdatedOrCreated;

            SetEntityType(usrProfile);

            if (string.IsNullOrEmpty(usrProfile.Id))
            {
                //New item
                var up = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId), usrProfile);
                var newlyCreatedUserQuestionnaire = (UserProfile)(dynamic)up.Resource;
                idUpdatedOrCreated = newlyCreatedUserQuestionnaire.Id;
            }
            else
            {
                //existing item
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_settings.DatabaseId, _settings.DefaultCollectionId, usrProfile.Id)); // will throw an exception if item not exists
                var existingItem = (UserProfile)(dynamic)document;
                document.LoadFromExisingEntity(usrProfile);
                //TODO: the dtLastUpdated timestamp ends with with offset hours in the Cosmos db, that should be changed to use the specific json serializer, see the LoadFromExisingEntity() method as a reference
                Document updated = await client.ReplaceDocumentAsync(document);
                idUpdatedOrCreated = existingItem.Id;
            }
            return idUpdatedOrCreated;

        }

        public async Task<List<EbaPointsSent>> FindEbaPointsSent_ByRelationshipIdAsync(string relationshipId)
        {
            var results = new List<EbaPointsSent>();

            try
            {
                var q = client.CreateDocumentQuery<EbaPointsSent>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(e => e.RelationshipId == relationshipId && e.Type == GetCosmosDbEntityType<EbaPointsSent>())
                .AsDocumentQuery();

                while (q.HasMoreResults)
                {
                    results.AddRange(await q.ExecuteNextAsync<EbaPointsSent>());
                }

                return results;

            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<EbaPointsSent> GetEbaPointsSent_ByUserIdAsync(string userId)
        {
            try
            {
                var query = client.CreateDocumentQuery<EbaPointsSent>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(e => e.UserprofileId == userId && e.Type == GetCosmosDbEntityType<EbaPointsSent>())
                .AsDocumentQuery();

                var results = await query.ExecuteNextAsync<EbaPointsSent>();

                return results.SingleOrDefault();
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

        }

        /// <summary>
        /// Assumes all data is Validated and Complete prior to calling this method, since it will replace the complete document if already exists, else create a new one. 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<string> SetEbaPointsSentAsync(EbaPointsSent entity)
        {
            SetEntityType(entity);

            //Below logic was introduced 180908 to replace the read and the replace operation we used to follow prior
            var upsert = await client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId), entity);
            return upsert.Resource.Id;

        }

        public async Task<string> CreateUserIndexHistory(UserIndexHistory userIndexHistory)
        {
            //assuming all data is Validated prior to calling this method
            SetEntityType(userIndexHistory);

            //New item
            var up = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId), userIndexHistory);
            var createdUserIndeHistory = (UserIndexHistory)(dynamic)up.Resource;
            return createdUserIndeHistory.Id;
        }

        public async Task<UserProfileRegistration> SetUserProfileRegistrationAsync(UserProfileRegistration usrProReg)
        {
            throw new NotImplementedException();
        }

        // See https://blogs.msdn.microsoft.com/webdev/2018/04/27/cosmos-db-solves-common-data-challenges/

        //public async Task<UserProfileRegistration> SetUserProfileRegistrationAsync(UserProfileRegistration usrProReg)
        //{
        //    var existingItem = await _ctx.UserProfileRegistrations.SingleOrDefaultAsync(e => e.UserProfile_Id == usrProReg.UserProfile_Id);

        //    if (existingItem == null)
        //    {
        //        var result = await _ctx.UserProfileRegistrations.AddAsync(usrProReg);
        //    }
        //    else
        //    {
        //        existingItem.DateOfBirth = usrProReg.DateOfBirth ?? existingItem.DateOfBirth;
        //        existingItem.Email = usrProReg.Email ?? existingItem.Email;
        //        existingItem.FirstName = usrProReg.FirstName ?? existingItem.FirstName;
        //        existingItem.Gender = usrProReg.Gender ?? existingItem.Gender;
        //        existingItem.HasMoreThanOneMarriage = usrProReg.HasMoreThanOneMarriage ?? existingItem.HasMoreThanOneMarriage;
        //        existingItem.HasAgreedToPrivacyPolicy = usrProReg.HasAgreedToPrivacyPolicy ?? existingItem.HasAgreedToPrivacyPolicy;
        //        existingItem.HasAgreedToRefundPolicy = usrProReg.HasAgreedToRefundPolicy ?? existingItem.HasAgreedToRefundPolicy;
        //        existingItem.HasAgreedToTandC = usrProReg.HasAgreedToTandC ?? existingItem.HasAgreedToTandC;
        //        existingItem.IsMarried = usrProReg.IsMarried ?? existingItem.IsMarried;
        //        existingItem.LastName = usrProReg.LastName ?? existingItem.LastName;
        //        existingItem.LastWeddingDate = usrProReg.LastWeddingDate ?? existingItem.LastWeddingDate;
        //        existingItem.MobilePhone = usrProReg.MobilePhone ?? existingItem.MobilePhone;
        //        existingItem.NotificationOption = usrProReg.NotificationOption ?? existingItem.NotificationOption;
        //        existingItem.NrOfChildren = usrProReg.NrOfChildren ?? existingItem.NrOfChildren;
        //        existingItem.NrOfStepChildren = usrProReg.NrOfStepChildren ?? existingItem.NrOfStepChildren;
        //        existingItem.PartnerDateOfBirth = usrProReg.PartnerDateOfBirth ?? existingItem.PartnerDateOfBirth;
        //        existingItem.PartnerEmail = usrProReg.PartnerEmail ?? existingItem.PartnerEmail;
        //        existingItem.PartnerFirstName = usrProReg.PartnerFirstName ?? existingItem.PartnerFirstName;
        //        existingItem.PartnerGender = usrProReg.PartnerGender ?? existingItem.PartnerGender;
        //        existingItem.PartnerLastName = usrProReg.PartnerLastName ?? existingItem.PartnerLastName;
        //        existingItem.PartnerMobilePhone = usrProReg.PartnerMobilePhone ?? existingItem.PartnerMobilePhone;
        //    }
        //    var saveResult = await _ctx.SaveChangesAsync();

        //    //TODO: change return singature to void...
        //    return new UserProfileRegistration();

        //}


        public async Task SetRelationshipAsync(Relationship relationship)
        {
            //TODO: BBB move this logic to the BL
            try
            {
                if (string.IsNullOrEmpty(relationship.Id))
                {
                    await createNewRelationshipAsync();
                }
                else
                {
                    Relationship existingItem;
                    Document document;
                    document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_settings.DatabaseId, _settings.DefaultCollectionId, relationship.Id)); // will throw an exception if item not exists

                    //item found, let's update it
                    existingItem = (Relationship)(dynamic)document;
                    existingItem.BeenToCounseler = (relationship.BeenToCounseler == null || relationship.BeenToCounseler == CounselerStatusEnum.Unknown) ? existingItem.BeenToCounseler : relationship.BeenToCounseler;
                    existingItem.CurrentWeddingDate = relationship.CurrentWeddingDate ?? existingItem.CurrentWeddingDate;
                    existingItem.MarriageStatus = (relationship.MarriageStatus == null || relationship.MarriageStatus == MarriageStatusEnum.Unknown) ? existingItem.MarriageStatus : relationship.MarriageStatus;
                    existingItem.NrOfChildren = relationship.NrOfChildren ?? existingItem.NrOfChildren;
                    existingItem.NrOfStepChildren = relationship.NrOfStepChildren ?? existingItem.NrOfStepChildren;
                    existingItem.RelationshipStatus = (relationship.RelationshipStatus == null || relationship.RelationshipStatus == RelationshipStatusEnum.Unknown) ? existingItem.RelationshipStatus : relationship.RelationshipStatus;

                    document.LoadFromExisingEntity(existingItem);

                    Document updated = await client.ReplaceDocumentAsync(document);
                }
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    //New item, let's create it
                    await createNewRelationshipAsync();
                }
                else
                {
                    throw;
                }
            }

            async Task createNewRelationshipAsync()
            {
                var up = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId), SetEntityType(relationship));
                var newlyCreatedRelationhip = (Relationship)(dynamic)up.Resource;
                var x = await this.GetUserProfile_byIdAsync(relationship.RegisteredPartner_Id);
                x.Relationship_Id = newlyCreatedRelationhip.Id;
                x.DTLastUpdated = DateTimeOffset.UtcNow;
                await this.SetUserProfileAsync(x);
            }

        }

        #region AppCenter Device Details


        public async Task<List<AppCenterDeviceDetail>> FindAppCenterDeviceDetail_ByUserIdAsync(string userId)
        {
            var results = new List<AppCenterDeviceDetail>();

            try
            {
                var q = client.CreateDocumentQuery<AppCenterDeviceDetail>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(e => e.UserprofileId == userId && e.Type == GetCosmosDbEntityType<AppCenterDeviceDetail>())
                .AsDocumentQuery();

                while (q.HasMoreResults)
                {
                    results.AddRange(await q.ExecuteNextAsync<AppCenterDeviceDetail>());
                }

                return results;

            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }



        public async Task<AppCenterDeviceDetail> GetAppCenterDeviceDetails_DeviceId(string deviceId)
        {
            try
            {
                var query = client.CreateDocumentQuery<AppCenterDeviceDetail>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(e => e.DeviceId == deviceId && e.Type == GetCosmosDbEntityType<AppCenterDeviceDetail>())
                .AsDocumentQuery();

                var results = await query.ExecuteNextAsync<AppCenterDeviceDetail>();

                return results.SingleOrDefault();
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<string> SetAppCenterDeviceDetailsAsync(AppCenterDeviceDetail entity)
        {
            SetEntityType(entity);

            //TODO: Consider setting TTL so these records are automatically deleted after a few weeks, so we don' send notifications to devices that are not in use any longer... - see https://docs.microsoft.com/en-us/azure/cosmos-db/time-to-live
            var upsert = await client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId), entity);
            return upsert.Resource.Id;
        }

        #endregion

        #region Questionnaires
        public async Task<string> CreateQuestionnaireTemplateAsync(QuestionnaireTemplate questionnaireTemplate)
        {
            questionnaireTemplate.DTCreated = DateTimeOffset.UtcNow;
            questionnaireTemplate.DTLastUpdated = questionnaireTemplate.DTCreated;
            questionnaireTemplate.Id = null; //Set by CosmosDb

            var up = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId), SetEntityType(questionnaireTemplate));
            var newItem = (QuestionnaireTemplate)(dynamic)up.Resource;
            return newItem.Id;
        }

        public async Task<QuestionnaireTemplate> GetQuestionnaireTemplate_ByIdAsync(string questionnaireTemplateId)
        {
            try
            {
                var result = await client.ReadDocumentAsync<QuestionnaireTemplate>(UriFactory.CreateDocumentUri(_settings.DatabaseId, _settings.DefaultCollectionId, questionnaireTemplateId));
                return result.Document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<List<QuestionnaireTemplate>> GetQuestionnaireTemplates_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType questionnaireType, QuestionnaireTemplate.GenderType genderType)
        {
            try
            {
                var query = client.CreateDocumentQuery<QuestionnaireTemplate>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                new FeedOptions { MaxItemCount = 2 })
                .Where(e =>
                e.IsActive
                && e.TypeOfQuestionnaire == questionnaireType
                && (e.TypeOfGender == QuestionnaireTemplate.GenderType.Any || e.TypeOfGender == genderType || genderType == QuestionnaireTemplate.GenderType.Any)
                && e.Type == "QuestionnaireTemplate"
                )
                .AsDocumentQuery();

                var qTemplates = await query.ExecuteNextAsync<QuestionnaireTemplate>();

                return qTemplates.ToList();
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<UserQuestionnaire> GetUserQuestionnaire_ByUserIdAsync(string userId, QuestionnaireTemplate.QuestionnaireType questionnaireType, string questionnaireTemplateId = null)
        {
            try
            {
                var query = client.CreateDocumentQuery<UserQuestionnaire>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                new FeedOptions { MaxItemCount = 2 })
                .Where(e => e.Type == "UserQuestionnaire"
                && e.UserprofileId == userId
                && e.QuestionnaireTemplateType == questionnaireType
                && (string.IsNullOrWhiteSpace(questionnaireTemplateId) || e.QuestionnaireTemplateId == questionnaireTemplateId)
                ).AsDocumentQuery();

                var qTemplates = await query.ExecuteNextAsync<UserQuestionnaire>();

                var results = qTemplates.ToList();
                if (results.Count > 1)
                {
                    throw new ApplicationException("found more than 1 item");
                }
                return qTemplates.ToList().FirstOrDefault();
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<string> SetUserQuestionnaireAsync(UserQuestionnaire entity)
        {
            //assuming all data is Validated prior to calling this method
            string idUpdatedOrCreated;

            SetEntityType(entity);

            if (string.IsNullOrEmpty(entity.Id))
            {
                //New item
                var up = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId), entity);
                var newlyCreatedUserQuestionnaire = (UserQuestionnaire)(dynamic)up.Resource;
                idUpdatedOrCreated = newlyCreatedUserQuestionnaire.Id;
            }
            else
            {
                //existing item
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_settings.DatabaseId, _settings.DefaultCollectionId, entity.Id)); // will throw an exception if item not exists
                var existingItem = (UserQuestionnaire)(dynamic)document;
                document.LoadFromExisingEntity(entity);
                Document updated = await client.ReplaceDocumentAsync(document);
                idUpdatedOrCreated = existingItem.Id;
            }
            return idUpdatedOrCreated;
        }

        #endregion


        #region Business Logic Settings

        public async Task<string> SetBusinessLogicSettingsAsync(BusinessLogicSettings businessLogicSettings)
        {
            //assuming all data is Validated prior to calling this method
            string idUpdatedOrCreated;

            SetEntityType(businessLogicSettings);

            if (string.IsNullOrEmpty(businessLogicSettings.Id))
            {
                //New item
                var up = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId), businessLogicSettings);
                var newlyCreatedEntity = (BusinessLogicSettings)(dynamic)up.Resource;
                idUpdatedOrCreated = newlyCreatedEntity.Id;
            }
            else
            {
                //existing item
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_settings.DatabaseId, _settings.DefaultCollectionId, businessLogicSettings.Id)); // will throw an exception if item not exists
                var existingItem = (UserProfile)(dynamic)document;
                document.LoadFromExisingEntity(businessLogicSettings);
                Document updated = await client.ReplaceDocumentAsync(document);
                idUpdatedOrCreated = existingItem.Id;
            }
            return idUpdatedOrCreated;
        }

        public async Task<BusinessLogicSettings> GetBusinessLogicSettingsAsync()
        {
            try
            {
                //Only query on the Type since there should be 1 and only 1 instance in the DB
                var query = client.CreateDocumentQuery<BusinessLogicSettings>(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(e => e.Type == nameof(BusinessLogicSettings))
                .AsDocumentQuery();

                var entity = await query.ExecuteNextAsync<BusinessLogicSettings>();

                return entity.SingleOrDefault();
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion


        #region non functional
        /// <summary>
        /// Prepare entity to be be Created in Cosmos by Set the 'Type'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        private void InitializeClient(CosmosDbSettingsModel settingsModel)
        {
            if (!isSuccessFullyInitialized)
            {
                lock (lockObject)
                {
                    if (client == null)
                    {
                        try
                        {
                            _settings = settingsModel;
                            //Setting default serialize for all CosmosDb operations, required to ensure UTC format is preserved for datetimeOffset properties. Prior to adding this it was an issue in that it was converted to local witm with an offest of 7hrs
                            var jsonSettings = new JsonSerializerSettings { DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc, DateParseHandling = DateParseHandling.DateTimeOffset };

                            //based on https://docs.microsoft.com/en-us/azure/cosmos-db/performance-tips
                            client = new DocumentClient(new Uri(_settings.Endpoint), _settings.AccessKey, jsonSettings,
                                new ConnectionPolicy
                                {
                                    ConnectionMode = ConnectionMode.Direct,
                                    ConnectionProtocol = Protocol.Tcp
                                });

                            var sw = Stopwatch.StartNew();

                            CreateDatabaseIfNotExists();
                            System.Diagnostics.Debug.WriteLine("Db init: CreateDatabaseIfNotExists(): {0} - Completed", sw.ElapsedMilliseconds);

                            if (_settings.StartupMode == "CleanTestAbc")
                            {
                                System.Diagnostics.Debug.WriteLine("Db init: 'SetupFor_CleanTest_Mode()': {0} - Starting", sw.ElapsedMilliseconds);
                                SetupFor_CleanTest_Mode();
                                System.Diagnostics.Debug.WriteLine("Db init: 'SetupFor_CleanTest_Mode()': {0} - Completed", sw.ElapsedMilliseconds);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Db init: 'CreateCollectionIfNotExistsAsync()': {0} - Starting", sw.ElapsedMilliseconds);
                                CreateCollectionIfNotExists();
                                System.Diagnostics.Debug.WriteLine("Db init: 'CreateCollectionIfNotExistsAsync()': {0} - Completed", sw.ElapsedMilliseconds);
                            }
                            isSuccessFullyInitialized = true;
                        }
                        catch (Exception ex)
                        {
                            //TODO: Log
                            if (ex?.InnerException is HttpRequestException)
                            {
                                var message = $"Endpoint:{_settings.Endpoint.Substring(0, _settings.Endpoint.Length > 20 ? 20 : 10)}... DatabaseId:{_settings.DatabaseId.Substring(0, _settings.DatabaseId.Length > 10 ? 10 : 5)}...";
                                throw new ApplicationException($"Failed to connect to CosmosDb '{message}': {ex?.InnerException?.InnerException?.Message}", ex);
                            }
                            throw;
                        }
                    }
                }
            }
        }

        private void CreateDatabaseIfNotExists()
        {
            try
            {
                var task = Task.Run<ResourceResponse<Database>>(async () => await client.CreateDatabaseIfNotExistsAsync(new Database { Id = _settings.DatabaseId })); //var db = client.CreateDatabaseIfNotExistsAsync(new Database { Id = _settings.DatabaseId }).Result;
                var db = task.Result;

                if (db.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    //TODO: Log
                }

                if (db.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //TODO: log
                }
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable && client.ConnectionPolicy.ConnectionMode == ConnectionMode.Direct)
                {
                    //TODO: Log: downgrading ConnectionMode from Direct to Gateway - make this work
                    client.ConnectionPolicy.ConnectionMode = ConnectionMode.Gateway;
                    CreateDatabaseIfNotExists();
                }
                else
                {
                    throw;
                }
            }
        }

        private void CreateCollectionIfNotExists()
        {
            try
            {
                var task = Task.Run(async () => await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_settings.DatabaseId),
                        new DocumentCollection { Id = _settings.DefaultCollectionId },
                        new RequestOptions { OfferThroughput = 400 }));
                var x = task.Result;
            }
            catch (Exception e)
            {
                //TODO: Log error
                throw e;
            }
        }

        public bool IsDbAccessible()
        {
            try
            {
                var task = Task.Run(async () => await client.GetDatabaseAccountAsync()); //Using this method since it doesn't consume any request units
            }
            catch (DocumentClientException e)
            {
                //TODO: Logging
                return false;
            }

            return true;
        }

        private T SetEntityType<T>(T entity) where T : CosmosDbEntity
        {
            entity.Type = GetCosmosDbEntityType<T>();
            return entity;
        }

        public string GetCosmosDbEntityType<T>() where T : CosmosDbEntity
        {
            return typeof(T).Name;
        }

        private void SetupFor_CleanTest_Mode()
        {
            try
            {
                _settings.DefaultCollectionId = "Test-" + _settings.StartupMode;

                CreateCollectionIfNotExists();

                //Based on https://talkingaboutdata.wordpress.com/2015/08/24/deleting-multiple-documents-from-azure-documentdb/
                var crossPartition = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = 50 };
                var documentsList = client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, _settings.DefaultCollectionId), crossPartition).AsDocumentQuery();
                while (documentsList.HasMoreResults)
                {
                    var task = Task.Run(async () => await documentsList.ExecuteNextAsync());
                    var results = task.Result;
                    foreach (Document document in results)
                    {
                        System.Diagnostics.Debug.WriteLine($"deleting Document {document.Id}");
                        var tsk = Task.Run(async () => await client.DeleteDocumentAsync(document.SelfLink));
                        var y = tsk.Result;
                    }
                }
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    throw;
                }
            }
        }

        #endregion
    }
}
