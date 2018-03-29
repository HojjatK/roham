using NUnit.Framework;
using Roham.Caching;
using Roham.Data;
using Roham.Domain.Configs;
using Roham.Domain.Services;
using Roham.Lib.Settings;

namespace Roham.SmokeTests.Caching
{   
    [TestFixture]
    [Category("SmokeTest.Redis")]
    public class RedisCacheServiceSmokeTests : GivenCacheService
    {
        protected readonly IRohamConfigs _rohamConfigs;
        protected readonly static ICacheProvider _cacheProvider = new CacheProvider();

        public bool IsTestEnabled => _rohamConfigs != null && _rohamConfigs.CacheProvider == CacheProviders.Redis
                                     ? AppSettings.GetValue<bool>("RedisCachingTestEnabled")
                                     : true;

        public RedisCacheServiceSmokeTests()
        {
            _rohamConfigs = new RedisConfigs();
            SubjectFactory = () => new CacheService(() => _rohamConfigs, () => _cacheProvider);
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            if (!IsTestEnabled)
            {
                Assert.Ignore($"Testing {_rohamConfigs.CacheProvider} CacheService is disabled.");
            }
            else
            {
                if (_rohamConfigs.CacheProvider == CacheProviders.Redis)
                {
                    string errorMessage = "";
                    if (!_cacheProvider.TryConnect(CacheProviders.Redis, _rohamConfigs.CacheConnectionString, out errorMessage))
                    {
                        errorMessage = errorMessage ?? "Redis server cannot be connected.";
                        errorMessage = $"{errorMessage} [ConnectionString: {_rohamConfigs.CacheConnectionString}]";
                        Assert.Ignore(errorMessage);
                    }
                }
            }
        }

        private class RedisConfigs : IRohamConfigs
        {
            public int AdoNetBatchSize => 200;

            public string CacheConnectionString => AppSettings.GetValue<string>("RedisCacheConnectionString");

            public CacheProviders CacheProvider => CacheProviders.Redis;

            public string ConnectionString => "";

            public DbProviders DatabaseProvider => DbProviders.None;

            public string InstallationPassword => "";

            public bool IsConfigFileMissing => false;

            public bool IsInstalled => true;

            public string AppName => "TestApp";

            public bool ShowSql => false;

            public bool SmtpEnabled => false;

            public int SmtpPort => 25;

            public string SmtpDomain => null;

            public bool SmtpEnableSsl => false;

            public string SmtpHost => "";

            public string SmtpPassword => null;

            public string SmtpUsername => null;

            public string SmtpFrom => null;
        }
    }
}
