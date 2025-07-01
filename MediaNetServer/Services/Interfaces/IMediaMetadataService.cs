using Org.OpenAPITools.Model;

namespace MediaNetServer.Services.Interfaces
{
    public interface IMediaMetadataService
    {
        Task<GetMediaDetail200Response> GetMediaDetailAsync(int mediaId);
        Task<CreditsResponse> GetMediaCreditsAsync(int mediaId);
        Task<CreditsResponse> GetEpisodeCreditsAsync(int seriesId, int seasonNumber, int episodeNumber);
        Task<EpisodesResponse> GetEpisodesAsync(int seriesId, int seasonNumber);
        Task<MediaFilesResponse> GetMediaFilesAsync(int mediaId);
        Task<SeasonsResponse> GetSeasonsAsync(int seriesId);
    }
}
