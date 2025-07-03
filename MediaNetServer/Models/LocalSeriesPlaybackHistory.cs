namespace MediaNetServer.Models;

public class LocalSeriesPlaybackHistory
{
    public int? MediaId { get; set; }
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
    public int? Position { get; set; }
    public int? Runtime { get; set; }
    
    public LocalSeriesPlaybackHistory(){}
}