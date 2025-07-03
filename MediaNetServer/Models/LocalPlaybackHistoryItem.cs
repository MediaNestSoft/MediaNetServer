namespace MediaNetServer.Models;

public class LocalPlaybackHistoryItem
{
    public int? MediaId { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; }
    public string? PosterPath { get; set; }
    public string? Additional { get; set; }
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
    public int? Position { get; set; }
    public int? Runtime { get; set; }

    public LocalPlaybackHistoryItem(int? mediaId, string? title, string? type,
        string? posterPath, string? additional, int? seasonNumber, int? episodeNumber,
        int? position, int? runtime)
    {
        MediaId = mediaId;
        Title = title;
        Type = type;
        PosterPath = posterPath;
        Additional = additional;
        SeasonNumber = seasonNumber;
        EpisodeNumber = episodeNumber;
        Position = position;
        Runtime = runtime;
    }
    public LocalPlaybackHistoryItem(){}
}