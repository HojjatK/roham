using System;

namespace Roham.Domain.Exceptions
{
    [Serializable]
    public class ConfigurationException : RohamException
    {
        public ConfigurationException(string message, string displayMessage) : base(message, displayMessage) { }
        public ConfigurationException(string message, string displayMessage, Exception inner) : base(message, displayMessage, inner) { }
    }
}
