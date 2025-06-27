using MediaNetServer.Models.Media;

namespace MediaNetServer.Models.Playlist;

public class RecentAddResponseDto : PagedMediaResponseDto { }

public class ContinueWatchItemDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string BackdropUrl { get; set; }
    public int Position { get; set; }
    public int Duration { get; set; }
    public string? Additional { get; set; }
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
}

public class ContinueWatchResponseDto
{
    public IEnumerable<ContinueWatchItemDto>? Items { get; set; }
    public int TotalCount { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
}

public class RecommendedItemDto : MediaOverviewDto { }

public class RecommendedResponseDto : PagedMediaResponseDto { }

