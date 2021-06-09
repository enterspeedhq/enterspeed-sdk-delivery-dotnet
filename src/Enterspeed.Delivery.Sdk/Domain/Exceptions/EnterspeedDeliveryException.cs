using System;

namespace Enterspeed.Delivery.Sdk.Domain.Exceptions
{
    public class EnterspeedDeliveryException : Exception
    {
        internal EnterspeedDeliveryException(string message)
            : base(message)
        {
        }
    }
}