using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Roham.Lib.Caches;
using Roham.Domain.Services;
using Roham.Lib.Domain.Cache;

namespace Roham.SmokeTests.Caching
{
    public abstract class CacheServiceFixtureBase
    {   
        protected Func<ICacheService> SubjectFactory { get; set; }

        protected Company CreateTestCompnay(int id)
        {
            return new Company
            {
                Id = id,
                Uid = Guid.NewGuid(),
                Name = $"Test Company {id}",
                Children = new List<Company>
                {
                    new Company { Id = id + 1, Uid = Guid.NewGuid(), Name = $"Test Company {id + 1}" },
                    new Company { Id = id + 2, Uid = Guid.NewGuid(), Name = $"Test Company {id + 2}",
                                 Children = new List<Company> { new Company { Id = id + 3, Uid = Guid.NewGuid(), Name = $"Test Company {id + 3}" } } },
                    new Company { Id = id + 4, Uid = Guid.NewGuid(), Name = $"Test Company {id + 4}" },
                }
            };
        }

        protected Employee CreateTestEmployee(int id, Company company)
        {
            return new Employee
            {
                Id = id,
                Uid = Guid.NewGuid(),
                Name = $"Employee {id}",
                Company = company,
                Customers = new List<Customer>()
            };
        }

        protected Customer CreateTestCustomer(int id)
        {
            var customer = new Customer
            {
                Id = id,
                Uid = Guid.NewGuid(),
                Name = $"Test Customer {id}"
            };
            var orders = new List<CustomerOrder>
            {
                new CustomerOrder { Id = id, Amount = 1000m, OrderDate = DateTime.Now.AddDays(-10), Customer = customer },
                new CustomerOrder { Id = id + 1, Amount = 400m, OrderDate = DateTime.Now.AddDays(-30), Customer = customer },
                new CustomerOrder { Id = id + 2, Amount = 2000m, OrderDate = DateTime.Now.AddDays(-60), Customer = customer }
            };
            customer.Orders = orders;
            return customer;
        }

        protected void AssertAreEqual(Company expected, Company actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Uid, actual.Uid);
            Assert.AreEqual(expected.Name, actual.Name);
            if (expected.Children == null)
            {
                Assert.IsNull(actual.Children);
            }
            else
            {
                Assert.AreEqual(expected.Children.Count, actual.Children.Count);
                if (expected.Children.Count > 0)
                {
                    var actualLookup = actual.Children.ToDictionary(i => i.Uid.ToString());
                    foreach (var child in expected.Children)
                    {
                        var uidStr = child.Uid.ToString();
                        if (!actualLookup.ContainsKey(uidStr))
                        {
                            Assert.Fail("Expected child not found");
                        }
                        AssertAreEqual(child, actualLookup[uidStr]);
                    }
                }
            }
        }

        protected void AssertAreEqual(Employee expected, Employee actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Uid, actual.Uid);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Salary, actual.Salary);
            AssertAreEqual(expected.Company, actual.Company);

            if (expected.Customers == null)
            {   
                Assert.IsNull(actual.Customers);
            }
            else
            {
                Assert.IsNotNull(actual.Customers);
                foreach(var expectedCustomer in expected.Customers)
                {
                    var actualCustomer = actual.Customers.FirstOrDefault(c => c.Uid == expectedCustomer.Uid);
                    AssertAreEqual(expectedCustomer, actualCustomer);
                }
            }
        }

        protected void AssertAreEqual(Customer expected, Customer actual, bool checkOrders = true)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Uid, actual.Uid);
            Assert.AreEqual(expected.Name, actual.Name);
            if (checkOrders)
            {
                if (expected.Orders == null)
                {
                    Assert.IsNull(actual.Orders);
                }
                else
                {
                    foreach (var expectedOrder in expected.Orders)
                    {
                        var actualOrder = actual.Orders.FirstOrDefault(o => o.Id == expectedOrder.Id && o.OrderDate == expectedOrder.OrderDate);
                        Assert.IsNotNull(actualOrder);
                        Assert.AreEqual(expectedOrder.Amount, actualOrder.Amount);
                        AssertAreEqual(expectedOrder.Customer, actualOrder.Customer, false);
                    }
                }
            }
        }

        #region Nested Classes
        protected abstract class BaseEntity
        {
            public int Id { get; set; }
            public Guid Uid { get; set; }
            public string Name { get; set; }
        }

        protected class Customer : BaseEntity, ICacheable
        {
            public CacheKey CacheKey => CacheKey.New<Customer, Guid>(nameof(Uid), Uid);
            public string Key => CacheKey?.Key;
            
            public List<CustomerOrder> Orders { get; set; }
        }

        protected class CustomerOrder
        {
            public int Id { get; set; }            
            public DateTime OrderDate { get; set; }
            public decimal Amount { get; set; }
            public Customer Customer { get; set; }
        }

        protected class Company : BaseEntity, ICacheable
        {
            public CacheKey CacheKey => CacheKey.New<Company, Guid>(nameof(Uid), Uid);
            public string Key => CacheKey?.Key;
                        
            public List<Company> Children { get; set; }
        }

        protected class Employee : BaseEntity, ICacheable
        {
            public CacheKey CacheKey => CacheKey.New<Employee, Guid>(nameof(Uid), Uid);
            public string Key => CacheKey?.Key;
            
            private decimal AmountPerDay { get; set; }
            private int Days { get; set; }

            public decimal Salary => AmountPerDay * Days;

            public string UserName { get; set; }

            public Company Company { get; set; }
            public List<Customer> Customers { get; set; }
        }        
        #endregion
    }
}
