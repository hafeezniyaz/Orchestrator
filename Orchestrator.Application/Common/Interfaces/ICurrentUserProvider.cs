namespace Orchestrator.Application.Common.Interfaces
{
    public interface ICurrentUserProvider
    {
        string? UserId { get; }
        string? UserName { get; }
    }
}