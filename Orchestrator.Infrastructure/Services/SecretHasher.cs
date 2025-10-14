using Microsoft.Identity.Client;
using Orchestrator.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Services
{
    public class SecretHasher : ISecretHasher
    {
        public string HashSecret(string secret) => BCrypt.Net.BCrypt.HashPassword(secret);

        public bool VerifySecret(string providedSecret, string hashSecret) => BCrypt.Net.BCrypt.Verify(providedSecret, hashSecret);
    }
}
