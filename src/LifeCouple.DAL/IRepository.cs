using System.Collections.Generic;
using System.Threading.Tasks;
using LifeCouple.DAL.Entities;

namespace LifeCouple.DAL
{
    public interface IRepository
    {
        bool IsDbAccessible();

        Task<UserProfile> CreateUserAsync(string primaryEmail, string externalRefId, bool isDevTest, string firstName, string lastName, string relationshipId, bool isUserInvitee = false);
        Task<UserProfile> GetUserProfile_byExtRefIdAsync(string externalRefId);
        Task<UserProfile> GetUserProfile_byIdAsync(string userId);
        Task<UserProfile> GetUserProfile_byInvitationId(string invitationId);
        Task<string> GetUserProfileIdOfPartner_byUserId(string userId);
        Task<List<UserProfile>> FindUserProfiles_byRelationshipId(string relationshipId);
        Task<List<UserProfile>> FindUserProfiles_byPrimaryEmailAsync(string primaryEmail);

        Task<string> SetUserProfileAsync(UserProfile usrProfile);
        Task SetRelationshipAsync(Relationship relationship);
        Task<Relationship> GetRelationship_byIdAsync(string relationshipId);


        Task<UserProfileRegistration> GetUserProfileRegistrationAsync(string userId);
        Task<UserProfileRegistration> SetUserProfileRegistrationAsync(UserProfileRegistration usrProReg);

        Task<UserQuestionnaire> GetUserQuestionnaire_ByUserIdAsync(string userId, QuestionnaireTemplate.QuestionnaireType questionnaireType, string questionnaireTemplateId = null);
        
        /// <summary>
        /// Returns the Id of the entity created/updated
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<string> SetUserQuestionnaireAsync(UserQuestionnaire entity);

        Task<string> CreateQuestionnaireTemplateAsync(QuestionnaireTemplate newQuestionnaire);
        Task<List<QuestionnaireTemplate>> GetQuestionnaireTemplates_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType questionnaireType, QuestionnaireTemplate.GenderType genderType);
        Task<QuestionnaireTemplate>  GetQuestionnaireTemplate_ByIdAsync(string questionnaireTemplateId);

        /// <summary>
        /// Creates a new UserIndexHistory, assumes that all data is correct and validated prior to calling this method.
        /// </summary>
        /// <param name="userIndexHistory"></param>
        /// <returns></returns>
        Task<string> CreateUserIndexHistory(UserIndexHistory userIndexHistory);

        Task<string> SetBusinessLogicSettingsAsync(BusinessLogicSettings businessLogicSettings);
        Task<BusinessLogicSettings> GetBusinessLogicSettingsAsync();

        //Emotional Bank Account
        Task<string> SetEbaPointsSentAsync(EbaPointsSent entity);
        Task<EbaPointsSent> GetEbaPointsSent_ByUserIdAsync(string userId);
        Task<List<EbaPointsSent>> FindEbaPointsSent_ByRelationshipIdAsync(string relationshipId);

        //AppCenter Device info
        Task<List<AppCenterDeviceDetail>> FindAppCenterDeviceDetail_ByUserIdAsync(string userId);
        Task<AppCenterDeviceDetail> GetAppCenterDeviceDetails_DeviceId(string deviceId);
        Task<string> SetAppCenterDeviceDetailsAsync(AppCenterDeviceDetail entity);

        //Activities
        Task<string> SetActivityAsync(Activity entity);
        Task<Activity> GetActivitiy_ByUserIdAsync(string userId, string activityType, string activityStatusType);

    }
}
