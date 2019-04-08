using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LifeCouple.WebApi.Test.FunctionalTests
{
    public class SystemStatusControllerGET : BaseWebApiTest
    {
        public SystemStatusControllerGET(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async Task ReturnSuccess()
        {
            var response = await _clientWithToken.GetAsync("/api/systemstatus?$clientAccessKey=1514BF2C-4F9F-4E97-B452-877695B1FF2C");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Assert.Empty(response.Headers.Server);
            Assert.False(response.Headers.Contains("X-Powered-By"));
        }

        [Fact]
        public async Task ReturnSuccessOK()
        {
            var response = await _clientWithToken.GetAsync("/api/systemstatus?$clientAccessKey=1514BF2C-4F9F-4E97-B452-877695B1FF2C");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

        }

        [Fact]
        public async Task Return401()
        {
            var response = await _clientWithToken.GetAsync("/api/systemstatus?$clientAccessKey=dummy");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);

            response = await _clientWithToken.GetAsync("/api/systemstatus");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}
