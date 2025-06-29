namespace MediaNetServer.Models.Files;

public class ItemInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;

    public ItemInfo() { }

    public ItemInfo(Guid id, string name, string path)
    {
        Id = id;
        Name = name;
        Path = path;
    }
}