namespace Orchestrator.Application.Common.Interfaces
{
    public interface ISecretHasher
    {
        string HashSecret(string secret);
        bool VerifySecret(string providedSecret, string hashedSecret);
    }
}