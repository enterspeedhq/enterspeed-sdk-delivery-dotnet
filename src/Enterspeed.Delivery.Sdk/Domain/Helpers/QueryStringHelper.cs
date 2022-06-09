using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enterspeed.Delivery.Sdk.Domain.Helpers
{
     internal class QueryStringHelper
    {
        private readonly List<KeyValuePair<string, string>> _parameters = new List<KeyValuePair<string, string>>();

        public override string ToString()
        {
            var keyValues = _parameters.Select(
                kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}");
            return string.Concat("?", string.Join("&", keyValues));
        }

        public void Add(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be an empty string or null");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be an empty string or null");
            }

            _parameters.Add(new KeyValuePair<string, string>(key, value));
        }
    }
}
