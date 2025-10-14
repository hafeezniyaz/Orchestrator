using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Common.Interfaces
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypth the string
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        string Encrypt(string plainText);

        /// <summary>
        /// Decrypt the string
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        string Decrypth(string cipherText);
    }
}
