namespace MediaNetServer.Models;

public class LocalReportPlaybackRequest
{
    public int MediaId { get; set; }
    public int Position { get; set; }
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
}