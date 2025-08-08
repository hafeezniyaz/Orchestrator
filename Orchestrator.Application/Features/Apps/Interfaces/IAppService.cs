using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchestrator.Application.Features.Apps.Models;

namespace Orchestrator.Application.Features.Apps.Interfaces
{
    public interface IAppService
    {
        /// <summary>
        /// Retrieves a list of all applications with optional filtering and pagination.
        /// </summary>
        /// <param name="name">Optional filter by application name (contains)</param>
        /// <param name="isActive">Optional filter by active status</param>
        /// <param name="skip">Number of items to skip for pagination</param>
        /// <param name="top">Number of items to take for pagination</param>
        /// <returns>A list of application DTOs.</returns>
        Task<IEnumerable<AppDto>> GetAllAppsAsync(string? name = null, bool? isActive = null, int skip = 0, int top = 100);

        /// <summary>
        /// Retrieves a specific application by its ID.
        /// </summary>
        /// <param name="appId">The unique identifier of the application.</param>
        /// <returns>The application DTO if found; otherwise, null.</returns>
        Task<AppDto?> GetAppByIdAsync(Guid appId);

        /// <summary>
        /// Creates a new application.
        /// </summary>
        /// <param request="CreateRequestAppModel">The request object containing the details of the new application.</param>
        /// <returns>The created application DTO.</returns>
        Task<AppDto> CreateAppAsync(CreateAppRequest request);

        /// <summary>
        /// update an existing app
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AppDto?> UpdateAppAsync(Guid id, UpdateAppRequest request);

        /// <summary>
        /// Delete app by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAppAsync(Guid id);
    }
}
