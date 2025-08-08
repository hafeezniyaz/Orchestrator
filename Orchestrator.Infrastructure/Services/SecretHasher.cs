using Orchestrator.Application.Common.Interfaces;

namespace Orchestrator.Infrastructure.Services
{
    public class SecretHasher : ISecretHasher
    {
        public string HashSecret(string secret) => BCrypt.Net.BCrypt.HashPassword(secret);
        public bool VerifySecret(string providedSecret, string hashedSecret) => BCrypt.Net.BCrypt.Verify(providedSecret, hashedSecret);
    }
}