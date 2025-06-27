namespace MediaNetServer.Models.Playlist;

public class PlaylistSummaryDto
{
    public int PlaylistId { get; set; }
    public required string Name { get; set; }
}

// 单个播放列表项中的媒体信息
public class PlaylistMediaDto
{
    public int MediaId { get; set; }
    public required string Title { get; set; }
    public required string Type { get; set; }       // Movie | Series
    public string? PosterPath { get; set; }
    public int? SeasonNumber { get; set; } // 如果是剧集
    public int? EpisodeNumber { get; set; }// 如果是剧集
}

/// <summary>
/// 播放列表详情，包括列表基础信息和其中的所有媒体
/// 对应：GET /playlists/{userId}/{playlistId}
/// </summary>
public class PlaylistDetailDto
{
    public int PlaylistId { get; set; }
    public required string Name { get; set; }
    public bool IsSystem { get; set; }
    public IEnumerable<PlaylistMediaDto> Items { get; set; }
}

/// <summary>
/// 向播放列表中添加媒体请求 DTO
/// 对应：POST /playlists/{userId}/{playlistId}/items
/// </summary>
public class AddPlaylistItemsRequestDto
{
    public required IEnumerable<int> MediaIds { get; set; }
}

/// <summary>
/// 从播放列表中移除媒体请求 DTO
/// 对应：DELETE /playlists/{userId}/{playlistId}/items
/// </summary>
public class RemovePlaylistItemsRequestDto
{
    public required IEnumerable<int> MediaIds { get; set; }
}

/// <summary>
/// 播放列表操作结果 DTO（添加/移除后的反馈）
/// </summary>
public class ModifyPlaylistItemsResponseDto
{
    public required IEnumerable<int> Succeeded { get; set; }
    public required IEnumerable<int> Failed { get; set; }
}