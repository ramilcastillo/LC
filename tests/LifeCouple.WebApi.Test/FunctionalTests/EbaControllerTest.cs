using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using Xunit;
using Xunit.Abstractions;

namespace LifeCouple.WebApi.Test.FunctionalTests
{
    public class EbaControllerTest : BaseWebApiTest
    {
        public EbaControllerTest(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async Task Get_And_Post_Ok()
        {
            //Get current data
            var getResponse = await _clientWithToken.GetAsync(ApiEndpoints.userprofiles_me_eba);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var getResponsebody = await getResponse.Content.ReadAsStringAsync();

            var getPayload = Deserialize<EbaResponseInfo>(getResponsebody);
            Assert.Equal(3, getPayload.EbaPointOptions.Count);
            Assert.Equal(0, getPayload.EbaPointsBalance);
            Assert.Equal(0, getPayload.EbaPointsDeposited);
            Assert.Empty(getPayload.RecentTransactions);

            //Post new eba txn data
            var postResponse = await _clientWithToken.PostAsync(ApiEndpoints.userprofiles_me_eba, GetJsonContent(new EbaRequestInfo { Comment = "deposited 50 points", PointsToDeposit = 50 }));
            Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
            var postPayload = Helper.ContentAsserIsEmpty(postResponse);

            //Get Data 
            getResponse = await _clientWithToken.GetAsync(ApiEndpoints.userprofiles_me_eba);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            getResponsebody = await getResponse.Content.ReadAsStringAsync();
            getPayload = Deserialize<EbaResponseInfo>(getResponsebody);
            Assert.Equal(0, getPayload.EbaPointsBalance);
            Assert.Equal(50, getPayload.EbaPointsDeposited);
            Assert.Single(getPayload.RecentTransactions);

            //Post Second txn
            postResponse = await _clientWithToken.PostAsync(ApiEndpoints.userprofiles_me_eba, GetJsonContent(new EbaRequestInfo { Comment = "deposited 100 points", PointsToDeposit = 100 }));
            Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
            postPayload = Helper.ContentAsserIsEmpty(postResponse);

            //Get Data again
            getResponse = await _clientWithToken.GetAsync(ApiEndpoints.userprofiles_me_eba);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            getResponsebody = await getResponse.Content.ReadAsStringAsync();
            getPayload = Deserialize<EbaResponseInfo>(getResponsebody);
            Assert.Equal(0, getPayload.EbaPointsBalance);
            Assert.Equal(150, getPayload.EbaPointsDeposited);
            Assert.Equal(2, getPayload.RecentTransactions.Count);
            Assert.Equal("deposited 100 points", getPayload.RecentTransactions.First().Comment);
            Assert.Equal("Per-lc", getPayload.RecentTransactions.First().FirstName);
            Assert.Equal(EbaTransactionType.Sent, getPayload.RecentTransactions.First().TypeOfTransaction);
            Assert.Equal(100, getPayload.RecentTransactions.First().Point);
            Assert.Equal("deposited 50 points", getPayload.RecentTransactions.Last().Comment);
            Assert.Equal(50, getPayload.RecentTransactions.Last().Point);
            Assert.Equal("Per-lc", getPayload.RecentTransactions.Last().FirstName);
            Assert.Equal(EbaTransactionType.Sent, getPayload.RecentTransactions.Last().TypeOfTransaction);

        }

    }
}

