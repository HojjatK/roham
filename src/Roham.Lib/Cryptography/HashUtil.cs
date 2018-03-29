using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Roham.Lib.Cryptography
{
    public enum HashAlgorithm
    {
        MD5,
        SHA1,
        SHA256,
        PBKDF2
    }

    public static class HashUtil
    {
        public static string Hash(HashAlgorithm algorithm, string password)
        {
            switch (algorithm)
            {
                case HashAlgorithm.MD5:
                    return ToHexString(HashMD5(password));
                case HashAlgorithm.SHA1:
                    return ToHexString(HashSHA1(password));
                case HashAlgorithm.SHA256:
                    return ToHexString(HashSHA256(password));
                case HashAlgorithm.PBKDF2:
                    return HashPBKDF2(password);
                default:
                    throw new NotSupportedException($"{algorithm} hash algorithm is not supported");
            }
        }

        public static bool ValidateHash(HashAlgorithm algorithm, string password, string correctHash)
        {            
            switch (algorithm)
            {
                case HashAlgorithm.MD5:
                    return ToBase64(HashMD5(password)) == correctHash;
                case HashAlgorithm.SHA1:
                    return ToBase64(HashSHA1(password)) == correctHash;
                case HashAlgorithm.SHA256:
                    return ToBase64(HashSHA256(password)) == correctHash;
                case HashAlgorithm.PBKDF2:
                    return ValidatePBKDF2(password, correctHash);
                default:
                    throw new NotSupportedException($"{algorithm} hash algorithm is not supported");
            }
        }

        public static string ToBase64(byte[] hash)
        {
            return Convert.ToBase64String(hash);
        }

        public static string ToHexString(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "");
        }

        private static string HashPBKDF2(string password)
        {
            // PBKDF2: The following constants may be changed without breaking existing hashes.
            const int SaltByteSize = 24;
            const int HashByteSize = 24;
            const int PBKDF2Iterations = 1000;

            var salt = new byte[SaltByteSize];
            // generate a random salt
            using (var csprng = new RNGCryptoServiceProvider())
            {
                csprng.GetBytes(salt);
            }
            // Hash the password and encode the parameters
            byte[] hash = PBKDF2(password, salt, PBKDF2Iterations, HashByteSize);
            return PBKDF2Iterations + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        private static bool ValidatePBKDF2(string password, string correctHash)
        {
            const int IterationIndex = 0;
            const int SaltIndex = 1;
            const int PBKDF2Index = 2;

            // Extract the parameters from the hash
            var delimiter = new[] { ':' };
            var split = correctHash.Split(delimiter);
            var iterations = int.Parse(split[IterationIndex]);
            var salt = Convert.FromBase64String(split[SaltIndex]);
            var hash = Convert.FromBase64String(split[PBKDF2Index]);
            var testHash = PBKDF2(password, salt, iterations, hash.Length);

            return SlowEquals(hash, testHash);
        }

        private static byte[] HashMD5(string password)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(password);

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(inputBytes);

            return hash;
        }

        private static byte[] HashSHA1(string password)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(password);

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(inputBytes);

            return hash;
        }

        private static byte[] HashSHA256(string password)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(password);

            var sha256 = new SHA256Managed();
            byte[] hash = sha256.ComputeHash(inputBytes, 0, inputBytes.Length);
            return hash;
        }

        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
        }

        private static bool SlowEquals(IList<byte> a, IList<byte> b)
        {
            var diff = (uint)a.Count ^ (uint)b.Count;

            for (var i = 0; (i < a.Count) && (i < b.Count); i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }

            return diff == 0;
        }
    }
}
