using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeCouple.WebApi.DomainLogic;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;
using Xunit;

namespace LifeCouple.WebApi.BusinessLogicTests
{
    public class PartnerInvitationTests : BusinessLogicTestBase
    {
        [Fact]
        public async Task CreateNewInvitation_Ok()
        {
            //Create unique user for this test
            var extRefid = $"{GetType().Name}.{GetCallerMemberName()}";
            var testUser = await BL.CreateUserAsync($"{extRefid}@lifecouple.net", extRefid, true, "fname", "lname", null);

            //Update the userprofiles so we can check that it is not overwritten later...
            testUser.FirstName = "per";
            testUser.HasAgreedToTandC = false;
            var index = new DomainLogic.BL_UserProfile.Index { TypeOfIndex = DAL.Entities.IndexTypeEnum.Overall, Value = 50 };
            testUser.Indexes = new List<DomainLogic.BL_UserProfile.Index> { index };
            await BL.SetUserAsync(testUser);


            //Initialize new entity to be saved
            var dob = DateTime.Now.AddYears(-21);
            var newPartnerInvitation = new BL_PartnerInvitation
            {
                DateOfBirth = new DateTime(dob.Year, dob.Month, dob.Day, 0, 0, 0, DateTimeKind.Unspecified),
                FirstName = "fName",
                InvitedByUserId = testUser.Id,
                LastName = "lastName",
                MobilePhone = "7605006125",
                TypeOfGender = BL_GenderTypeEnum.Female,
            };

            var newId = await BL.SetPartnerInvitationsAsync(newPartnerInvitation);

            //Read new entity back
            var updatedTestUser = await BL.GetUserProfile_byExtRefIdAsync(testUser.ExternalRefId);
            Assert.NotNull(updatedTestUser);
            Assert.Equal(testUser.FirstName, updatedTestUser.FirstName);
            Assert.Equal(testUser.HasAgreedToTandC, updatedTestUser.HasAgreedToTandC);
            Assert.Equal(index.TypeOfIndex, updatedTestUser.Indexes[0].TypeOfIndex);
            Assert.Equal(index.Value, updatedTestUser.Indexes[0].Value);
            Assert.Equal(BL_PartnerInvitationStatusTypeEnum.Submitted, updatedTestUser.PartnerInvitationStatus);

            //Read partner invitation back..
            var readPartnerInvitation = await BL.GetPartnerInvitations_byExtRefIdAsync(testUser.ExternalRefId);
            Assert.Equal(newPartnerInvitation.DateOfBirth, readPartnerInvitation.DateOfBirth);
            Assert.Equal(newPartnerInvitation.FirstName, readPartnerInvitation.FirstName);
            Assert.Equal(BL_PartnerInvitationStatusTypeEnum.Submitted, readPartnerInvitation.InvitationStatus);
            Assert.Equal(newPartnerInvitation.InvitedByUserId, readPartnerInvitation.InvitedByUserId);
            Assert.Equal(newPartnerInvitation.LastName, readPartnerInvitation.LastName);
            Assert.Equal(newPartnerInvitation.MobilePhone, readPartnerInvitation.MobilePhone);
            Assert.Equal(newPartnerInvitation.TypeOfGender, readPartnerInvitation.TypeOfGender);
            Assert.Equal(readPartnerInvitation.InvitationId, new Guid(readPartnerInvitation.InvitationId).ToString().ToUpper());
            Assert.False(string.IsNullOrWhiteSpace(readPartnerInvitation.InvitationId), "InvitationId is empty, which is not the expected result");
        }

        [Fact]
        public async Task CreateNewInvitation_Validations()
        {
            //Create unique user for this test
            var extRefid = $"{GetType().Name}.{GetCallerMemberName()}";
            var testUser = await BL.CreateUserAsync($"{extRefid}@lifecouple.net", extRefid, true, "fName", "lName", null);

            //Update the userprofiles so we can check that it is not overwritten later...
            testUser.FirstName = "per";
            testUser.HasAgreedToTandC = false;
            var index = new DomainLogic.BL_UserProfile.Index { TypeOfIndex = DAL.Entities.IndexTypeEnum.Overall, Value = 50 };
            testUser.Indexes = new List<DomainLogic.BL_UserProfile.Index> { index };
            await BL.SetUserAsync(testUser);


            //Initialize new entity to be saved
            var dob = DateTime.Now.AddYears(-21);
            var newPartnerInvitation = new BL_PartnerInvitation
            {
                DateOfBirth = dob,
                FirstName = "",
                InvitedByUserId = "",
                LastName = "",
                MobilePhone = "",
                TypeOfGender = BL_GenderTypeEnum.Unknown,
            };

            var ex = await Assert.ThrowsAsync<BusinessLogicException>(async () => await BL.SetPartnerInvitationsAsync(newPartnerInvitation));
            Assert.Equal(ValidationErrorCode.EntityIsNull, ex.ErrorCode);
            Assert.Single(ex.Result.Failures);

            //Fix data failed above
            newPartnerInvitation.InvitedByUserId = testUser.Id;
            ex = await Assert.ThrowsAsync<BusinessLogicException>(async () => await BL.SetPartnerInvitationsAsync(newPartnerInvitation));
            Assert.Equal(ValidationErrorCode.DateOfBirth_ValidationError, ex.ErrorCode);
            Assert.Equal(6, ex.Result.Failures.Count);

            //Read new entity back
            var updatedTestUser = await BL.GetUserProfile_byExtRefIdAsync(testUser.ExternalRefId);
            Assert.NotNull(updatedTestUser);
            Assert.Equal(testUser.FirstName, updatedTestUser.FirstName);
            Assert.Equal(testUser.HasAgreedToTandC, updatedTestUser.HasAgreedToTandC);
            Assert.Equal(index.TypeOfIndex, updatedTestUser.Indexes[0].TypeOfIndex);
            Assert.Equal(index.Value, updatedTestUser.Indexes[0].Value);
            Assert.Equal(BL_PartnerInvitationStatusTypeEnum.Unknown, updatedTestUser.PartnerInvitationStatus);
        }

        [Fact]
        public async Task CreateInvitationSmsMessageBody()
        {
            var messageBody = await BL.GetPartnerInvitationSmsBodyAsync("Jane", "12345abcd", BL_GenderTypeEnum.Female);
            Assert.Equal("Jane joined LifeCouple and invited you to join her - https://toBeDefined.azurewebsites.net/pi/lp?id=12345abcd", messageBody);

            messageBody = await BL.GetPartnerInvitationSmsBodyAsync("John", "abcd", BL_GenderTypeEnum.Male);
            Assert.Equal("John joined LifeCouple and invited you to join him - https://toBeDefined.azurewebsites.net/pi/lp?id=abcd", messageBody);

            messageBody = await BL.GetPartnerInvitationSmsBodyAsync("Kim", "xyz", BL_GenderTypeEnum.NotSpecified);
            Assert.Equal("Kim joined LifeCouple and invited you to join - https://toBeDefined.azurewebsites.net/pi/lp?id=xyz", messageBody);

            messageBody = await BL.GetPartnerInvitationSmsBodyAsync("Jimbo", "xyz", BL_GenderTypeEnum.Unknown);
            Assert.Equal("Jimbo joined LifeCouple and invited you to join - https://toBeDefined.azurewebsites.net/pi/lp?id=xyz", messageBody);

        }



    }
}
