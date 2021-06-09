using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Enterspeed.Delivery.Sdk.Domain.Services;
using Xunit;

namespace Enterspeed.Delivery.Sdk.Tests.Domain.Services
{
    public class EnterspeedDeliveryServiceTests
    {
        public class EnterspeedDeliveryServiceTestFixture : Fixture
        {
            public EnterspeedDeliveryServiceTestFixture()
            {
                Customize(new AutoNSubstituteCustomization());
            }
        }

        [Fact]
        public async Task ApiKey_Null_Throws()
        {
            var fixture = new EnterspeedDeliveryServiceTestFixture();

            var sut = fixture.Create<EnterspeedDeliveryService>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Fetch(null));
        }

        [Fact]
        public async Task ApiKey_EmptyString_Throws()
        {
            var fixture = new EnterspeedDeliveryServiceTestFixture();

            var sut = fixture.Create<EnterspeedDeliveryService>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Fetch("   "));
        }
    }
}