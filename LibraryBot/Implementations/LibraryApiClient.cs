using LibraryBot.Interfaces;
using SQLitePCL;
using System.Net.Http.Json;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Authors;
using Tekwill.Library.Application.DTOs.Books;
using Tekwill.Library.Application.DTOs.Categories;

namespace LibraryBot.Implementations
{
    public class LibraryApiClient : ILibraryApiClient
    {
        private readonly ILogger<LibraryApiClient> logger;
        private readonly HttpClient client;

        public LibraryApiClient(IHttpClientFactory httpClientFactory, ILogger<LibraryApiClient> logger)
        {
            this.logger = logger;
            client = httpClientFactory.CreateClient(Constants.LibraryApiClient);
        }
        public async Task<AuthorDto> GetAuthorById(int id, CancellationToken ct)
        {
            try
            {
                return await client.GetFromJsonAsync<AuthorDto>($"authors/{id}", ct);
            }
            catch(Exception e)
            {
                logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<BookDto> GetBooksById(int id, CancellationToken ct)
        {
            try
            {
                return await client.GetFromJsonAsync<BookDto>($"books/{id}", ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<BookDto> GetBooksByQuery(string query, CancellationToken ct)
        {
            try
            {
                return await client.GetFromJsonAsync<BookDto>($"books/search?query={query}", ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<CategoryDto> GetCategoriesById(int id, CancellationToken ct)
        {
            try
            {
                return await client.GetFromJsonAsync<CategoryDto>($"categories/{id}", ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<PaginatedList<AuthorDto>> GetPaginatedAuthors(int pageSize, int pageIndex, CancellationToken ct)
        {
            try
            {
                return await client.GetFromJsonAsync<PaginatedList<AuthorDto>>($"authors?page={pageIndex}&pageSize={pageSize}", ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<PaginatedList<BookDto>> GetPaginatedBooks(int pageSize, int pageIndex, CancellationToken ct)
        {
            try
            {
                return await client.GetFromJsonAsync<PaginatedList<BookDto>>($"books?page={pageIndex}&pageSize={pageSize}", ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<PaginatedList<CategoryDto>> GetPaginatedCategories(int pageSize, int pageIndex, CancellationToken ct)
        {
            try
            {
                return await client.GetFromJsonAsync<PaginatedList<CategoryDto>>($"categories?page={pageIndex}&pageSize={pageSize}", ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return null;
            }
        }
    }
}
