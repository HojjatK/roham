using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Roham.Lib.Settings
{
    public class XmlFileSettings
    {
        private readonly string _settingsFilePath;
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(Settings));

        public XmlFileSettings(string settingsFilePath)
        {
            _settingsFilePath = settingsFilePath;
        }

        public bool IsSettingsFileMissing()
        {
            return !File.Exists(_settingsFilePath);
        }

        public string Get(string name)
        {
            return OpenSettings().Get(name);
        }

        public bool GetBool(string name, bool defaultValue)
        {
            bool result;
            if (!bool.TryParse(Get(name), out result))
            {
                return defaultValue;
            }
            return result;
        }

        public int GetInt(string name, int defaultValue)
        {
            int result;
            if (!int.TryParse(Get(name), out result))
            {
                return defaultValue;
            }
            return result;
        }

        public void Set(string name, string value)
        {
            var settings = OpenSettings();
            settings.Set(name, value);
            if (settings.HasChanged)
            {
                SaveSettings(settings);
            }
        }

        public void SetCollection(ICollection<KeyValuePair<string, string>> nameValues)
        {
            if (nameValues == null || nameValues.IsEmpty())
            {
                return;
            }

            var settings = OpenSettings();
            nameValues.ForEach(pair => settings.Set(pair.Key, pair.Value));
            if (settings.HasChanged)
            {
                SaveSettings(settings);
            }
        }

        private Settings OpenSettings()
        {
            if (IsSettingsFileMissing())
            {
                return new Settings();
            }
            using (var stream = new StreamReader(_settingsFilePath))
            {
                return (Settings)_serializer.Deserialize(stream);
            }
        }

        private void SaveSettings(Settings config)
        {
            using (var writer = new StreamWriter(_settingsFilePath))
            {
                _serializer.Serialize(writer, config);
                writer.Flush();
            }
        }

        [XmlRoot("Settings")]
        public class Settings
        {
            public Settings() { }

            [XmlIgnore]
            public bool HasChanged { get; private set; }

            [XmlElement("Setting")]
            public List<XmlSetting> SettingsList { get; set; }

            public void Set(string name, string value)
            {
                var existing = SettingsList.FirstOrDefault(x => x.Key == name);
                if (existing == null)
                {
                    existing = new XmlSetting { Key = name };
                    SettingsList.Add(existing);
                }

                if (existing.Value == value)
                {
                    return;
                }

                existing.Value = value;                
                HasChanged = true;
            }

            public string Get(string name)
            {
                if (SettingsList == null)
                {
                    return null;
                }

                var existing = SettingsList.FirstOrDefault(x => x.Key == name);
                if (existing == null)
                {
                    return null;
                }
                return existing.Value;
            }
        }

        public class XmlSetting
        {
            [XmlAttribute("key")]
            public string Key { get; set; }

            [XmlAttribute("value")]
            public string Value { get; set; }
        }
    }
}
