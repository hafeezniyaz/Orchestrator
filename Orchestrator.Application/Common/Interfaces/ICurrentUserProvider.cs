namespace Orchestrator.Application.Common.Interfaces
{
    /// <summary>
    /// Provides access to the details of the currently authenticated user.
    /// This abstracts the underlying mechanism (e.g., HttpContext) for better testability and separation of concerns.
    /// </summary>
    public interface ICurrentUserProvider
    {
        /// <summary>
        /// Gets the unique identifier of the current user.
        /// Can be null if the user is not authenticated.
        /// </summary>
        string? UserId { get; }

        /// <summary>
        /// Gets the name of the current user.
        /// Can be null if the user is not authenticated.
        /// </summary>
        string? UserName { get; }
    }
}