using System;
using System.Collections.Generic;
using System.Linq;

namespace BandApi.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(IEnumerable<T> items, int pageSize, int currentPage, int totalItems)
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(Convert.ToDouble(TotalItems) / PageSize);
            AddRange(items);
        }

        public static PagedList<T> InstantiatePagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var totalItems = source.Count();
            var items = source.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToList();

            return new PagedList<T>(items, pageSize, pageNumber, totalItems);

        }
    }
}
