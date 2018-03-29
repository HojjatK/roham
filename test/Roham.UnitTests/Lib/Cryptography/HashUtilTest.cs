using NUnit.Framework;
using System.Collections.Generic;

namespace Roham.Lib.Cryptography
{
    [TestFixture]
    [Category("UnitTests.Cryptography.HashUtil")]
    public class HashUtilTest : UnitTestFixture
    {
        private Dictionary<string, string> testPBKDF2Data = new Dictionary<string, string> {
            { "1234567", "1000:c6l3DDbBx9mjYVbM1VkPKwZwgfnKeDwa:meRzOnLIc4kfIv0dYpy1rJPR6zVv1xoQ"},
            { "test", "1000:Ze1dXJ5GQqR6oUT3lUmKs8XAj+AcpCDe:hl+73xfvNtzjnB0IrsHmUFBNacKPvMa5"},
            { "p@ssw0rd", "1000:xhWPZ/lqh5DxHJLC9da1BeuB+nHv4K2s:cAMiuOq4b2Tg1JGq9LwgKWfAhQ0OnBmB"},
            { "awesome$123%", "1000:MGz7BmaHgEDne+cJpX5p23kAGsWjP5dE:bTSa3LJuby3Mm5ZDPzaymvAlphlQrG2+"}
        };

        [Test]
        public void WhenCorrectPasswordProvided_ThenMD5HashIsCorrect()
        {
            const string password = "p@$$w0rd";
            const string expectedHash = "B7463760284FD06773AC2A48E29B0ACF";

            Assert.AreEqual(expectedHash, HashUtil.Hash(HashAlgorithm.MD5, password).ToUpperInvariant());
        }

        [Test]
        public void WhenCorrectPasswordProvided_ThenSHA1HashIsCorrect()
        {
            const string password = "p@$$w0rd";
            const string expectedHash = "F2F7D2A4E7C1D1FCC4EF7E7968586C99AADE8B5B";

            Assert.AreEqual(expectedHash, HashUtil.Hash(HashAlgorithm.SHA1, password).ToUpperInvariant());
        }

        [Test]
        public void WhenCorrectPasswordProvided_ThenSHA256HashIsCorrect()
        {
            const string password = "p@$$w0rd";
            const string expectedHash = "18F3C96386407BA486F6F6178A14639194E498C4F8338FC61BF2945653FE058A";

            Assert.AreEqual(expectedHash, HashUtil.Hash(HashAlgorithm.SHA256, password).ToUpperInvariant());
        }
        

        [Test]
        public void WhenCorrectPasswordProvided_ThenValidatePBKDF2Succeeds()
        {
            foreach (var entry in testPBKDF2Data)
            {
                Assert.IsTrue(HashUtil.ValidateHash(HashAlgorithm.PBKDF2, entry.Key, entry.Value));
            }
        }

        [Test]
        public void WhenInCorrectPasswordProvided_ThenValidatePBKDF2Fails()
        {
            foreach (var entry in testPBKDF2Data)
            {
                string incorrectPassword = entry.Key + "2";
                Assert.IsFalse(HashUtil.ValidateHash(HashAlgorithm.PBKDF2, incorrectPassword, entry.Value));
            }
        }
    }
}
