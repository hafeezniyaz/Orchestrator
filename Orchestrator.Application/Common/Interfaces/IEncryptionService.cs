namespace Orchestrator.Application.Common.Interfaces
{
    /// <summary>
    /// Defines a service for performing strong, authenticated encryption and decryption.
    /// This contract ensures that secrets are handled securely for data at rest.
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypts a plain-text string into a secure format containing the ciphertext,
        /// initialization vector (nonce), and authentication tag.
        /// </summary>
        /// <param name="plainText">The string to encrypt.</param>
        /// <returns>A formatted, encrypted string ready for database storage.</returns>
        string Encrypt(string plainText);

        /// <summary>
        /// Decrypts a previously encrypted string, verifying its integrity in the process.
        /// </summary>
        /// <param name="cipherText">The encrypted string from the database.</param>
        /// <returns>The original plain-text string.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException">
        /// Thrown if decryption fails, which can indicate tampering or a corrupted value.
        /// </exception>
        string Decrypt(string cipherText);
    }
}