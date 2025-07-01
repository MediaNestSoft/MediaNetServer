using Org.OpenAPITools.Model;

namespace MediaNetServer.Services.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResponse> SearchAsync(string? query = null, string? type = null, int? page = null, int? limit = null);
        Task<AutocompleteResponse> AutocompleteAsync(string query, int? limit = null);
    }
}
