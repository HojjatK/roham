using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Roham.SmokeTests.Caching
{
    public abstract partial class GivenCacheService : CacheServiceFixtureBase
    {
        [Test]
        public void WhenGetHashIsCalled_ThenCachedHashReturned()
        {
            // setup test data
            var testCompany = CreateTestCompnay(1);
            var testCustomers = new HashSet<Customer>();
            for (int i = 0; i < 300; i++)
            {
                testCustomers.Add(CreateTestCustomer(i));
            }
            string key = $"customers:{Guid.NewGuid()}";
            var cachedCustomers = Subject.GetHash<Customer>(key, null);
            Assert.IsNull(cachedCustomers);

            Subject.SetHash(key, testCustomers);
            cachedCustomers = Subject.GetHash<Customer>(key, null);
            Assert.NotNull(cachedCustomers);
            foreach (var expectedCustomer in testCustomers)
            {
                var cachedCustomer = cachedCustomers.FirstOrDefault(c => c.Uid == expectedCustomer.Uid);
                Assert.NotNull(cachedCustomer);
                AssertAreEqual(expectedCustomer, cachedCustomer);
            }

            testCustomers.Add(CreateTestCustomer(1000)); // add a new customer
            Subject.SetHash(key, testCustomers);
            cachedCustomers = Subject.GetHash<Customer>(key, null);
            Assert.NotNull(cachedCustomers);
            foreach (var expectedCustomer in testCustomers)
            {
                var cachedCustomer = cachedCustomers.FirstOrDefault(c => c.Uid == expectedCustomer.Uid);
                Assert.NotNull(cachedCustomer);
                AssertAreEqual(expectedCustomer, cachedCustomer);
            }
        }

        [Test]
        public void WhenSetHashIsNull_ThenNullReferenceIsThrown()
        {
            var testKey = Guid.NewGuid().ToString();            
            Assert.Throws<NullReferenceException>(() => Subject.SetHash<Customer>(testKey, null));
        }

        [Test]
        public void WhenSetHashIsCalled_ThenHashIsCached()
        {
            // setup test data
            var testKey = $"customers:{Guid.NewGuid()}";
            var testCompany = CreateTestCompnay(1);
            var testCustomers = new HashSet<Customer>();
            for (int i = 20; i < 40; i++)
            {
                testCustomers.Add(CreateTestCustomer(i));
            }

            Subject.SetHash(testKey, testCustomers);
            var cachedCustomers = Subject.GetHash<Customer>(testKey, null);
            Assert.NotNull(cachedCustomers);
            foreach (var expectedCustomer in testCustomers)
            {
                var cachedCustomer = cachedCustomers.FirstOrDefault(c => c.Uid == expectedCustomer.Uid);
                Assert.NotNull(cachedCustomer);
                AssertAreEqual(expectedCustomer, cachedCustomer);
            }
        }

        [Test]
        public void WhenRemoveHashIsCalled_ThenHashIsRemovedFromCache()
        {
            // setup test data
            var testKey = $"customers:{Guid.NewGuid()}";
            var testCompany = CreateTestCompnay(1);
            var testCustomers = new HashSet<Customer>();
            for (int i = 20; i < 40; i++)
            {
                testCustomers.Add(CreateTestCustomer(i));
            }

            Subject.SetHash(testKey, testCustomers);
            var cachedCustomers = Subject.GetHash<Customer>(testKey, null);
            Assert.NotNull(cachedCustomers);

            Subject.Remove(testKey);
            cachedCustomers = Subject.GetHash<Customer>(testKey, null);
            Assert.IsNull(cachedCustomers);
        }

        [Test]
        public void WhenObjectIsRemoved_ThenRelatingHashSetsAreExpired()
        {
            // setup test data
            var testCompany = CreateTestCompnay(1);

            var testKey1 = $"customers:{Guid.NewGuid()}";
            var testKey2 = $"customers:{Guid.NewGuid()}";
            var testCustomers1 = new HashSet<Customer>();
            var testCustomers2 = new HashSet<Customer>();
            for (int i = 1; i <= 5; i++)
            {
                testCustomers1.Add(CreateTestCustomer(i));
                testCustomers2.Add(CreateTestCustomer(i + 10));
            }
            var customer6 = CreateTestCustomer(6);
            testCustomers1.Add(customer6);
            testCustomers2.Add(customer6);

            Subject.SetHash(testKey1, testCustomers1);
            var cachedCustomers = Subject.GetHash<Customer>(testKey1, null);
            Assert.NotNull(cachedCustomers);
            Assert.AreEqual(6, cachedCustomers.Count);

            Subject.SetHash(testKey2, testCustomers2);
            cachedCustomers = Subject.GetHash<Customer>(testKey2, null);
            Assert.NotNull(cachedCustomers);
            Assert.AreEqual(6, cachedCustomers.Count);

            Subject.Remove(customer6.CacheKey.Key);

            cachedCustomers = Subject.GetHash<Customer>(testKey1, null);
            Assert.IsNull(cachedCustomers);

            cachedCustomers = Subject.GetHash<Customer>(testKey2, null);
            Assert.IsNull(cachedCustomers);

            Subject.SetHash(testKey1, testCustomers2);
            cachedCustomers = Subject.GetHash<Customer>(testKey1, null);
            Assert.IsNotNull(cachedCustomers);
            Assert.AreEqual(6, cachedCustomers.Count);
        }
    }
}
