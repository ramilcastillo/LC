using LifeCouple.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.DAL
{
    public class Repository //: IRepository
    {
        private readonly Context _ctx;
        private readonly ILogger<Repository> _logger;

        public Repository(Context ctx, ILogger<Repository> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public async Task<UserProfile> CreateUserAsync(string primaryEmail, string externalRefId, bool isDevTest)
        {
            var user = new UserProfile()
            {
                PrimaryEmail = primaryEmail,
                ExternalRefId = externalRefId,
                IsDevTest = isDevTest,
                Id = _ctx.GetNewId(typeof(UserProfile))
            };

            var result = await _ctx.UserProfiles.AddAsync(user);
            var saveResult = await _ctx.SaveChangesAsync();
            return user;
        }
        public async Task<string> SetUserProfileAsync(UserProfile usrProfile)
        {
            throw new NotImplementedException("method in this class is not implemented, use Cosmos Db instead");

            var existingItem = await _ctx.UserProfiles.SingleOrDefaultAsync(e => e.Id == usrProfile.Id);

            if (existingItem == null)
            {
                throw new ApplicationException($"Userprofile with id:{usrProfile.Id} not found.");
            }
            else
            {
                existingItem.DateOfBirth = usrProfile.DateOfBirth ?? existingItem.DateOfBirth;
                existingItem.FirstName = usrProfile.FirstName ?? existingItem.FirstName;
                existingItem.Gender = usrProfile.Gender ?? existingItem.Gender;
                existingItem.LastName = usrProfile.LastName ?? existingItem.LastName;
                existingItem.NotificationOption = usrProfile.NotificationOption ?? existingItem.NotificationOption;
                existingItem.PrimaryEmail = usrProfile.PrimaryEmail ?? existingItem.PrimaryEmail;
                existingItem.PrimaryMobileNr = usrProfile.PrimaryMobileNr ?? existingItem.PrimaryMobileNr;
            }
            var saveResult = await _ctx.SaveChangesAsync();
        }

        public async Task SetRelationshipAsync(Relationship relationship)
        {
            var existingItem = await _ctx.Relationships.SingleOrDefaultAsync(e => e.Id == relationship.Id);

            if (existingItem == null)
            {
                relationship.Id = _ctx.GetNewId(typeof(Relationship));
                var result = await _ctx.Relationships.AddAsync(relationship);
            }
            else
            {
                //item found, let's update it
                existingItem.BeenToCounseler = (relationship.BeenToCounseler == null || relationship.BeenToCounseler == CounselerStatusEnum.Unknown) ? existingItem.BeenToCounseler : relationship.BeenToCounseler;
                existingItem.CurrentWeddingDate = relationship.CurrentWeddingDate ?? existingItem.CurrentWeddingDate;
                existingItem.MarriageStatus = (relationship.MarriageStatus == null || relationship.MarriageStatus == MarriageStatusEnum.Unknown) ? existingItem.MarriageStatus : relationship.MarriageStatus;
                existingItem.NrOfChildren = relationship.NrOfChildren ?? existingItem.NrOfChildren;
                existingItem.NrOfStepChildren = relationship.NrOfStepChildren ?? existingItem.NrOfStepChildren;
                existingItem.RelationshipStatus = (relationship.RelationshipStatus == null || relationship.RelationshipStatus == RelationshipStatusEnum.Unknown) ? existingItem.RelationshipStatus : relationship.RelationshipStatus;
            }

            var userprofile = await _ctx.UserProfiles.SingleOrDefaultAsync(e => e.Id == relationship.RegisteredPartner_Id);
            userprofile.Relationship_Id = relationship.Id;


            var saveResult = await _ctx.SaveChangesAsync();

        }

        public async Task<UserProfileRegistration> SetUserProfileRegistrationAsync(UserProfileRegistration usrProReg)
        {
            var existingItem = await _ctx.UserProfileRegistrations.SingleOrDefaultAsync(e => e.UserProfile_Id == usrProReg.UserProfile_Id);

            if (existingItem == null)
            {
                var result = await _ctx.UserProfileRegistrations.AddAsync(usrProReg);
            }
            else
            {
                existingItem.PartnerDateOfBirth = usrProReg.PartnerDateOfBirth ?? existingItem.PartnerDateOfBirth;
                existingItem.PartnerEmail = usrProReg.PartnerEmail ?? existingItem.PartnerEmail;
                existingItem.PartnerFirstName = usrProReg.PartnerFirstName ?? existingItem.PartnerFirstName;
                existingItem.PartnerGender = usrProReg.PartnerGender ?? existingItem.PartnerGender;
                existingItem.PartnerLastName = usrProReg.PartnerLastName ?? existingItem.PartnerLastName;
                existingItem.PartnerMobilePhone = usrProReg.PartnerMobilePhone ?? existingItem.PartnerMobilePhone;
            }
            var saveResult = await _ctx.SaveChangesAsync();

            //TODO: change return singature to void...
            return new UserProfileRegistration();

        }

        public async Task<List<UserProfile>> FindUserProfiles_byPrimaryEmailAsync(string primaryEmail)
        {
            var userprofiles = _ctx.UserProfiles.Where(e => e.PrimaryEmail == primaryEmail);
            var userProfileList = await userprofiles.ToListAsync();
            return userProfileList;
        }

        public Task<UserProfileRegistration> GetUserProfileRegistrationAsync(string userId)
        {
            var userProfileRegistration = _ctx.UserProfileRegistrations.SingleOrDefaultAsync(e => e.UserProfile_Id == userId);
            return userProfileRegistration;
        }

        public Task<Relationship> GetRelationship_byIdAsync(string relationshipId)
        {
            return _ctx.Relationships.SingleOrDefaultAsync(e => e.Id == relationshipId);
        }

        public Task<UserProfile> GetUserProfile_byIdAsync(string userId)
        {
            return _ctx.UserProfiles.SingleOrDefaultAsync(e => e.Id == userId);
        }

        public Task<UserProfile> GetUserProfile_byExtRefIdAsync(string externalRefId)
        {
            return _ctx.UserProfiles.SingleOrDefaultAsync(e => e.ExternalRefId == externalRefId);
        }


        public bool IsDbAccessible()
        {
            var r = _ctx.UserProfiles.Where(e => e.Id == "").ToArray();

            return (r.Length == 0);
        }

        public Task<string> CreateQuestionnaireTemplateAsync(QuestionnaireTemplate newQuestionnaire)
        {
            throw new NotImplementedException();
        }

        public Task<List<QuestionnaireTemplate>> GetQuestionnaireTemplates_ActiveByTypeAndGender(QuestionnaireTemplate.QuestionnaireType questionnaireType, QuestionnaireTemplate.GenderType genderType)
        {
            throw new NotImplementedException();
        }

        public Task<UserQuestionnaire> GetUserQuestionnaire_ByUserIdAsync(string userId, QuestionnaireTemplate.QuestionnaireType questionnaireType, string questionnaireTemplateId = null)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionnaireTemplate> GetQuestionnaireTemplate_ByIdAsync(string questionnaireTemplateId)
        {
            throw new NotImplementedException();
        }

        public Task<string> SetUserQuestionnaireAsync(UserQuestionnaire entity)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateUserIndexHistory(UserIndexHistory userIndexHistory)
        {
            throw new NotImplementedException();
        }
    }
}
