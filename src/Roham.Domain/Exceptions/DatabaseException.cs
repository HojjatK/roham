using System;

namespace Roham.Domain.Exceptions
{
    public class DatabaseException : RohamException
    {
        public DatabaseException(string message, string displayMessage) : base(message, displayMessage) { }
        public DatabaseException(string message, string displayMessage, Exception inner) : base(message, displayMessage, inner) { }
    }
}
