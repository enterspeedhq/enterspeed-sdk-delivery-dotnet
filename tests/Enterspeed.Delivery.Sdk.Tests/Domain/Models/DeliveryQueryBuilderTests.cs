using System;
using System.Web;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Domain.Exceptions;
using Xunit;

namespace Enterspeed.Delivery.Sdk.Tests.Domain.Models
{
    public class DeliveryQueryBuilderTests
    {
        public class DeliveryQueryBuilderTestFixture : Fixture
        {
            public DeliveryQueryBuilderTestFixture()
            {
                Customize(new AutoNSubstituteCustomization());
            }
        }

        [Fact]
        public void Ids_CanAddId_Contains()
        {
            var sut = new DeliveryQueryBuilder();

            var id1 = "id1";
            var id2 = "id2";

            sut.WithId(id1);
            sut.WithId(id2);

            var query = sut.Build();
            var uri = query.GetUri(new Uri("https://example.com"), "/v1");

            Assert.Contains("id=id1", uri.AbsoluteUri);
            Assert.Contains("id=id2", uri.AbsoluteUri);
        }

        [Fact]
        public void Handles_CanAddHandle_Contains()
        {
            var sut = new DeliveryQueryBuilder();

            var handle1 = "handle1";
            var handle2 = "handle2";

            sut.WithHandle(handle1);
            sut.WithHandle(handle2);

            var query = sut.Build();
            var uri = query.GetUri(new Uri("https://example.com"), "/v1");

            Assert.Contains("handle=handle1", uri.AbsoluteUri);
            Assert.Contains("handle=handle2", uri.AbsoluteUri);
        }

        [Fact]
        public void Url_CanAddUrl_Contains()
        {
            var sut = new DeliveryQueryBuilder();

            var url = "/test/";

            sut.WithUrl(url);

            var query = sut.Build();
            var uri = query.GetUri(new Uri("https://example.com"), "/v1");

            Assert.Contains("url=/test/", HttpUtility.UrlDecode(uri.AbsoluteUri));
        }

        [Fact]
        public void Url_AddsMoreThanOne_Throws()
        {
            var sut = new DeliveryQueryBuilder();

            var url = "/test/";

            sut.WithUrl(url);
            Assert.Throws<EnterspeedDeliveryException>(() => sut.WithUrl("/test2/"));
        }

        [Fact]
        public void Url_CanAddSameUrl()
        {
            var sut = new DeliveryQueryBuilder();

            var url = "/test/";

            sut.WithUrl(url);
            sut.WithUrl(url);

            var query = sut.Build();

            Assert.Equal(url, query.Url);
        }

        [Fact]
        public void Url_Handle_Id_contains()
        {
            var sut = new DeliveryQueryBuilder();

            var url = "/test/";

            sut.WithUrl(url);
            sut.WithHandle("handle1");
            sut.WithId("id1");

            var query = sut.Build();
            var uri = query.GetUri(new Uri("https://example.com"), "/v1");

            Assert.Contains(url, HttpUtility.UrlDecode(uri.AbsoluteUri));
            Assert.Contains("handle=handle1", uri.AbsoluteUri);
            Assert.Contains("id=id1", uri.AbsoluteUri);
        }
    }
}