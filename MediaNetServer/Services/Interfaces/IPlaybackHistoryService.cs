using Org.OpenAPITools.Model;

namespace MediaNetServer.Services.Interfaces
{
    public interface IPlaybackHistoryService
    {
        Task<PlaybackHistoryResponse> GetPlaybackHistoryAsync(int? page = null, int? limit = null);
        Task<bool> ReportPlaybackAsync(ReportPlaybackRequest request);
    }
}
