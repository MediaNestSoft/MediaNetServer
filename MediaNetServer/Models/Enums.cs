namespace MediaNetServer.Models;

public static class MediaType
{
    public const string Movie = "Movie";
    public const string Series = "Series";

    public static readonly HashSet<string> ValidTypes = new()
    {
        Movie, Series
    };

    public static bool IsValid(string? type)
    {
        return type != null && ValidTypes.Contains(type);
    }
}