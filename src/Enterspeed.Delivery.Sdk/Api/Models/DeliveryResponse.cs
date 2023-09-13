using System.Collections.Generic;

namespace Enterspeed.Delivery.Sdk.Api.Models
{
    public class DeliveryResponse<T>
    {
        public Meta Meta { get; set; }

        public Dictionary<string, T> Route { get; set; }

        public Dictionary<string, T> Views { get; set; }
    }

    public class DeliveryResponse
    {
        public Meta Meta { get; set; }

        public Dictionary<string, object> Route { get; set; }

        public Dictionary<string, object> Views { get; set; }
    }
}