using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
namespace IdentityService.Identity.Domain.Helper
{
    public class PasswordHasher
    {
        public static byte[] HashPassword(string password, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8,
                Iterations = 4,
                MemorySize = 65536
            };
            return argon2.GetBytes(32);
        }
        public static byte[] GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }
        public static bool VerifyPassword(string password, byte[] salt, byte[] expectedHash)
        {
            var hash = HashPassword(password, salt);
            return CryptographicOperations.FixedTimeEquals(hash, expectedHash);
        }
    }
}
