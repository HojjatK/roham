using System.Collections;
using System.Collections.Generic;

namespace Roham.Lib.Domain
{
    public class PagedResult<T> : IEnumerable<T>
    {
        private readonly IList<T> _results;

        public PagedResult(IList<T> results, int totalCount, int skipped, int itemsPerPage)
        {
            _results = results;
            TotalResults = totalCount;
            ItemsPerPage = itemsPerPage;
            Page = (int)((decimal)skipped / itemsPerPage) + 1;
            TotalPages = (int)((decimal)totalCount / itemsPerPage + 1);
        }

        public int TotalResults { get; }
        public int Page { get; }
        public int TotalPages { get;  }
        public int ItemsPerPage { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
