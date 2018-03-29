using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Settings
{
    public interface ISettingsProvider
    {
        T GetSettings<T>(long? siteId) where T : ISettings;
        T GetDefaultSettings<T>(long? siteId) where T : ISettings;
        void SaveSettings<T>(T settings) where T : ISettings;
        void SaveSettings<T>(IPersistenceUnitOfWork uow, T settings) where T : ISettings;
    }

    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class SettingsProvider : ISettingsProvider
    {
        private readonly object _lock = new object();
        private readonly IPersistenceUnitOfWorkFactory _uowFactory;
        private readonly Dictionary<KeyValuePair<Type, long?>, ISettings> _settingsStore = new Dictionary<KeyValuePair<Type, long?>, ISettings>();

        public SettingsProvider(IPersistenceUnitOfWorkFactory uowFactory)
        {
            Objects.Requires(uowFactory != null, () => new NullReferenceException(nameof(IPersistenceUnitOfWorkFactory)));
            _uowFactory = uowFactory;
        }

        public T GetDefaultSettings<T>(long? siteId) where T : ISettings
        {
            var settings = Activator.CreateInstance<T>();
            settings.SiteId = siteId;

            var settingMetadata = ReadSettingMetadata<T>();
            foreach (var setting in settingMetadata)
            {
                // Initialize with default values
                setting.Write(settings, setting.DefaultValue);
            }

            return settings;
        }

        public T GetSettings<T>(long? siteId) where T : ISettings
        {
            var key = new KeyValuePair<Type, long?>(typeof(T), siteId);
            if (!_settingsStore.ContainsKey(key))
            {
                lock (_lock)
                {
                    if (!_settingsStore.ContainsKey(key))
                    {
                        LoadSettings<T>(siteId);
                    }
                }
            }

            return (T)_settingsStore[key];
        }

        public void SaveSettings<T>(T settingsToSave) where T : ISettings
        {   
            using (var uow = _uowFactory.CreateWithTransaction(IsolationLevel.ReadCommitted))
            {
                SaveSettings(uow, settingsToSave);
                uow.Complete();
            }
        }

        public void SaveSettings<T>(IPersistenceUnitOfWork uow, T settingsToSave) where T : ISettings
        {
            var settingsMetadata = ReadSettingMetadata<T>();
            var databaseSettings = GetDatabaseSettings(uow, settingsToSave.SiteId);
            Site site = null;
            if (settingsToSave.SiteId != null)
            {
                site = uow.Context.FindById<Site>(settingsToSave.SiteId.Value);
            }

            foreach (var setting in settingsMetadata)
            {
                // Write over it using the stored value
                var value = setting.Read(settingsToSave);
                var dbSetting = databaseSettings.FirstOrDefault(x => x.Name == setting.Storage.Key);
                if (dbSetting != null)
                {
                    dbSetting.Value = value ?? setting.DefaultValue as string ?? string.Empty;
                    uow.Context.Update(dbSetting);
                }
                else
                {
                    var newSettings = new Setting
                    {
                        Section = "",
                        Name = setting.Storage.Key,
                        Title = setting.DisplayName,
                        Description = setting.Description,
                        Value = value ?? setting.DefaultValue as string ?? string.Empty,
                        Site = site
                    };
                    databaseSettings.Add(newSettings);
                    uow.Context.Add(newSettings);
                }
            }

            var key = new KeyValuePair<Type, long?>(typeof(T), settingsToSave.SiteId);
            if (_settingsStore.ContainsKey(key))
            {
                _settingsStore[key] = settingsToSave;
            }
            else
            {
                _settingsStore.Add(key, settingsToSave);
            }
        }

        private void LoadSettings<T>(long? siteId) where T : ISettings
        {
            var key = new KeyValuePair<Type, long?>(typeof(T), siteId);
            var settings = Activator.CreateInstance<T>();
            _settingsStore.Add(key, settings);
            var settingMetadata = ReadSettingMetadata<T>();
            using (var uow = _uowFactory.Create())
            {
                var databaseSettings = GetDatabaseSettings(uow, siteId);
                foreach (var setting in settingMetadata)
                {
                    // Initialize with default values
                    setting.Write(settings, setting.DefaultValue);
                    // Write over it using the stored value
                    var dbSetting = databaseSettings.FirstOrDefault(x => x.Name == setting.Storage.Key);
                    if (dbSetting != null)
                    {
                        setting.Write(settings, dbSetting.Value);
                    }
                }

                uow.Complete();
            }
        }

        private List<Setting> GetDatabaseSettings(IPersistenceUnitOfWork uow, long? siteId)
        {
            if (siteId == null)
            {
                return uow.Context.Query<Setting>().Where(s => s.Site == null).ToList();
            }
            else
            {
                long siteIdValue = siteId.Value;
                return uow.Context.Query<Setting>().Where(s => s.Site.Id == siteIdValue).ToList();
            }
        }

        private static IEnumerable<SettingDescriptor> ReadSettingMetadata<T>()
        {
            return typeof(T).GetProperties()
                .Where(x => x.GetCustomAttributes(true).OfType<SettingKeyAttribute>().Any())
                .Select(x => new SettingDescriptor(x))
                .ToArray();
        }

        #region Nested Classes

        private class SettingDescriptor
        {
            private readonly PropertyInfo property;
            private object defaultValue;
            private string description;
            private string displayName;
            private SettingKeyAttribute storage;

            public SettingDescriptor(PropertyInfo property)
            {
                this.property = property;
                displayName = property.Name;

                ReadAttribute<DefaultValueAttribute>(d => defaultValue = d.Value);
                ReadAttribute<DescriptionAttribute>(d => description = d.Description);
                ReadAttribute<DisplayNameAttribute>(d => displayName = d.DisplayName);
                ReadAttribute<SettingKeyAttribute>(d => storage = d);
            }

            private void ReadAttribute<TAttribute>(Action<TAttribute> callback)
            {
                var instances = property.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>();
                foreach (var instance in instances)
                {
                    callback(instance);
                }
            }

            public object DefaultValue => defaultValue;
            public string Description => description;
            public string DisplayName => displayName;
            public SettingKeyAttribute Storage => storage; 

            public void Write(ISettings settings, object value)
            {
                if (value != null)
                {
                    var converted = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(settings, converted, null);
                }
            }

            public string Read(ISettings settings)
            {
                return (property.GetValue(settings, null) ?? string.Empty).ToString();
            }
        }

        #endregion
    }
}
