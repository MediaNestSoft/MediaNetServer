using Org.OpenAPITools.Model;
using Microsoft.AspNetCore.Mvc;

namespace MediaNetServer.Services.Interfaces
{
    public interface IPlaylistService
    {
        Task<PlaylistsResponse> GetPlaylistsAsync();
        Task<CreatePlaylistResponse> CreatePlaylistAsync(CreatePlaylistRequest request);
        Task<bool> RenamePlaylistAsync(int playlistId, RenamePlaylistRequest request);
        Task<bool> DeletePlaylistAsync(int playlistId);
        Task<bool> AddToPlaylistAsync(int playlistId, MediaIdsRequest request);
        Task<bool> RemoveFromPlaylistAsync(int playlistId, MediaIdsRequest request);
        Task<MediaIdsResponse> GetPlaylistMediaAsync(int playlistId);
    }
}
