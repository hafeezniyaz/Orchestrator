using Microsoft.Extensions.Configuration;
using Orchestrator.Application.Common.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Orchestrator.Infrastructure.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(IConfiguration configuration)
        {
            var base64Key = configuration["EncryptionSettings:CredentialKey"];
            if (string.IsNullOrEmpty(base64Key))
            {
                throw new InvalidOperationException("Encryption MasterKey is not configured.");
            }
            _key = Convert.FromBase64String(base64Key);
        }

        public string Encrypt(string plainText)
        {
            // AES-GCM uses a "nonce" (number used once), which is similar to an IV.
            // For AES-GCM, the standard nonce size is 12 bytes.
            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            // The authentication tag is a key component of GCM. It ensures data integrity.
            // The standard size is 16 bytes.
            byte[] tag = new byte[16];
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherTextBytes = new byte[plainTextBytes.Length];

            using (var aes = new AesGcm(_key))
            {
                aes.Encrypt(nonce, plainTextBytes, cipherTextBytes, tag);
            }

            // We combine the nonce, ciphertext, and tag into a single string for storage.
            // This makes it easy to retrieve all necessary components for decryption.
            return $"{Convert.ToBase64String(nonce)}:{Convert.ToBase64String(cipherTextBytes)}:{Convert.ToBase64String(tag)}";
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                var parts = cipherText.Split(':');
                if (parts.Length != 3)
                {
                    throw new FormatException("Invalid ciphertext format.");
                }

                byte[] nonce = Convert.FromBase64String(parts[0]);
                byte[] cipherTextBytes = Convert.FromBase64String(parts[1]);
                byte[] tag = Convert.FromBase64String(parts[2]);

                byte[] decryptedBytes = new byte[cipherTextBytes.Length];

                using (var aes = new AesGcm(_key))
                {
                    aes.Decrypt(nonce, cipherTextBytes, tag, decryptedBytes);
                }

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (AuthenticationTagMismatchException ex)
            {
                // This is a CRITICAL exception. It means the data has been tampered with.
                // We throw a generic CryptographicException to avoid leaking details.
                throw new CryptographicException("Decryption failed due to a data integrity issue.", ex);
            }
            catch (Exception ex)
            {
                // Catch other potential errors like bad Base64 formatting.
                throw new CryptographicException("Decryption failed.", ex);
            }
        }
    }
}