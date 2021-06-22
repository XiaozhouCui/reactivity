using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        // add pagination header to responses
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            // create a anonymous object, to be serialised into JSON
            var paginationHeader = new
            {
                currentPage,
                itemsPerPage,
                totalItems,
                totalPages
            };
            // create custom header "Pagination"
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
            // expose custom header
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}