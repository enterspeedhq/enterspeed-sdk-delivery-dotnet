using System;
using AutoFixture;
using Enterspeed.Delivery.Sdk.Domain.Helpers;
using Xunit;

namespace Enterspeed.Delivery.Sdk.Tests.Domain.Helpers
{
    public class QueryStringHelperTests
    {
        [Fact]
        public void Add_KeyNullOrEmptyString_Throws()
        {
            var fixture = new Fixture();

            var sut = fixture.Create<QueryStringHelper>();

            Assert.Throws<ArgumentNullException>(() => sut.Add(string.Empty, "value"));
        }

        [Fact]
        public void Add_ValueNullOrEmptyString_Throws()
        {
            var fixture = new Fixture();

            var sut = fixture.Create<QueryStringHelper>();

            Assert.Throws<ArgumentNullException>(() => sut.Add("key", "   "));
        }

        [Fact]
        public void Add_CanAdd_Equal()
        {
            var fixture = new Fixture();

            var sut = fixture.Create<QueryStringHelper>();
            sut.Add("key", "value");

            var result = sut.ToString();

            Assert.Equal("?key=value", result);
        }
    }
}