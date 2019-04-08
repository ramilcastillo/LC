using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LifeCouple.WebApi.Test.FunctionalTests
{
    public class UserProfilesRegistrationCntrlrTests : BaseWebApiTest
    {
        public UserProfilesRegistrationCntrlrTests(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async Task PaymentAndTermsGETandPUT()
        {
            //Step 1 - Get current data
            var responseGET = await _clientWithToken.GetAsync("/api/userprofiles/me/registration/paymentandterms");
            var bodyGET = await responseGET.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, responseGET.StatusCode);
            Assert.Equal(@"{""hasAgreedToTandC"":null,""hasAgreedToPrivacyPolicy"":null,""hasAgreedToRefundPolicy"":null}", bodyGET);

            //Step 2 - PUT data
            var contentPUT = GetJsonContent(@"{
  ""hasAgreedToTandC"": false,
  ""hasAgreedToPrivacyPolicy"": false,
  ""hasAgreedToRefundPolicy"": true
}");
            var responsePUT = await _clientWithToken.PutAsync("/api/userprofiles/me/registration/paymentandterms", contentPUT);
            var bodyPUT = await responsePUT.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.NoContent, responsePUT.StatusCode);
            Assert.DoesNotContain(@"{""error"":", bodyPUT);

            //Step 3 - Get data to verify Put
            responseGET = await _clientWithToken.GetAsync("/api/userprofiles/me/registration/paymentandterms");
            bodyGET = await responseGET.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, responseGET.StatusCode);
            Assert.Equal(@"{""hasAgreedToTandC"":false,""hasAgreedToPrivacyPolicy"":false,""hasAgreedToRefundPolicy"":true}", bodyGET);

            //Step 4 - PUT data
            contentPUT = GetJsonContent(@"{
  ""hasAgreedToTandC"": true,
  ""hasAgreedToPrivacyPolicy"": true,
  ""hasAgreedToRefundPolicy"": false
}");
            responsePUT = await _clientWithToken.PutAsync("/api/userprofiles/me/registration/paymentandterms", contentPUT);
            bodyPUT = await responsePUT.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.NoContent, responsePUT.StatusCode);
            Assert.DoesNotContain(@"{""error"":", bodyPUT);

            //Step 5 - Get data to verify Put
            responseGET = await _clientWithToken.GetAsync("/api/userprofiles/me/registration/paymentandterms");
            bodyGET = await responseGET.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, responseGET.StatusCode);
            Assert.Equal(@"{""hasAgreedToTandC"":true,""hasAgreedToPrivacyPolicy"":true,""hasAgreedToRefundPolicy"":false}", bodyGET);

        }

        [Fact]
        public async Task PaymentAndTermsPUT_Validation()
        {
            //Step 1 - Get current data
            var responseGET = await _clientWithToken.GetAsync("/api/userprofiles/me/registration/paymentandterms");
            var bodyGET = await responseGET.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, responseGET.StatusCode);

            //Step 2 - PUT data
            var contentPUT = GetJsonContent(@"{
  ""hasAgreedToTandC"": ""string of some value..."",
  ""hasAgreedToPrivacyPolicy"": false,
  ""hasAgreedToRefundPolicy"": true
}");
            var responsePUT = await _clientWithToken.PutAsync("/api/userprofiles/me/registration/paymentandterms", contentPUT);
            var bodyPUT = await responsePUT.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, responsePUT.StatusCode);
            Assert.Equal("{\"error\":{\"code\":\"InvalidModelState\",\"message\":\"Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary\",\"debugMessage\":null}}", bodyPUT);


        }
    }
}
