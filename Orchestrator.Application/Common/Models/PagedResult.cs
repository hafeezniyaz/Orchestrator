using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Common.Models
{
    /// <summary>
    /// A generic wrapper for paginated data for api response
    /// </summary>
    public class PagedResult<T>
    {
        /// <summary>
        /// total count for pagination
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Items for the current page
        /// </summary>
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    }
}
