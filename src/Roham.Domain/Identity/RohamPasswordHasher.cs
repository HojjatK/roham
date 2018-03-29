using Microsoft.AspNet.Identity;
using Roham.Lib.Cryptography;

namespace Roham.Domain.Identity
{
    public class RohamPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return HashUtil.Hash(HashAlgorithm.PBKDF2, password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return HashUtil.ValidateHash(HashAlgorithm.PBKDF2, providedPassword, hashedPassword) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }
}
