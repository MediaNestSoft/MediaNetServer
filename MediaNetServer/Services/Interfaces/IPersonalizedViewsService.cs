using Org.OpenAPITools.Model;

namespace MediaNetServer.Services.Interfaces
{
    public interface IPersonalizedViewsService
    {
        Task<ContinueWatchResponse> GetContinueWatchingAsync(int? limit = null);
    }
}
