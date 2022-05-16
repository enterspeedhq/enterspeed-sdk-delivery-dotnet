using System;
using System.Collections.Generic;
using System.Text;
namespace Enterspeed.Delivery.Sdk.Domain.Helpers
{
    internal class QueryStringHelper
    {
        private readonly IList<string> _keys = new List<string>();
        private readonly IList<string> _values = new List<string>();

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < _keys.Count; i++)
            {
                builder.Append(i == 0 ? $"?{_keys[i]}={_values[i]}" : $"&{_keys[i]}={_values[i]}");
            }

            return builder.ToString();
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
