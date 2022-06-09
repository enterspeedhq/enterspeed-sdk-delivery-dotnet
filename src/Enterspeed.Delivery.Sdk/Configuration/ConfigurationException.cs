using System;
namespace Enterspeed.Delivery.Sdk.Configuration
{
    public class ConfigurationException : Exception
    {
        private ConfigurationException()
        {
        }

        public ConfigurationException(string parameterName)
            : base($"Missing {parameterName}")
        {
        }
    }
}
