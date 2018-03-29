using System;
using System.Runtime.Serialization;

namespace Roham.Lib.Domain.Exceptions
{   
    [Serializable]
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException() { }
        public InvalidPasswordException(string messae) : base(messae) { }
        public InvalidPasswordException(string message, Exception inner) : base(message, inner) { }
        protected InvalidPasswordException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
