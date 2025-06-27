namespace MediaNetServer.Models.Playback;

public class PlaybackProgressRequestDto
{
    public int MediaId { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public int Position { get; set; }
}

public class HistoryItemDto
{
    public int MediaId { get; set; }
    public required string Title { get; set; }
    public DateTime WatchedAt { get; set; }
    public int Position { get; set; }
    public int Duration { get; set; }
    public required string BackdropPath { get; set; }
    public string? Additional { get; set; }
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
}

public class PlaybackHistoryResponseDto
{
    public IEnumerable<HistoryItemDto>? Items { get; set; }
    public int TotalCount { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
}