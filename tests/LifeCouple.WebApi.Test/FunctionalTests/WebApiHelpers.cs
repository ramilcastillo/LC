using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace LifeCouple.WebApi.Test.FunctionalTests
{
    public class WebApiHelpers
    {
        public const string ClientKey = "1514BF2C-4F9F-4E97-B452-877695B1FF2C";

        TestServer _server;

        public string ClientKeyValue { get { return ClientKey; } }

        public WebApiHelpers(TestServer testServer)
        {
            _server = testServer;
        }


        public async Task<PartnerInvitationRequestInfo> CreatePartnerInvitation(HttpClient client, [CallerMemberName]string callerMemberName = null, [CallerFilePath] string assemblyFilePath = null)
        {
            var postRequest = new PartnerInvitationRequestInfo
            {
                DateOfBirth = DateTime.Now.AddYears(-21).ExGetDateOfBirth(),
                FirstName = $"{callerMemberName}",
                LastName = $"{assemblyFilePath.ExGetFileNameFromAssemblyPath()}",
                MobilePhone = "7605006125",
                TypeOfGender = GenderType.Male,
            };
            var postResponse = await client.PostAsync(ApiEndpoints.userprofiles_me_partnerinvitation, BaseWebApiTest.GetJsonContent(postRequest));
            postResponse.EnsureSuccessStatusCode();
            return postRequest;
        }

        public async Task<PartnerInvitationResponseInfo> GetPartnerInvitation(HttpClient client, [CallerMemberName]string callerMemberName = null, [CallerFilePath] string assemblyFilePath = null)
        {
            //Get current data again (after Post)
            var getResponse = await client.GetAsync(ApiEndpoints.userprofiles_me_partnerinvitation);
            getResponse.EnsureSuccessStatusCode();
            return Deserialize<PartnerInvitationResponseInfo>(await getResponse.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Create and Update a new userProfile and return the AuthToken that can be used for subsequent web api calls for that userProfile
        /// The passed in http client will be updated with relevant AuthToken (same as being returned)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="assemblyFilePath"></param>
        /// <returns></returns>
        public async Task<string> CreateDevUserProfile(HttpClient client, [CallerMemberName]string callerMemberName = null, [CallerFilePath] string assemblyFilePath = null)
        {
            //Add clientKey token to header
            BaseWebApiTest.SetAuthToken(client, ClientKey, false);

            //Create Dev user
            var newEmail = $"Test-{assemblyFilePath.ExGetFileNameFromAssemblyPath()}.{callerMemberName}@lifecouple.net";
            var userProfileRequestInfo = new UserProfileRequestInfo { Email = newEmail };
            var postCreateUserResponse = await client.PostAsync($"api/userprofiles/devuser", BaseWebApiTest.GetJsonContent(userProfileRequestInfo));
            postCreateUserResponse.EnsureSuccessStatusCode();

            //Create Token
            var tokenRequestInfo = new TokenRequestInfo { Email = newEmail, Password = newEmail.Split('@')[0] + "@" };
            var postResponse = await client.PostAsync($"api/tokens", BaseWebApiTest.GetJsonContent(tokenRequestInfo));
            postResponse.EnsureSuccessStatusCode();
            var tokenResponseInfo = BaseWebApiTest.Deserialize<TokenResponseInfo>(await postResponse.Content.ReadAsStringAsync());

            //Add token to header
            BaseWebApiTest.SetAuthToken(client, tokenResponseInfo.Token, true);

            // Set User profile
            var reqPayload = new UserProfileRegAboutYouRequestInfo
            {
                DateOfBirth = DateTime.Now.AddYears(-20),
                FirstName = "Tester",
                LastName = "Testlastname",
                Gender = "m",
                MobilePhone = "7605006125",
                NotificationOption = "True"
            };
            postResponse = await client.PutAsync(ApiEndpoints.userprofiles_me_registration_aboutyou, BaseWebApiTest.GetJsonContent(reqPayload));
            postResponse.EnsureSuccessStatusCode();

            return tokenResponseInfo.Token;
        }

        public async Task SetUserprofileRelationship(HttpClient client)
        {
            // Set User profile
            var reqPayload = new UserProfileRegAboutYourRelationshipRequestInfo
            {
                BeenToCounselorOrTherapist = BeenToCounselorOrTherapistType.NoNever,
                HasMoreThanOneMarriage = false,
                IsMarried = true,
                LastWeddingDate = DateTime.Now.AddYears(-20),
                NrOfChildren = 1,
                NrOfStepChildren = 0
            };

            var postResponse = await client.PutAsync(ApiEndpoints.userprofiles_me_registration_aboutyourrelationship, BaseWebApiTest.GetJsonContent(reqPayload));
            postResponse.EnsureSuccessStatusCode();
        }

        public async Task<UserProfileRegAboutYouResponseInfo> GetUserProfileAboutYou(HttpClient client, [CallerMemberName]string callerMemberName = null, [CallerFilePath] string assemblyFilePath = null)
        {
            var resp = await client.GetAsync(ApiEndpoints.userprofiles_me_registration_aboutyou);
            resp.EnsureSuccessStatusCode();
            var userProfileRegAboutYouResponseInfo = Deserialize<UserProfileRegAboutYouResponseInfo>(await resp.Content.ReadAsStringAsync());

            return userProfileRegAboutYouResponseInfo;
        }

        public async Task<UserProfileResponseInfo> GetUserProfile(HttpClient client, [CallerMemberName]string callerMemberName = null, [CallerFilePath] string assemblyFilePath = null)
        {
            var resp = await client.GetAsync(ApiEndpoints.userprofiles_me);
            resp.EnsureSuccessStatusCode();
            var userProfileResponseInfo = Deserialize<UserProfileResponseInfo>(await resp.Content.ReadAsStringAsync());

            return userProfileResponseInfo;
        }


        public T DeserializeObj<T>(string jsonString) => Deserialize<T>(jsonString);

        public async Task<T> ReadContent<T>(HttpResponseMessage httpResponse) => DeserializeObj<T>(await httpResponse.Content.ReadAsStringAsync());

        public async Task ContentAsserIsEmpty(HttpResponseMessage httpResponse, string userMessage = null)
        {
            var body = await httpResponse.Content.ReadAsStringAsync();
            Assert.True(string.IsNullOrEmpty(body), $"Expected content to be empty, was '{body}'. {userMessage}");
        }

        public static T Deserialize<T>(string jsonString) => JsonConvert.DeserializeObject<T>(jsonString);
    }
}
