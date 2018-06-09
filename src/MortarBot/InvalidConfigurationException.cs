using System;
using System.Runtime.Serialization;

namespace MortarBot
{
    [Serializable]
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException() { }
        public InvalidConfigurationException(string message) : base(message) { }
        public InvalidConfigurationException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
