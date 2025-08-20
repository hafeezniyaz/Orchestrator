using Orchestrator.Application.Features.Credentials.Models;
using Orchestrator.Domain.Entities;

namespace Orchestrator.Infrastructure.Features.Credentials.Helpers
{
    /// <summary>
    /// A static class containing extension methods for mapping Credential entities to DTOs.
    /// </summary>
    public static class CredentialMapper
    {
        /// <summary>
        /// Maps a Credential entity to its "safe" CredentialDto representation.
        /// </summary>
        /// <param name="credential">The source Credential entity.</param>
        /// <returns>A new CredentialDto object.</returns>
        public static CredentialDto ToCredentialDto(this Credential credential)
        {
            return new CredentialDto
            {
                Id = credential.Id,
                Name = credential.Name,
                Description = credential.Description,
                Username = credential.Username,
                AppId = credential.AppId,
                CreatedAt = credential.CreatedAt,
                ModifiedAt = credential.ModifiedAt
            };
        }
    }
}