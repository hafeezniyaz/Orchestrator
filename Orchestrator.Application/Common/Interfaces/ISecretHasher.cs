namespace Orchestrator.Application.Common.Interfaces
{
    /// <summary>
    /// Defines a service for securely hashing and verifying secrets.
    /// </summary>
    public interface ISecretHasher
    {
        /// <summary>
        /// Creates a secure hash from a plain-text secret.
        /// </summary>
        /// <param name="secret">The plain-text secret to hash.</param>
        /// <returns>The resulting hash string.</returns>
        string HashSecret(string secret);

        /// <summary>
        /// Verifies that a plain-text secret matches a stored hash.
        /// </summary>
        /// <param name="providedSecret">The plain-text secret from the user/client.</param>
        /// <param name="hashedSecret">The stored hash from the database.</param>
        /// <returns>True if the secret matches the hash, otherwise false.</returns>
        bool VerifySecret(string providedSecret, string hashedSecret);
    }
}