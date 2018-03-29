using NUnit.Framework;
using Roham.Caching;
using Roham.Data;
using Roham.Domain.Configs;
using Roham.Domain.Services;

namespace Roham.SmokeTests.Caching
{
    [TestFixture]
    [Category("SmokeTest.Memory")]
    public class MemCacheServiceSmokeTests : GivenCacheService
    {
        protected readonly IRohamConfigs _rohamConfigs;
        protected readonly static ICacheProvider _cacheProvider = new CacheProvider();

        public MemCacheServiceSmokeTests()
        {
            _rohamConfigs = new MemCacheConfigs();
            SubjectFactory = () => new CacheService(() => _rohamConfigs, () => _cacheProvider);
        }

        private class MemCacheConfigs : IRohamConfigs
        {
            public int AdoNetBatchSize => 200;

            public string CacheConnectionString => "";

            public CacheProviders CacheProvider => CacheProviders.Memory;

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

            public string SmtpHost => "localhost";

            public string SmtpPassword => null;

            public string SmtpUsername => null;

            public string SmtpFrom => null;
        }
    }
}
