using System.Collections.Generic;

namespace Enterspeed.Delivery.Sdk.Api.Models
{
    public class DeliveryResponse
    {
        public Meta Meta { get; set; }

        public Dictionary<string, object> Route { get; set; }

        public Dictionary<string, object> Views { get; set; }
    }
}