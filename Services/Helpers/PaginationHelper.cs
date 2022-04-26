using System.Text.Json;
using Fakestagram.Data.DTOs.Pagination;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Http.Headers;

namespace Fakestagram.Services.Helpers
{
    public class PaginationHelper : IPaginationHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string HTTP_HEADER_NAME = "X-Pagination";

        public PaginationHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetPaginationHeader(int totalCount, int currentPageNumber, int itemsPerPage)
        {
            var paginationMetaData = new PaginationMetadata(totalCount, currentPageNumber, itemsPerPage);
            _httpContextAccessor.HttpContext.Response.Headers.Add(HTTP_HEADER_NAME, JsonSerializer.Serialize(paginationMetaData));
        }
    }
}
