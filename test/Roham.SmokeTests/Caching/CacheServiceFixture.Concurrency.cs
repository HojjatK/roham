using System;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Roham.SmokeTests.Caching
{
    public abstract partial class GivenCacheService : CacheServiceFixtureBase
    {
        [Test]
        public void WhenCacheObjectIsCalledConcurrently_ThenItWorksAsExpected()
        {
            var testCutomer = CreateTestCustomer(10);
            Parallel.For(0, 1000, i =>
            {
                Subject.Set(testCutomer, TimeSpan.FromMilliseconds(100));

                Thread.Sleep(1);
                var cachedCustomer = Subject.Get(testCutomer.CacheKey, () => testCutomer);
                AssertAreEqual(testCutomer, cachedCustomer);

                if (i % 3 == 0)
                {
                    Subject.Remove(testCutomer.CacheKey.Key);
                }
            });
        }

        [Test]
        public void WhenCacheHashIsCalledConcurrently_ThenItWorksAsExpected()
        {
            var testCustomers = new HashSet<Customer>();
            for (int i = 0; i < 5; i++)
            {
                testCustomers.Add(CreateTestCustomer(i));
            }
            var hashKey = $"customers:{Guid.NewGuid()}";

            Parallel.For(0, 1000, i =>
            {
                Subject.SetHash(hashKey, testCustomers, TimeSpan.FromMilliseconds(100));

                Thread.Sleep(1);
                var cachedCustomers = Subject.GetHash(hashKey, () => testCustomers);
                Assert.AreEqual(testCustomers.Count, cachedCustomers.Count);

                if (i % 3 == 0)
                {
                    Subject.Remove(hashKey);
                }
            });
        }
    }
}
