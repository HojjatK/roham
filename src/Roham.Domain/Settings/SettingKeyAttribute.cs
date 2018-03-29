using System;

namespace Roham.Domain.Settings
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingKeyAttribute : Attribute
    {
        public SettingKeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}
