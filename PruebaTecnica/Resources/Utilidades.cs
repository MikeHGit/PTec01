//using System.Security.Cryptography;
//using System.Text;

//namespace PruebaTecnica.Resources
//{
//    public class Utilidades
//    {
//        public static string EncriptarPassword(string pass)
//        {
//            StringBuilder sb = new StringBuilder();
//            //using (SHA256 hash = SHA256.Create())
//            using (SHA256 hash = SHA256.Create())
//            {
//                Encoding enc = Encoding.UTF8;
//                byte[] result = hash.ComputeHash(enc.GetBytes(pass));

//                foreach (byte b in result)
//                {
//                    sb.Append(b.ToString("x2"));

//                }
//            }
//            return sb.ToString();
//        }
//    }
//}

using System;
using System.Security.Cryptography;
using System.Text;

namespace PruebaTecnica.Resources
{
    public class Utilidades
    {
        private const int SaltSize = 128 / 8;
        private const int KeySize = 256 / 8;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private const char Delimiter = ';';

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);

            return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public static bool Verify(string passwordHash, string inputPassword)
        {
            var elements = passwordHash.Split(Delimiter);
            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);

            var hashInput = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, Iterations, _hashAlgorithmName, KeySize);

            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }
    }
}


