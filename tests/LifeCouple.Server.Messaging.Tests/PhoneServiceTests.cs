using System.Threading.Tasks;
using Xunit;

namespace LifeCouple.Server.Messaging.Tests
{
    public class PhoneServiceTests : MessagingTestBase
    {

        private static PhoneService svc = new PhoneService(settingsOptions);

        [Fact]
        public async Task Lookup_Ok()
        {
            var phonenr = await svc.GetPhoneNrInfo("7605006125");

            Assert.True(phonenr.IsValid);
            Assert.Equal("US", phonenr.CountryCode);
            Assert.Equal("(760) 500-6125", phonenr.NationalFormat);
            Assert.Equal("+17605006125", phonenr.PhoneNr);
            Assert.Equal(PhoneNrInfo.TypeOfNumberEnum.Mobile, phonenr.TypeOfNumber);
        }

        [Fact]
        public async Task Lookup_Failed()
        {
            var phonenr = await svc.GetPhoneNrInfo("760500612");
            Assert.False(phonenr.IsValid);
            Assert.Null(phonenr.CountryCode);
            Assert.Null(phonenr.NationalFormat);
            Assert.Null(phonenr.PhoneNr);
            Assert.Equal(PhoneNrInfo.TypeOfNumberEnum.Unknown, phonenr.TypeOfNumber);
        }

        [Fact]
        public async Task Lookup_OtherTypeOfPhoneNr()
        {
            var phonenr = await svc.GetPhoneNrInfo("8582072258");
            Assert.True(phonenr.IsValid);
            Assert.Equal("US", phonenr.CountryCode);
            Assert.Equal("(858) 207-2258", phonenr.NationalFormat);
            Assert.Equal("+18582072258", phonenr.PhoneNr);
            Assert.Equal(PhoneNrInfo.TypeOfNumberEnum.Other, phonenr.TypeOfNumber);
        }

        [Fact]
        public async Task Lookup_SwedisNrOther()
        {
            var phonenr = await svc.GetPhoneNrInfo("468201000");
            Assert.True(phonenr.IsValid);
            Assert.Equal("SE", phonenr.CountryCode);
            Assert.Equal("08-20 10 00", phonenr.NationalFormat);
            Assert.Equal("+468201000", phonenr.PhoneNr);
            Assert.Equal(PhoneNrInfo.TypeOfNumberEnum.Other, phonenr.TypeOfNumber);
        }

        [Fact]
        public async Task Lookup_SwedisNrMobile()
        {
            var phonenr = await svc.GetPhoneNrInfo("46705594050");
            Assert.True(phonenr.IsValid);
            Assert.Equal("SE", phonenr.CountryCode);
            Assert.Equal("070-559 40 50", phonenr.NationalFormat);
            Assert.Equal("+46705594050", phonenr.PhoneNr);
            Assert.Equal(PhoneNrInfo.TypeOfNumberEnum.Mobile, phonenr.TypeOfNumber);
        }

        [Fact]
        public async Task Lookup_SwedishFailed()
        {
            var phonenr = await svc.GetPhoneNrInfo("0705594050");
            Assert.False(phonenr.IsValid);
            Assert.Null(phonenr.CountryCode);
            Assert.Null(phonenr.NationalFormat);
            Assert.Null(phonenr.PhoneNr);
            Assert.Equal(PhoneNrInfo.TypeOfNumberEnum.Unknown, phonenr.TypeOfNumber);
        }

        [Fact]
        public async Task Submit_SMS_Message_Request()
        {
            await svc.PublishSmsDtoAsync("+17605006125", $"Test SMS generated from {GetType().Name}.{GetCallerMemberName()}");
        }

        [Fact]
        public async Task Submit_Long_SMS_Message_Request()
        {
            await svc.PublishSmsDtoAsync("+17605006125", $"Jane just joined LifeCouple - a new and easy way to use your phone to help work on your relationship. She's invited you to join her. Click this link to join: https://toBeDefined.azurewebsites.net/partnerInvite?id=414590FE-369F-45F8-AB1B-736E53E5591A");
        }


    }
}
