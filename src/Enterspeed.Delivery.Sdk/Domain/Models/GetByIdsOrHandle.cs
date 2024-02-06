using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Enterspeed.Delivery.Sdk.Domain.Models
{
    public class GetByIdsOrHandle
    {
        [JsonPropertyName("ids")]
        public List<string> Ids { get; set; }

        [JsonPropertyName("handles")]
        public List<string> Handles { get; set; }
    }
}
