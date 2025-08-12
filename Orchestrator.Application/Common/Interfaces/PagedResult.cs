namespace Orchestrator.Application.Common.Models
{
    /// <summary>
    /// A generic wrapper for returning paginated data from an API.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// The items for the current page.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        /// <summary>
        /// The total number of records available across all pages.
        /// </summary>
        public int TotalCount { get; set; }
    }
}