using Orchestrator.Application.Features.Credentials.Models;
using Orchestrator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Features.Credentials.Helpers
{
   public static class CredentialMapper
    {
        public static CredentialDto ToCredentialDto (this Credential credential)
        {
            return new CredentialDto
            {
                Id = credential.Id,
                Name = credential.Name,
                Username = credential.Username,
                AppID = credential.AppId,
                CreatedAt = credential.CreatedAt,
                ModifiedAt = credential.ModifiedAt,
                Decription = credential.Description

            };
        }
    }
}
