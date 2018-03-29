using System;

namespace Roham.Lib.Domain.DataAnnotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : Attribute
    {
        public UniqueAttribute(string keyName = null)
        {
            KeyName = keyName;
        }

        public string KeyName { get; }
    }
}
