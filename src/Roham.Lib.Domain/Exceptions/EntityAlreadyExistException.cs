using System;
using System.Runtime.Serialization;

namespace Roham.Lib.Domain.Exceptions
{   
    [Serializable]
    public class EntityAlreadyExistException : Exception
    {
        public EntityAlreadyExistException() { }
        public EntityAlreadyExistException(string messae) : base(messae) { }
        public EntityAlreadyExistException(string message, Exception inner) : base(message, inner) { }
        protected EntityAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
