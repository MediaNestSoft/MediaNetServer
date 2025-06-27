using MediaNetServer.Models.Media;

namespace MediaNetServer.Models.Search;

public class SearchResponseDto : PagedMediaResponseDto { }

public class AutocompleteResponseDto
{
    public IEnumerable<MediaOverviewDto> Items { get; set; }
}