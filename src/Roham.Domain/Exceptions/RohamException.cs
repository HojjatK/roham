using System;
using System.Runtime.Serialization;

namespace Roham.Domain.Exceptions
{
    [Serializable]
    public abstract class RohamException : Exception
    {
        public RohamException(string message, string displayMessage, Exception inner) : base(message, inner)
        {
            DisplayMessage = displayMessage;
        }

        public RohamException(string message, string displayMessage) : base(message)
        {
            DisplayMessage = displayMessage;
        }        

        protected RohamException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string DisplayMessage { get; }
    }
}
