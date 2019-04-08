using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using LifeCouple.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace LifeCouple.WebApi.Test.FunctionalTests
{
    public class BaseWebApiTest
    {
        public static readonly TestServer _testServer = CreateTestServer();
        public static readonly HttpClient _clientWithToken = CreateClientWithToken(GetToken());
        private readonly ITestOutputHelper output;

        public WebApiHelpers Helper { get; }


        public BaseWebApiTest(ITestOutputHelper output)
        {
            this.output = output;

            var msg = $"BaseWebApiTest - ctor - {GetHashCode()}";
            output.WriteLine(msg);
            System.Diagnostics.Debug.WriteLine(msg);

            Helper = new WebApiHelpers(_testServer);
        }

        /// <summary>
        /// If invalid use VS Code to call api/tokens api...
        /// </summary>
        public static string GetToken()
        {

            //POST https://{{host}}/api/tokens HTTP/1.1
            //content - type: application / json
            //{
            //    "email": "pgrimskog@lifecouple.net",
            //    "password": "pgrimskog@"
            //}

            // 'Replace' below is simple a hack to prevent getting a Warning about it being a secure token.
            var response = new
            {
                token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJFQTVDRUUyNC05MEY5LTQ4MDQtQTQ1QS00RDJFN0ZDQTVEMUEiLCJqdGkiOiIyNTg1MTFkYTU0Yzc0OGJhODY2MDNlNWJkM2MzYTBlMyIsInVuaXF1ZV9uYW1lIjoiRUE1Q0VFMjQtOTBGOS00ODA0LUE0NUEtNEQyRTdGQ0E1RDFBIiwiZW1haWwiOiJwZ3JpbXNrb2dAbGlmZWNvdXBsZS5uZXQiLCJvaWQiOiJFQTVDRUUyNC05MEY5LTQ4MDQtQTQ1QS00RDJFN0ZDQTVEMUEiLCJmYW1pbHlfbmFtZSI6IkdyaW1za29nIiwiZ2l2ZW5fbmFtZSI6IlBlciIsIm5iZiI6MTUzNzYyNjU1NywiZXhwIjoxNTQwMzA0OTU3LCJpYXQiOjE1Mzc2MjY1NTcsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6ODg4OCIsImF1ZCI6InVzZXJzIn0.RcGVwcBWiLQutgP32kuuoTlMwxpe8cWGjZ0cFm_bsmE".Replace(" ", ""),
                expiration = "2018-10-23T14:29:17Z"
            };

            return response.token;
        }

        public static HttpClient CreateClientWithToken(string authToken, bool isBearerToken = true)
        {
            System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!!!!!!!! - CreateClientWithToken()");
            var client = _testServer.CreateClient();
            //TODO: C add call to get token so we don't need to keep on update it everytime it expires

            SetAuthToken(client, authToken, isBearerToken);

            System.Diagnostics.Debug.WriteLine("################### - CreateClientWithToken()");

            return client;
        }

        public static void SetAuthToken(HttpClient client, string authToken, bool isBearerToken)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", $"{(isBearerToken ? "Bearer " : "")}{authToken}");
            }
        }

        public static StringContent GetJsonContent(string jsonString) => new StringContent(jsonString, Encoding.UTF8, "application/json");

        public static StringContent GetJsonContent(object entity) => GetJsonContent(JsonConvert.SerializeObject(entity));

        public static T Deserialize<T>(string jsonString) => WebApiHelpers.Deserialize<T>(jsonString);

        public static string GetCallerMemberName([CallerMemberName]string name = "") => name;

        public static TestServer CreateTestServer()
        {
            System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!!!!!!!! - CreateTestServer()");
            // Arrange - changed based on: https://github.com/aspnet/Hosting/issues/1191 
            // See also https://github.com/aspnet/MetaPackages/blob/dev/src/Microsoft.AspNetCore/WebHost.cs#L148
            var builder = new WebHostBuilder().UseEnvironment("Test")
                .UseConfiguration(new ConfigurationBuilder()
                .AddUserSecrets("ac948920-353d-45bc-8520-906312b08889")
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build()).UseStartup<Startup>();
            var server = new TestServer(builder);
            System.Diagnostics.Debug.WriteLine("################### - CreateTestServer()");
            return server;

            //var hostBuilder = new WebHostBuilder();
            //var server2 = new TestServer(hostBuilder.UseEnvironment("Development")
            //    .UseConfiguration(new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json")
            //    .Build()
            //    )
            //    .UseStartup<Startup>());
            //return server2;
        }
    }
}
