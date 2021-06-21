using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Core
{
    // generic parameter T: can be a list of anything
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            // add the items we get, pass it in as a parameter into the class
            AddRange(items); // without this, it will return 0 items
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        // static CreateAsync method: create a page list and return it
        // IQuerable: receive a list of items before being executed to a list in DB, deffering the execution
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // "source" is a query going to the DB
            var count = await source.CountAsync(); // query the db to get total number
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); // deferring db execution
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}