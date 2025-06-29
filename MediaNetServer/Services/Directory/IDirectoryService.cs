using System.Collections.Generic;
namespace MediaNetServer.Services.Directory;

public interface IDirectoryService
{
    void CreateDirectory(string path);
    void DeleteDirectory(string path, bool recursive);
    string RenameDirectory(string oldPath, string newName);
    IEnumerable<(string Name, string Path)> GetFileSystemEntries(string path);
}