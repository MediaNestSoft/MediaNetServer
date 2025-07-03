namespace MediaNetServer.Models;

public class LocalPlaybackHistoryResponse
{
    public List<LocalPlaybackHistoryItem>? Items { get; set; } 
    public int TotalCount { get; set; }
    
    public LocalPlaybackHistoryResponse(){}
}