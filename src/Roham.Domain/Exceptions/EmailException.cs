using System;

namespace Roham.Domain.Exceptions
{
    [Serializable]
    public class EmailException : RohamException
    {
        public EmailException(string message, string displayMessage) : base(message, displayMessage) { }
        public EmailException(string message, string displayMessage, Exception inner) : base(message, displayMessage, inner) { }
    }
}
