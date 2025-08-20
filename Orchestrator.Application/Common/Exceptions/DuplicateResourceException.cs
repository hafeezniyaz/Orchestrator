using System;

namespace Orchestrator.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when an attempt is made to create a resource that already exists.
    /// This will be mapped to an HTTP 409 Conflict response.
    /// </summary>
    public class DuplicateResourceException : Exception
    {
        public DuplicateResourceException(string message) : base(message)
        {
        }
    }
}