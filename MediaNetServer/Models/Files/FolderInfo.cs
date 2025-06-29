namespace MediaNetServer.Models.Files;

public class FolderInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;
}