using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PassManaAlpha.Core.Scurity
{
    public static class HakoHelper
    {
        // Derives a 256-bit key from a password using PBKDF2
        private static byte[] DeriveKey(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32); // AES-256
        }

        public static string Encrypt(string plainText, string password)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentException("Cannot encrypt empty or null plaintext");

            using var aes = Aes.Create();
            aes.GenerateIV();
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            aes.Key = DeriveKey(password, salt);

            using var ms = new MemoryStream();
            ms.Write(salt, 0, salt.Length);       // prepend salt
            ms.Write(aes.IV, 0, aes.IV.Length);   // prepend IV

            using (var encryptor = aes.CreateEncryptor())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
                sw.Flush(); 
                cs.FlushFinalBlock();
            }

            return Convert.ToBase64String(ms.ToArray());
        }



        public static string Decrypt(string cipherText, string password)
        {
            try
            {
                byte[] fullData = Convert.FromBase64String(cipherText);
                Debug.WriteLine($"FullData length: {fullData.Length}");

                if (fullData.Length < 32)
                    throw new Exception("Encrypted data too short to contain salt and IV");

                byte[] salt = fullData[..16];
                byte[] iv = fullData[16..32];
                byte[] encrypted = fullData[32..];

                using var aes = Aes.Create();
                aes.Key = DeriveKey(password, salt);
                aes.IV = iv;

                using var ms = new MemoryStream(encrypted);
                using var decryptor = aes.CreateDecryptor();
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                string result = sr.ReadToEnd();

                Debug.WriteLine($"Decryption result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Decrypt failed: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
