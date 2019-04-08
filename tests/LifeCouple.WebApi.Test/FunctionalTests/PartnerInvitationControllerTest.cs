using System;
using System.Net;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using Xunit;
using Xunit.Abstractions;

namespace LifeCouple.WebApi.Test.FunctionalTests
{
    public class PartnerInvitationControllerTest : BaseWebApiTest
    {
        public PartnerInvitationControllerTest(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async Task Get_and_Post_Ok()
        {
            //Get current data
            var getResponse = await _clientWithToken.GetAsync(ApiEndpoints.userprofiles_me_partnerinvitation);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
            var getResponsebody = await getResponse.Content.ReadAsStringAsync();
            Assert.Equal("{\"error\":{\"code\":\"NotFound\",\"message\":\"No data found for UserId EA5CEE24-90F9-4804-A45A-4D2E7FCA5D1A\",\"debugMessage\":null}}", getResponsebody);

            //POST data (create invitation)
            var postRequest = new PartnerInvitationRequestInfo
            {
                DateOfBirth = DateTime.Now.AddYears(-21).ExGetDateOfBirth(),
                FirstName = "fName",
                LastName = "lastName",
                MobilePhone = "7605006125",
                TypeOfGender = GenderType.Male,
            };
            var postResponse = await _clientWithToken.PostAsync(ApiEndpoints.userprofiles_me_partnerinvitation, GetJsonContent(postRequest));
            Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
            var postResponseBody = await postResponse.Content.ReadAsStringAsync();
            Assert.Equal("", postResponseBody);

            //Get current data again (after Post)
            getResponse = await _clientWithToken.GetAsync(ApiEndpoints.userprofiles_me_partnerinvitation);
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            getResponsebody = await getResponse.Content.ReadAsStringAsync();
            var payload = Deserialize<PartnerInvitationResponseInfo>(getResponsebody);
            Assert.Equal(postRequest.DateOfBirth, payload.DateOfBirth);
            Assert.Equal(postRequest.FirstName, payload.FirstName);
            Assert.Equal(PartnerInvitationStatusType.Submitted, payload.InvitationStatus);
            Assert.Equal(postRequest.LastName, payload.LastName);
            Assert.Equal(postRequest.MobilePhone, payload.MobilePhone);
            Assert.Equal(postRequest.TypeOfGender, payload.TypeOfGender);
            Assert.Equal(payload.InvitationId, new Guid(payload.InvitationId).ToString().ToUpper());

            //Resend Invitation
            postResponse = await _clientWithToken.PostAsync("api/userprofiles/me/partnerinvitation/resendinvite", null);
            Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
            postResponseBody = await postResponse.Content.ReadAsStringAsync();
            Assert.Equal("", postResponseBody);


        }

    }
}
