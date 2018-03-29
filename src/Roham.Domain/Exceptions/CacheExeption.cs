using System;

namespace Roham.Domain.Exceptions
{
    [Serializable]
    public class CacheExeption : RohamException
    {
        public CacheExeption(string message, string displayMessage) : base(message, displayMessage) { }
        public CacheExeption(string message, string displayMessage, Exception inner) : base(message, displayMessage, inner) { }
    }
}
