using MediaNetServer.Models.Media;

namespace MediaNetServer.Models.Search;

public class GenreSummaryDto
{
    public required string GenreId { get; set; }
    public required string Name { get; set; }
    public int MediaCount { get; set; }
}

public class GenresResponseDto
{
    public required IEnumerable<GenreSummaryDto> Items { get; set; }
}

public class GenreMediaResponseDto : PagedMediaResponseDto { }