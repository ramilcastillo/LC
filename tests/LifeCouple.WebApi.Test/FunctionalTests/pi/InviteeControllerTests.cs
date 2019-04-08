using System.Net;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using LifeCouple.WebApi.Common;
using Xunit;
using Xunit.Abstractions;

namespace LifeCouple.WebApi.Test.FunctionalTests.pi
{
    public class InviteeControllerTests : BaseWebApiTest
    {
        public InviteeControllerTests(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async Task Accept_401()
        {
            //Get current data
            var id = "1234";
            var getResponse = await _clientWithToken.PutAsync($"api/pi/{id}/accept", null);
            Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
            var getResponsebody = await getResponse.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task Decline_401()
        {
            //Get current data
            var id = "1234";
            var getResponse = await _clientWithToken.PutAsync($"api/pi/{id}/decline", null);
            Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
            var getResponsebody = await getResponse.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task Accept_Decline_204()
        {
            var client = CreateClientWithToken("", false);

            //Create User 
            var authToken = await Helper.CreateDevUserProfile(client);

            //Invite Partner and get invitation
            await Helper.CreatePartnerInvitation(client);
            var invite = await Helper.GetPartnerInvitation(client);

            //Partner Accepts invitation - web api to be called from WebApp
            SetAuthToken(client, Helper.ClientKeyValue, false);
            var getResponse = await client.PutAsync($"{ApiEndpoints.pi}{invite.InvitationId}/accept", null);
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, getResponse.StatusCode);
            var getResponsebody = await getResponse.Content.ReadAsStringAsync();
            Assert.Equal("", getResponsebody);

            SetAuthToken(client, Helper.ClientKeyValue, false);
            getResponse = await client.PutAsync($"{ApiEndpoints.pi}{invite.InvitationId}/decline", null);
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, getResponse.StatusCode);
            getResponsebody = await getResponse.Content.ReadAsStringAsync();
            Assert.Equal("", getResponsebody);
            SetAuthToken(client, authToken, true);
            invite = await Helper.GetPartnerInvitation(client);
            Assert.Equal(PartnerInvitationStatusType.Declined, invite.InvitationStatus);

            //Should be able to decline it again
            SetAuthToken(client, Helper.ClientKeyValue, false);
            getResponse = await client.PutAsync($"{ApiEndpoints.pi}{invite.InvitationId}/decline", null);
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, getResponse.StatusCode);
            getResponsebody = await getResponse.Content.ReadAsStringAsync();
            Assert.Equal("", getResponsebody);
            SetAuthToken(client, authToken, true);
            invite = await Helper.GetPartnerInvitation(client);
            Assert.Equal(PartnerInvitationStatusType.Declined, invite.InvitationStatus);

            SetAuthToken(client, Helper.ClientKeyValue, false);
            getResponse = await client.PutAsync($"{ApiEndpoints.pi}{invite.InvitationId}/accept", null);
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, getResponse.StatusCode);
            getResponsebody = await getResponse.Content.ReadAsStringAsync();
            Assert.Equal("", getResponsebody);
            SetAuthToken(client, authToken, true);
            invite = await Helper.GetPartnerInvitation(client);
            Assert.Equal(PartnerInvitationStatusType.Accepted, invite.InvitationStatus);

            //Should be able to accept it again
            SetAuthToken(client, Helper.ClientKeyValue, false);
            getResponse = await client.PutAsync($"{ApiEndpoints.pi}{invite.InvitationId}/accept", null);
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, getResponse.StatusCode);
            getResponsebody = await getResponse.Content.ReadAsStringAsync();
            Assert.Equal("", getResponsebody);
            SetAuthToken(client, authToken, true);
            invite = await Helper.GetPartnerInvitation(client);
            Assert.Equal(PartnerInvitationStatusType.Accepted, invite.InvitationStatus);

        }


        [Fact]
        public async Task Create_Invitee()
        {
            var client = CreateClientWithToken("", false);

            //Create User 
            var authToken = await Helper.CreateDevUserProfile(client);

            //Invite Partner and get invitation
            var invitationPosted = await Helper.CreatePartnerInvitation(client);
            var invite = await Helper.GetPartnerInvitation(client);

            //Try creating new invitee user but should fail since invitation status is Submitted
            SetAuthToken(client, authToken, true);
            var postResponse = await client.PostAsync($"{ApiEndpoints.pi}", GetJsonContent(new InviteeRequestInfo { InvitationId = invite.InvitationId }));
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
            var postResponseErrorPayload = await Helper.ReadContent<ErrorResponseInfo>(postResponse);
            Assert.Equal("ValidationFailure_PartnerInvitation", postResponseErrorPayload.Error.Code);
            Assert.StartsWith("Invalid Invitation Status, cannot create user for Invitation with Id '", postResponseErrorPayload.Error.DebugMessage);
            Assert.EndsWith("' and Email 'Test-InviteeControllerTests.Create_Invitee@lifecouple.net'", postResponseErrorPayload.Error.DebugMessage);
            Assert.Equal("BusinessLogicException", postResponseErrorPayload.Error.Message);

            //Change status to Accepted
            SetAuthToken(client, Helper.ClientKeyValue, false);
            var getResponse = await client.PutAsync($"{ApiEndpoints.pi}{invite.InvitationId}/accept", null);
            getResponse.EnsureSuccessStatusCode();

            //Try creating new invitee user but should fail since Privacy and T&C is not set
            SetAuthToken(client, authToken, true);
            postResponse = await client.PostAsync($"{ApiEndpoints.pi}", GetJsonContent(new InviteeRequestInfo { InvitationId = invite.InvitationId }));
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
            postResponseErrorPayload = await Helper.ReadContent<ErrorResponseInfo>(postResponse);
            Assert.Equal("ValidationFailure_UserProfile_PrivacyPolicy", postResponseErrorPayload.Error.Code);
            Assert.StartsWith("Privacy policy must be set to true for Invitation with Id '", postResponseErrorPayload.Error.DebugMessage);
            Assert.Equal("BusinessLogicException", postResponseErrorPayload.Error.Message);

            //Try creating new invitee user but should fail since T&C is not set
            SetAuthToken(client, authToken, true);
            postResponse = await client.PostAsync($"{ApiEndpoints.pi}", GetJsonContent(new InviteeRequestInfo { InvitationId = invite.InvitationId, HasAgreedToPrivacyPolicy = true }));
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
            postResponseErrorPayload = await Helper.ReadContent<ErrorResponseInfo>(postResponse);
            Assert.Equal("ValidationFailure_UserProfile_TermsOfService", postResponseErrorPayload.Error.Code);
            Assert.StartsWith("TandC policy must be set to true for Invitation with Id '", postResponseErrorPayload.Error.DebugMessage);
            Assert.Equal("BusinessLogicException", postResponseErrorPayload.Error.Message);


            //Try creating new invitee user but should fail since user does not have a relationship id
            SetAuthToken(client, authToken, true);
            postResponse = await client.PostAsync($"{ApiEndpoints.pi}", GetJsonContent(new InviteeRequestInfo { InvitationId = invite.InvitationId, HasAgreedToPrivacyPolicy = true, HasAgreedToTandC = true }));
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
            postResponseErrorPayload = await Helper.ReadContent<ErrorResponseInfo>(postResponse);
            Assert.Equal("ValidationFailure_UserProfile_RelationshipId", postResponseErrorPayload.Error.Code);
            Assert.StartsWith("Relationship must be set for Inviter with Id '", postResponseErrorPayload.Error.DebugMessage);
            Assert.EndsWith($"' Invitation with Id '{invite.InvitationId}' and Email 'Test-InviteeControllerTests.Create_Invitee@lifecouple.net'", postResponseErrorPayload.Error.DebugMessage);
            Assert.Equal("BusinessLogicException", postResponseErrorPayload.Error.Message);

            //Token below was manually generated using http calls agains lcapicitest endpoint - see invitee.http
            var inviteeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5RDFFQThCNzg0MEQ0MTk1QTEyNUY0M0UzRjVDRUNBRSIsImp0aSI6IjM0NzVjN2ZlZGRkNzQwZDU5NWI5NWJiZjRkYzVmYWEzIiwidW5pcXVlX25hbWUiOiI5RDFFQThCNzg0MEQ0MTk1QTEyNUY0M0UzRjVDRUNBRSIsImVtYWlsIjoiVGVzdC1JbnZpdGVlQ29udHJvbGxlclRlc3RzLkNyZWF0ZV9JbnZpdGVlLUludml0ZWVAbGlmZWNvdXBsZS5uZXQiLCJvaWQiOiI5RDFFQThCNzg0MEQ0MTk1QTEyNUY0M0UzRjVDRUNBRSIsImZhbWlseV9uYW1lIjoiVEVTVC1JTlZJVEVFQ09OVFJPTExFUlRFU1RTLkNSRUFURV9JTlZJVEVFLUlOVklURUVATElGRUNPVVBMRS5ORVQiLCJnaXZlbl9uYW1lIjoidGVzdC1pbnZpdGVlY29udHJvbGxlcnRlc3RzLmNyZWF0ZV9pbnZpdGVlLWludml0ZWVAbGlmZWNvdXBsZS5uZXQiLCJuYmYiOjE1MzU1ODU5MzYsImV4cCI6MTUzODI2NDMzNiwiaWF0IjoxNTM1NTg1OTM2LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0Ojg4ODgiLCJhdWQiOiJ1c2VycyJ9.ldimgswPo0nYGH_8isjL_rkdlZBV4a_O5vAXz1HhYMw";

            //User correct User Id, but should fail becasue no relationship exist
            SetAuthToken(client, inviteeToken, true);
            postResponse = await client.PostAsync($"{ApiEndpoints.pi}", GetJsonContent(new InviteeRequestInfo { InvitationId = invite.InvitationId, HasAgreedToPrivacyPolicy = true, HasAgreedToTandC = true }));
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
            postResponseErrorPayload = await Helper.ReadContent<ErrorResponseInfo>(postResponse);
            Assert.Equal("ValidationFailure_UserProfile_RelationshipId", postResponseErrorPayload.Error.Code);
            Assert.StartsWith("Relationship must be set for Inviter with Id '", postResponseErrorPayload.Error.DebugMessage);
            Assert.EndsWith($"' Invitation with Id '{invite.InvitationId}' and Email 'Test-InviteeControllerTests.Create_Invitee-Invitee@lifecouple.net'", postResponseErrorPayload.Error.DebugMessage);
            Assert.Equal("BusinessLogicException", postResponseErrorPayload.Error.Message);

            //Create Relationsip data for Inviteer
            SetAuthToken(client, authToken, true);
            await Helper.SetUserprofileRelationship(client);


            //Try creating new invitee user but should fail since user already exists
            SetAuthToken(client, authToken, true);
            postResponse = await client.PostAsync($"{ApiEndpoints.pi}", GetJsonContent(new InviteeRequestInfo { InvitationId = invite.InvitationId, HasAgreedToPrivacyPolicy = true, HasAgreedToTandC = true }));
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
            postResponseErrorPayload = await Helper.ReadContent<ErrorResponseInfo>(postResponse);
            Assert.Equal("ValidationFailure_UserProfile_AlreadyExists", postResponseErrorPayload.Error.Code);
            Assert.StartsWith("Unable to create user with ExtRefenceNr: '", postResponseErrorPayload.Error.DebugMessage);
            Assert.EndsWith("' and Email: 'Test-InviteeControllerTests.Create_Invitee@lifecouple.net' since it already exists.", postResponseErrorPayload.Error.DebugMessage);
            Assert.Equal("BusinessLogicException", postResponseErrorPayload.Error.Message);


            //HAPPY PATH
            //Assumes that the user have gotten the JWT token from ADB2C and that is now passed in the header
            SetAuthToken(client, inviteeToken, true);
            postResponse = await client.PostAsync($"{ApiEndpoints.pi}", GetJsonContent(new InviteeRequestInfo { InvitationId = invite.InvitationId, HasAgreedToPrivacyPolicy = true, HasAgreedToTandC = true }));
            await Helper.ContentAsserIsEmpty(postResponse);
            Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);

            //Get Invitee User
            var inviteeUserprofile = await Helper.GetUserProfile(client);
            Assert.Equal(PartnerInvitationStatusType.Completed, inviteeUserprofile.PartnerInvitationStatus);


            //Get Inviteer User
            SetAuthToken(client, authToken, true);
            var inviterUserprofile = await Helper.GetUserProfile(client);
            Assert.Equal(PartnerInvitationStatusType.Completed, inviterUserprofile.PartnerInvitationStatus);


            //Emotional Bank Account Tests
            //======================================================================================
            //Inviter - Post new eba txn 50
            SetAuthToken(client, authToken, true);
            postResponse = await client.PostAsync("api/userprofiles/me/eba", GetJsonContent(new EbaRequestInfo { Comment = "deposited 50 points", PointsToDeposit = 50 }));
            Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
            var postPayload = Helper.ContentAsserIsEmpty(postResponse);

            //Inviter - Post new eba txn 100
            SetAuthToken(client, authToken, true);
            postResponse = await client.PostAsync("api/userprofiles/me/eba", GetJsonContent(new EbaRequestInfo { Comment = "deposited 100 points", PointsToDeposit = 100 }));
            Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
            postPayload = Helper.ContentAsserIsEmpty(postResponse);

            //Invitee - Post new eba txn 50
            SetAuthToken(client, inviteeToken, true);
            postResponse = await client.PostAsync("api/userprofiles/me/eba", GetJsonContent(new EbaRequestInfo { Comment = "deposited 50 points", PointsToDeposit = 50 }));
            Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
            postPayload = Helper.ContentAsserIsEmpty(postResponse);

            //Invitee - Post new eba txn 25
            SetAuthToken(client, inviteeToken, true);
            postResponse = await client.PostAsync("api/userprofiles/me/eba", GetJsonContent(new EbaRequestInfo { Comment = "deposited 150 points", PointsToDeposit = 25 }));
            Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
            postPayload = Helper.ContentAsserIsEmpty(postResponse);

            //Invitee - Get EBA
            getResponse = await client.GetAsync("api/userprofiles/me/eba");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var getResponsebody = await getResponse.Content.ReadAsStringAsync();
            var getPayload = Deserialize<EbaResponseInfo>(getResponsebody);
            Assert.Equal(150, getPayload.EbaPointsBalance);
            Assert.Equal(75, getPayload.EbaPointsDeposited);
            Assert.Equal(3, getPayload.RecentTransactions.Count);

            //Inviter - Get EBA
            SetAuthToken(client, authToken, true);
            getResponse = await client.GetAsync("api/userprofiles/me/eba");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            getResponsebody = await getResponse.Content.ReadAsStringAsync();
            getPayload = Deserialize<EbaResponseInfo>(getResponsebody);
            Assert.Equal(75, getPayload.EbaPointsBalance);
            Assert.Equal(150, getPayload.EbaPointsDeposited);
            Assert.Equal(3, getPayload.RecentTransactions.Count);

        }
    }
}
