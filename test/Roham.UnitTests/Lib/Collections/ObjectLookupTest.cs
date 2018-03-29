using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;

namespace Roham.Lib.Collections
{
    public class ObjectLookupTest 
    {
        [TestFixture]
        [Category("UnitTests.Collections.ObjectLookup")]
        public class GivenObjectLookup : UnitTestFixture<ObjectLookup>
        {
            public GivenObjectLookup() : base(() => new ObjectLookup())
            {
            }

            [Test]
            public void WhenGetOrAddCalledAndObjectNotInLookup_ThenItIsAdded()
            {
                var subject = CreateSubject();
                var result = subject.GetOrAdd("testkey", () => new TestItem { Value = "testvalue" });

                Assert.AreEqual("testvalue", result.Value);
            }

            [Test]
            public void WhenGetOrAddCalledAndObjectIsAlreadyInLookup_ThenItIsReturned()
            {
                var subject = CreateSubject();
                subject.GetOrAdd("testKey", () => new TestItem { Value = "testvalue" });

                var result = subject.GetOrAdd("testKey", () => new TestItem { Value = "testvalue2" });

                Assert.AreEqual("testvalue", result.Value);
            }

            [Test]
            public void WhenObjectsWithSameKeyAndDifferentTypesAreAdded_ThenObjectsAreAddedBasedOnTypes()
            {
                var subject = CreateSubject();

                subject.GetOrAdd("samekey", () => 121);
                subject.GetOrAdd("samekey", () => "string");
                subject.GetOrAdd("samekey", () => new TestItem { Value = "test item" });

                Assert.AreEqual(121, subject.GetOrAdd<int>("samekey", () => 0));
                Assert.AreEqual("string", subject.GetOrAdd<string>("samekey", () => ""));
                Assert.AreEqual("test item", subject.GetOrAdd<TestItem>("samekey", () => null).Value);
            }

            [Test]
            public void WhenObjectsWithSameTypeAndDifferentKeysAreAdded_ThenTheyAreAddedAsDifferentObjects()
            {
                var subject = CreateSubject();

                subject.GetOrAdd("key1", () => new TestItem { Value = "test item1" });
                subject.GetOrAdd("key2", () => new TestItem { Value = "test item2" });

                Assert.AreEqual("test item1", subject.GetOrAdd<TestItem>("key1", () => null).Value);
                Assert.AreEqual("test item2", subject.GetOrAdd<TestItem>("key2", () => null).Value);
            }

            [Test]
            public void WhenGetOrAddCalledAndObjectNotInLookup_ThenItIsAdded_MultiThreadScenario()
            {
                // setup
                var subject = CreateSubject();
                var cacheObject = Substitute.For<TestItem>();

                // action        
                Parallel.For(0, 1000,
                    i => subject.GetOrAdd("testkey", () => cacheObject.Init("test value")));
                var result = subject.GetOrAdd("testkey", () => new TestItem { Value = "test value2" });

                // assert
                Assert.AreEqual("test value", result.Value);
                cacheObject.Received(1).Init("test value");
            }
            
            [Test]
            public void WhenGetOrAddCalledAndObjectIsInLookup_ThenItIsReturned_MultiThreadScenario()
            {
                // setup
                var subject = CreateSubject();
                var cacheObject = Substitute.For<TestItem>();
                subject.GetOrAdd("testkey", () => new TestItem { Value = "test value" });

                // action
                Parallel.For(0, 50,
                    i => subject.GetOrAdd("testkey", () => cacheObject.Init("test.value2")));
                var result = subject.GetOrAdd("testkey", () => cacheObject.Init("test.value3"));

                // assert
                Assert.AreEqual("test value", result.Value);                
                cacheObject.DidNotReceive().Init("test.value2");
                cacheObject.DidNotReceive().Init("test.value3");
            }

        }

        public class TestItem
        {
            public string Value { get; set; }

            public TestItem Init(string value)
            {
                Value = value;
                return this;
            }
        }
    }
}
