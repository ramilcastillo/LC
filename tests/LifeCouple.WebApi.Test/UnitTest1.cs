using System;
using Xunit;

namespace LifeCouple.WebApi.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var x = "test";
            Assert.Equal("test", x);
        }
    }
}
