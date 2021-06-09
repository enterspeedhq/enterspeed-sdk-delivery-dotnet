using System;
using System.Collections.Generic;

namespace Enterspeed.Delivery.Sdk.Domain.Helpers
{
    internal class QueryStringHelper
    {
        private readonly IList<string> _keys = new List<string>();
        private readonly IList<string> _values = new List<string>();

        public override string ToString()
        {
            var output = string.Empty;

            for (var i = 0; i < _keys.Count; i++)
            {
                if (i == 0)
                {
                    output += $"?{_keys[i]}={_values[i]}";
                }
                else
                {
                    output += $"&{_keys[i]}={_values[i]}";
                }
            }

            return output;
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

            _keys.Add(key);
            _values.Add(value);
        }
    }
}