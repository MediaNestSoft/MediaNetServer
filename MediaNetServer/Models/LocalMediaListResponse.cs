namespace MediaNetServer.Models;

public class LocalMediaListResponse
{
    public List<LocalMediaItem> Items { get; set; }
    public int TotalCount { get; set; }
    
    public LocalMediaListResponse(List<LocalMediaItem> items, int totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }
    public LocalMediaListResponse() {}
}