namespace MediaNetServer.Data.media.Models;

public class Folders
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;

    // 与 files 的关联
    public ICollection<Files> Items { get; set; } = new List<Files>();
}