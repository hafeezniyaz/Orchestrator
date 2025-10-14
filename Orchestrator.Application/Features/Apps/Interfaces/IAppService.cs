using Orchestrator.Application.Common.Models;
using Orchestrator.Application.Features.Apps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Apps.Interfaces
{
    public interface IAppService
    {
        /// <summary>
        /// retrieve list of apps 
        /// </summary>
        /// <returns></returns>
        Task<PagedResult<AppDto>> GetAllAppsAsync(string? name, bool? isActive, int skip=0, int top=50);

        /// <summary>
        /// get app by its ID
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<AppDto?> GetAppByIdAsync(Guid appId);

        /// <summary>
        /// Create new app
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AppDto?> CreateAppAsync(CreateAppRequest request);

        /// <summary>
        /// update the app using id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AppDto?> UpdateAppAsync(Guid id, UpdateAppRequest request);

        /// <summary>
        /// delete app by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAppAsync(Guid id);

    }
}
