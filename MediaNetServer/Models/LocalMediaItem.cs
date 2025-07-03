namespace MediaNetServer.Models;

public class LocalMediaItem
{
    public int? MediaId { get; set; }
    public string? Title { get; set; }
    public string? Type {get; set;}
    public string? PosterPath { get; set; }
    public string? Additional {get; set;}
    
    public LocalMediaItem(
        int? tmDbId = null,
        string? title = null,
        string? type = null,
        string? posterPath = null,
        string? additional = null)
    {
        MediaId = tmDbId;
        Title = title;
        Type = type;
        PosterPath = posterPath;
        Additional = additional;

    }
}