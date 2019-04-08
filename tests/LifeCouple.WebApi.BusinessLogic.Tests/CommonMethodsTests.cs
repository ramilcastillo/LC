using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LifeCouple.DAL;
using LifeCouple.WebApi.DomainLogic;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using LifeCouple.DAL.Entities;
namespace LifeCouple.WebApi.BusinessLogicTests
{
    public class CommonMethodsTests : BusinessLogicTestBase
    {
        [Fact]
        public void GetStringWithFirstCharacterUpper_Test()
        {
            var bl = BL;

            Assert.Null(bl.GetStringWithFirstCharacterUpper(null));
            Assert.Equal("", bl.GetStringWithFirstCharacterUpper(""));
            Assert.Equal("Abc", bl.GetStringWithFirstCharacterUpper("abc"));
            Assert.Equal("Ab", bl.GetStringWithFirstCharacterUpper("ab"));
            Assert.Equal("A", bl.GetStringWithFirstCharacterUpper("a"));
        }

    }
}
