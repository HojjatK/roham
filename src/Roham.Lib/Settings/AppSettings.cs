using System;
using System.Configuration;

namespace Roham.Lib.Settings
{
    public static class AppSettings
    {
        public static bool HasValue(string key)
        {
            return (ConfigurationManager.AppSettings.Get(key) != null);
        }

        public static T GetValue<T>(string key)
        {
            var stringValue = ConfigurationManager.AppSettings.Get(key);

            return (T)Convert.ChangeType(stringValue, typeof(T));
        }

        public static T GetValue<T>(string key, T defaultValue)
        {
            var stringValue = ConfigurationManager.AppSettings.Get(key);

            if (stringValue == null)
            {
                return defaultValue;
            }

            if (typeof(Enum).IsAssignableFrom(typeof(T)))
            {
                return (T)Enum.Parse(typeof(T), stringValue);
            }
            return (T)Convert.ChangeType(stringValue, typeof(T));
        }

        public static T GetSection<T>(string sectionName)
        {
            object section = ConfigurationManager.GetSection(sectionName);
            if (section is T) return (T)section;
            return (T)Convert.ChangeType(section, typeof(T));
        }
    }
}