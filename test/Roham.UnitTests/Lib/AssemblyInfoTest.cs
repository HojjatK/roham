using NUnit.Framework;
using System.Reflection;

namespace Roham.Lib
{
    public class AssemblyInfoTest
    {
        [TestFixture]
        [Category("UnitTests.AssemblyInfo")]
        internal class GivenAssemblyInfo : UnitTestFixture
        {
            [Test]
            public void TestAssembyProperites()
            {
                // setup
                var info = new AssemblyInfo(Assembly.GetExecutingAssembly());

                // assert
                Assert.AreEqual("Roham.UnitTests", info.Title);
                Assert.AreEqual("Roham UnitTests Assembly", info.Description);
                Assert.AreEqual("Roham UnitTests Product", info.Product);
                Assert.AreEqual("Copyright ©  2016", info.Copyright);
                Assert.AreEqual("Roham Company", info.Company);
                Assert.AreEqual("1.0.0.0", info.Version);
            }
        }
    }
}
