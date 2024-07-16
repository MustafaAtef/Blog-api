using BlogApi.ServiceContracts;
using System.Security.Cryptography;

namespace BlogApi.Services {
    public class PasswordHashingService : IPasswordHashingService {
        private readonly int SaltSize = 16;
        private readonly int KeySize = 32;
        private readonly int Iterations = 100000;

        private readonly HashAlgorithmName hashAlgorithmName = HashAlgorithmName.SHA256;

        private readonly string Delemiter= ";";


        public string Hash(string password) {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, hashAlgorithmName, KeySize);
            return string.Join(Delemiter, Convert.ToBase64String(salt), Convert.ToBase64String(hashedPassword));
        }

        public bool Compare(string hashedPassword, string InputPassword) {
            var saltAndHash = hashedPassword.Split(Delemiter);
            var salt = Convert.FromBase64String(saltAndHash[0]);
            var hashed = Convert.FromBase64String(saltAndHash[1]);
            var IPHash = Rfc2898DeriveBytes.Pbkdf2(InputPassword, salt, Iterations, hashAlgorithmName, KeySize);
            return CryptographicOperations.FixedTimeEquals(hashed, IPHash);
        }
    }
}
