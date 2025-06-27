namespace MediaNetServer.Models.PlayControl;

public class StartPlaybackRequestDto
{
    public int MediaId { get; set; }
    public required string FileId { get; set; }
}

public class StartPlaybackResponseDto
{
    public required string StreamId { get; set; }
}

public class PlaybackCommandRequestDto
{
    public required string Command { get; set; }       // Play|Pause|Seek
    public int? Position { get; set; }
}