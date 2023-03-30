using System;
using System.Collections.Generic;
using Enterspeed.Delivery.Sdk.Domain.Exceptions;
using Enterspeed.Delivery.Sdk.Domain.Models;

namespace Enterspeed.Delivery.Sdk.Api.Models
{
    public class DeliveryQueryBuilder
    {
        internal DeliveryQueryBuilder()
        {
        }

        private readonly IList<string> _handles = new List<string>();
        private readonly IList<string> _ids = new List<string>();
        private string _url;
        public bool IsAbsoluteUrl { get; private set; }

        public DeliveryQueryBuilder WithHandle(string handle)
        {
            if (_handles.Contains(handle))
            {
                return this;
            }

            _handles.Add(handle);
            return this;
        }

        public DeliveryQueryBuilder WithId(string id)
        {
            if (_ids.Contains(id))
            {
                return this;
            }

            _ids.Add(id);
            return this;
        }

        public DeliveryQueryBuilder WithUrl(string url)
        {
            if (_url != null && url != _url)
            {
                throw new EnterspeedDeliveryException("Only one URL is allowed.");
            }

            _url = url;
            return this;
        }

        public DeliveryQueryBuilder Absolute()
        {
            IsAbsoluteUrl = true;
            return this;
        }

        internal DeliveryQuery Build()
        {
            return new DeliveryQuery(_url, _handles, _ids);
        }
    }
}