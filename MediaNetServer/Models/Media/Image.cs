namespace MediaNetServer.Models.Media;

public class ImageDto
{
    public required string ImageType { get; set; }     // Poster|Backdrop|...
    public string? Path { get; set; }
}

public class ImagesResponseDto
{
    public IEnumerable<ImageDto>? Images { get; set; }
}