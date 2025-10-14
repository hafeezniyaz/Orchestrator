using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Common.Interfaces
{
    public interface ISecretHasher
    {
        /// <summary>
        /// create a secure hash from a Plain-text secret
        /// </summary>
        /// <param name="secret">the plain text secret</param>
        /// <returns></returns>
        string HashSecret(string secret);

        /// <summary>
        /// verify that the plain text secret matched the stored hash 
        /// </summary>
        /// <param name="providedSecret">the plain text secret</param>
        /// <param name="hashSecret">the stored hash secret</param>
        /// <returns></returns>
        bool VerifySecret(string providedSecret, string hashSecret);
    }
}
