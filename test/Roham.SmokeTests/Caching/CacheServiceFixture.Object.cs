using System;
using System.Collections.Generic;
using NUnit.Framework;
using Roham.Lib.Caches;
using Roham.Domain.Services;
using Roham.Lib.Domain.Cache;

namespace Roham.SmokeTests.Caching
{   
    public abstract partial class GivenCacheService : CacheServiceFixtureBase
    {
        private ICacheService _subject;
        protected ICacheService Subject
        {
            get
            {
                if (_subject == null)
                {
                    _subject = SubjectFactory();
                }
                return _subject;
            }
        }

        [Test]
        public void WhenGetObjectIsCalled_ThenCachedObjectReturned()
        {
            // setup test data
            var testCompany = CreateTestCompnay(1);
            var testCustomers = new List<Customer>();
            for (int i = 0; i < 500; i++)
            {
                testCustomers.Add(CreateTestCustomer(i));
            }
            var testEmployee = CreateTestEmployee(1, testCompany);
            testEmployee.Customers = testCustomers;

            // if key is not in cache, then null is returned
            var testKey = CacheKey.New<ICacheable, string>("Uid", Guid.NewGuid().ToString());
            var result = Subject.Get<ICacheable>(testKey, null);
            Assert.IsNull(result);

            // get company object from cache
            Subject.Set(testCompany);
            var resultCompany = Subject.Get<Company>(testCompany.CacheKey, null);
            Assert.NotNull(resultCompany);
            AssertAreEqual(testCompany, resultCompany);

            // get customer object from cache
            foreach (var testCustomer in testCustomers)
            {
                Subject.Set(testCustomer);

                var resultCustomer = Subject.Get<Customer>(testCustomer.CacheKey, null);
                Assert.NotNull(resultCustomer);
                AssertAreEqual(testCustomer, resultCustomer);
            }

            // get employee object from cache
            Subject.Set(testEmployee);
            var resultEmployee = Subject.Get<Employee>(testEmployee.CacheKey, null);
            Assert.NotNull(resultEmployee);
            AssertAreEqual(testEmployee, resultEmployee);
        }

        [Test]
        public void WhenSetObjectIsCalled_ThenObjectIsCached()
        {
            var testKey = Guid.NewGuid().ToString();
            var testValue = new CacheObject<string>(testKey, "Test 1");
            Subject.Set(testValue);

            var cachedValue = Subject.Get<CacheObject<string>>(testValue.CacheKey, null);
            Assert.AreEqual("Test 1", cachedValue.Object);

            testValue = new CacheObject<string>(testKey, "Test 2");
            Subject.Set(testValue);

            cachedValue = Subject.Get<CacheObject<string>>(testValue.CacheKey, null);
            Assert.AreEqual("Test 2", Subject.Get<CacheObject<string>>(testValue.CacheKey, null).Object);

            var testCustomer = CreateTestCustomer(1);
            Subject.Set(testCustomer);

            var cachedCustomer = Subject.Get<Customer>(testCustomer.CacheKey, null);
            AssertAreEqual(testCustomer, cachedCustomer);

        }

        [Test]
        public void WhenRemoveObjectIsCalled_ThenObjectIsRemovedFromCache()
        {
            var testKey = Guid.NewGuid().ToString();
            Subject.Remove(testKey);

            var testEmployee = CreateTestEmployee(1, CreateTestCompnay(1));
            Subject.Set(testEmployee);
            var cachedEmployee = Subject.Get<Employee>(testEmployee.CacheKey, null);
            AssertAreEqual(testEmployee, cachedEmployee);

            Subject.Remove(testEmployee.CacheKey.Key);

            cachedEmployee = Subject.Get<Employee>(testEmployee.CacheKey, null);
            Assert.IsNull(cachedEmployee);
        }

        [Test]
        public void WhenGetObjectByRefIsCalled_ThenCacheObjectReturned()
        {
            var testEmployee = CreateTestEmployee(1, CreateTestCompnay(1));
            testEmployee.UserName = "emp1.username";

            var indexKey = CacheKey.NewIndex<Employee, string>(nameof(Employee.UserName), testEmployee.UserName);
            var e1 = Subject.Get<Employee>(indexKey, () => testEmployee);
            AssertAreEqual(testEmployee, e1);

            e1 = Subject.Get<Employee>(indexKey, null);
            AssertAreEqual(testEmployee, e1);
            
            var ee1 = Subject.Get<Employee>(testEmployee.CacheKey, null);
            AssertAreEqual(testEmployee, ee1);

            testEmployee.Name = "Modified " + testEmployee.Name;
            Subject.Set(testEmployee);

            e1 = Subject.Get<Employee>(indexKey, null);
            AssertAreEqual(testEmployee, e1);
            
            Subject.Remove(testEmployee.CacheKey.Key);
            var indexKey2 = CacheKey.NewIndex<Employee, string>(nameof(Employee.UserName), testEmployee.UserName);
            ee1 = Subject.Get<Employee>(indexKey2, null);
            Assert.IsNull(ee1);

            testEmployee.Name = "new employee name";
            e1 = Subject.Get<Employee>(indexKey2, () => testEmployee);
            AssertAreEqual(testEmployee, e1);

            ee1 = Subject.Get<Employee>(testEmployee.CacheKey, null);
            AssertAreEqual(testEmployee, ee1);

            e1 = Subject.Get<Employee>(indexKey, null);
            AssertAreEqual(testEmployee, e1);
        }
    }
}
