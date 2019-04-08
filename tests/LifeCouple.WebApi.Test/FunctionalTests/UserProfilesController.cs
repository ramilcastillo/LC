using System.Threading.Tasks;
using LifeCouple.DTO.v;
using Xunit;
using Xunit.Abstractions;

namespace LifeCouple.WebApi.Test.FunctionalTests
{
    public class UserProfilesController : BaseWebApiTest
    {
        public UserProfilesController(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async Task Get_Me_Ok()
        {
            var response = await _clientWithToken.GetAsync("api/userprofiles/me");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var payload = Deserialize<UserProfileResponseInfo>(body);
            Assert.Equal(PartnerInvitationStatusType.Unknown, payload.PartnerInvitationStatus);

        }
    }
}
