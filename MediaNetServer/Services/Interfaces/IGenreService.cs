using Org.OpenAPITools.Model;

namespace MediaNetServer.Services.Interfaces
{
    public interface IGenreService
    {
        Task<GenresResponse> GetGenresAsync();
        Task<MediaListResponse> GetMediaByGenreAsync(string genre, int? page = null, int? limit = null);
    }
}
