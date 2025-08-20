using System;

namespace Orchestrator.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a specific requested resource is not found.
    /// This will be mapped to an HTTP 404 Not Found response.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}