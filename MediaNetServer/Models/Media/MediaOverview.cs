
namespace MediaNetServer.Models.Media;

public class MediaOverviewDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Type { get; set; }      // "Movie" | "Series"
    public required string PosterPath { get; set; }
    public required string Additional { get; set; } // 年份或季数
}

public class PagedMediaResponseDto
{
    public required IEnumerable<MediaOverviewDto> Items { get; set; }
    public int TotalCount { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
}