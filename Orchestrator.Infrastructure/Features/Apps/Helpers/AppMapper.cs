using Orchestrator.Application.Features.Apps.Models;
using Orchestrator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Features.Apps.Helpers
{
    public static class AppMapper
    {
        public static AppDto ToAppDto(this App app)
        {
            return new AppDto
            {
                Name = app.Name,
                Description = app.Description,
                Id = app.Id,
                IsActive = app.IsActive
            };
        }
    }
}
