using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Delivery.Sdk.Domain.Helpers;

namespace Enterspeed.Delivery.Sdk.Domain.Models
{
    internal class DeliveryQuery
    {
        internal DeliveryQuery(string url, IList<string> handles, IList<string> ids)
        {
            Url = url;
            Handles = handles ?? new List<string>();
            Ids = ids ?? new List<string>();
        }

        public string Url { get; set; }
        public IList<string> Handles { get; set; }
        public IList<string> Ids { get; set; }

        public Uri GetUri(Uri current, string path)
        {
            var uriBuilder = new UriBuilder(new Uri(current, path));

            var query = new QueryStringHelper();

            if (!string.IsNullOrEmpty(Url))
            {
                query.Add("url", Url);
            }

            if (Ids.Any())
            {
                foreach (var id in Ids)
                {
                    query.Add("id", id);
                }
            }

            if (Handles.Any())
            {
                foreach (var handle in Handles)
                {
                    query.Add("handle", handle);
                }
            }

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }
    }
}