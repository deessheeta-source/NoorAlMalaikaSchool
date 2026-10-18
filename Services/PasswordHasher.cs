using System;
using System.Security.Cryptography;
using System.Text;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// Password hashing for .NET Framework 4.8.
    /// New passwords use PBKDF2 with a per-password random salt.
    /// Legacy SHA-256 hashes are supported only for one-time migration on successful login.
    /// </summary>
    public static class PasswordHasher
    {
        private const string Prefix = "PBKDF2";
        private const int Iterations = 120000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException("password");
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                hash = pbkdf2.GetBytes(HashSize);
            return string.Format("{0}${1}${2}${3}", Prefix, Iterations,
                Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public static bool Verify(string password, string encoded)
        {
            if (password == null || string.IsNullOrEmpty(encoded)) return false;
            string[] parts = encoded.Split('$');
            if (parts.Length != 4 || !string.Equals(parts[0], Prefix, StringComparison.Ordinal)) return false;

            int iterations;
            if (!int.TryParse(parts[1], out iterations) || iterations < 10000 || iterations > 1000000) return false;

            byte[] salt, expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException) { return false; }

            byte[] actual;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                actual = pbkdf2.GetBytes(expected.Length);

            return FixedTimeEquals(actual, expected);
        }

        public static bool IsLegacySha256(string encoded)
        {
            if (string.IsNullOrEmpty(encoded) || encoded.Length != 64) return false;
            for (int i = 0; i < encoded.Length; i++)
                if (!Uri.IsHexDigit(encoded[i])) return false;
            return true;
        }

        public static bool VerifyLegacySha256(string password, string encoded)
        {
            if (!IsLegacySha256(encoded)) return false;
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? ""));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return FixedTimeEquals(Encoding.ASCII.GetBytes(sb.ToString()), Encoding.ASCII.GetBytes(encoded.ToLowerInvariant()));
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
