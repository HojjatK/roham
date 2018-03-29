using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Roham.SmokeTests.Caching
{
    public abstract partial class GivenCacheService : CacheServiceFixtureBase
    {
        [Test]
        public void WhenObjectWithSlidingExpirationIsCached_ThenItWillExpireAfterTimespan()
        {
            var testCustomer = CreateTestCustomer(1);
            var key = testCustomer.CacheKey;

            var testCustomers = new HashSet<Customer> { CreateTestCustomer(2), CreateTestCustomer(3) };
            var hashKey = $"customers:{Guid.NewGuid()}";

            Subject.Set(testCustomer, TimeSpan.FromMilliseconds(200));
            Subject.SetHash(hashKey, testCustomers, TimeSpan.FromMilliseconds(200));

            Assert.IsNotNull(Subject.Get<Customer>(key, null));
            Assert.IsNotNull(Subject.GetHash<Customer>(hashKey, null));

            Thread.Sleep(250);

            Assert.IsNull(Subject.Get<Customer>(key, null));
            Assert.IsNull(Subject.GetHash<Customer>(hashKey, null));
        }

        [Test]
        public void WhenObjectWithAbsoluteExpirationIsCached_ThenItWillExpireAfterTimespan()
        {
            var testCustomer = CreateTestCustomer(10);
            var key = testCustomer.CacheKey;

            var testCustomers = new HashSet<Customer> { CreateTestCustomer(20), CreateTestCustomer(30) };
            var hashKey = $"customers:{Guid.NewGuid()}";

            var now = DateTime.Now;
            Subject.Set(testCustomer, now.AddMilliseconds(200));
            Subject.SetHash(hashKey, testCustomers, TimeSpan.FromMilliseconds(200));

            Assert.IsNotNull(Subject.Get<Customer>(key, null));
            Assert.IsNotNull(Subject.GetHash<Customer>(hashKey, null));

            Thread.Sleep(250);

            Assert.IsNull(Subject.Get<Customer>(key, null));
            Assert.IsNull(Subject.GetHash<Customer>(hashKey, null));
        }

        [Test]
        public void WhenCollectExpiredKeysIsCalled_ItCollectsExpiredHashKeys()
        {
            // need a new subject                
            var freshSubject = SubjectFactory();

            string[] hashKeys = new string[5];
            for (int i = 0; i < hashKeys.Length; i++)
            {
                var testCustomers = new HashSet<Customer> { CreateTestCustomer(i), CreateTestCustomer(i + 10) };
                hashKeys[i] = $"customers:{Guid.NewGuid()}";

                freshSubject.SetHash(hashKeys[i], testCustomers, TimeSpan.FromMilliseconds(200));
            }
            Assert.AreEqual(5, freshSubject.CachedSetsCount);

            Thread.Sleep(250);
            Assert.AreEqual(5, freshSubject.CachedSetsCount);

            freshSubject.CollectExpiredKeys();
            Assert.AreEqual(0, freshSubject.CachedSetsCount);
        }
    }
}
