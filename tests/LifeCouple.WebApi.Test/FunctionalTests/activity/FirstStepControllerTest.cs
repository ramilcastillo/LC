using System.Net;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using Xunit;
using Xunit.Abstractions;

namespace LifeCouple.WebApi.Test.FunctionalTests.activity
{
    public class FirstStepControllerTest : BaseWebApiTest
    {
        public FirstStepControllerTest(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async Task Get_Ok()
        {
            //Get current data
            var getResponse = await _clientWithToken.GetAsync(ApiEndpoints.userprofiles_me_activity_firststep);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getResponsebody = await getResponse.Content.ReadAsStringAsync();
            Assert.Equal("{\"overview\":{\"activityKey\":\"FirstStep\",\"videoUrl\":\"https://medialc-uswe.streaming.media.azure.net/ff6da407-cbb8-492b-903f-eb32eb4d24d7/LifeCouple%20Promo%20Gavin_180619.ism/manifest\",\"videoComment\":\"Let's start with something easy. Watch this short overview video.\",\"objective\":\"Is a fun and playful way to work on an area in your relationship to get notice and recognition by your partner.\",\"activityLevel\":\"Level 1\",\"format\":\"1 Part\",\"duration\":\"This activity is intended to be completed within 1 week.\",\"tip\":\"LifeCouple Tip: Be genuine in your actions as your partner will be asked about what you decided to focus in this activity\"},\"activityTypeOfStatus\":100,\"selectedAreaKey\":null,\"actionValue\":null,\"isAgreed\":null,\"areas\":[{\"areaKey\":\"Communication\",\"description\":\"Asking your partner their point of view or better listening\",\"title\":\"Communication\"},{\"areaKey\":\"Chores\",\"description\":\"Taking on things you don\u2019t  usually do in the household\",\"title\":\"Chores\"},{\"areaKey\":\"Romance\",\"description\":\"Doing things that can spark romance\",\"title\":\"Romance\"},{\"areaKey\":\"OwnershipCertificate\",\"description\":\"Say \\\"I\u2019m sorry\\\" for something that upset your partner\",\"title\":\"Ownership Certificate\"},{\"areaKey\":\"QualityTime\",\"description\":\"Make an effort to create quality time together\",\"title\":\"Quality Time\"},{\"areaKey\":\"Tone\",\"description\":\"When talking to your partner,  change or watch your tone\",\"title\":\"Tone\"},{\"areaKey\":\"Intensity\",\"description\":\"Take the vibe to fun and light hearted\",\"title\":\"Intensity\"}]}", getResponsebody);


            var getPayload = Deserialize<Activity_FirstStep_ResponseInfo>(getResponsebody);
            Assert.Null(getPayload.ActionValue);
            Assert.Equal(ActivityStatusType.NotStarted, getPayload.ActivityTypeOfStatus);
            Assert.Equal(7, getPayload.Areas.Count);
            Assert.Equal("Communication", getPayload.Areas[0].AreaKey);
            Assert.Equal("Chores", getPayload.Areas[1].AreaKey);
            Assert.Equal("Romance", getPayload.Areas[2].AreaKey);
            Assert.Equal("OwnershipCertificate", getPayload.Areas[3].AreaKey);
            Assert.Equal("QualityTime", getPayload.Areas[4].AreaKey);
            Assert.Equal("Tone", getPayload.Areas[5].AreaKey);
            Assert.Equal("Intensity", getPayload.Areas[6].AreaKey);
            Assert.Null(getPayload.IsAgreed);
            Assert.Equal("FirstStep", getPayload.Overview.ActivityKey);
            Assert.Equal("Level 1", getPayload.Overview.ActivityLevel);
            Assert.Equal("This activity is intended to be completed within 1 week.", getPayload.Overview.Duration);
            Assert.Equal("1 Part", getPayload.Overview.Format);
            Assert.Equal("Is a fun and playful way to work on an area in your relationship to get notice and recognition by your partner.", getPayload.Overview.Objective);
            Assert.Equal("LifeCouple Tip: Be genuine in your actions as your partner will be asked about what you decided to focus in this activity", getPayload.Overview.Tip);
            Assert.Equal("Let's start with something easy. Watch this short overview video.", getPayload.Overview.VideoComment);
            Assert.Equal(@"https://medialc-uswe.streaming.media.azure.net/ff6da407-cbb8-492b-903f-eb32eb4d24d7/LifeCouple%20Promo%20Gavin_180619.ism/manifest", getPayload.Overview.VideoUrl);
            Assert.Null(getPayload.SelectedAreaKey);

        }
    }
}
