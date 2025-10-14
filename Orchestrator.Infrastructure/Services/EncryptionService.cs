using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Orchestrator.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(IConfiguration configuration)
        {
            var base64Key = configuration["EncryptionSettings:CredentialKey"];
            if (string.IsNullOrEmpty(base64Key))
                throw new InvalidOperationException("Credential key is not defined in the config");
            _key = Convert.FromBase64String(base64Key);
        }
        public string Decrypth(string cipherText)
        {
            try
            {

                var parts = cipherText.Split(":");
                if (parts.Length != 3)
                {
                    throw new FormatException("Invalid cipher format");

                }

                byte[] nonce = Convert.FromBase64String(parts[0]);
                byte[] cipherTextBytes = Convert.FromBase64String(parts[1]);
                byte[] tag = Convert.FromBase64String(parts[2]);

                byte[] decryptBytes = new byte[cipherTextBytes.Length];
                using (var aes = new AesGcm(_key))
                {
                    aes.Decrypt(nonce, cipherTextBytes, tag, decryptBytes);
                }

                return Encoding.UTF8.GetString(decryptBytes);
            }
            catch (AuthenticationTagMismatchException ex)
            {

                throw new CryptographicException("decryption failed due to data integrity issue");
            }
            catch(Exception ex)
            {
                throw new CryptographicException("Decrption failed.", ex);
            }
        }

        public string Encrypt(string plainText)
        {
            //nonce (number used once for AES-GCM with 12 as standard size
            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            // tag ensures data integrity 
            byte[] tag = new byte[16];
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherTextBytes = new byte[plainTextBytes.Length];

            using (var aes = new AesGcm(_key))
            {
                aes.Encrypt(nonce, plainTextBytes, cipherTextBytes, tag);
            }

            return $"{Convert.ToBase64String(nonce)}:{Convert.ToBase64String(cipherTextBytes)}:{Convert.ToBase64String(tag)}";

        }
    }
}
