using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
/*
namespace MediaNetServer.Services.Directory;

public class FileSystemDirectoryService : IDirectoryService
{
    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public void DeleteDirectory(string path, bool recursive)
    {
        Directory.Delete(path, recursive);
    }

    public string RenameDirectory(string oldPath, string newName)
    {
        var parent = Path.GetDirectoryName(oldPath) ?? throw new InvalidOperationException("Invalid path");
        var newPath = Path.Combine(parent, newName);
        Directory.Move(oldPath, newPath);
        return newPath;
    }

    public IEnumerable<(string Name, string Path)> GetFileSystemEntries(string path)
    {
        var dirs = Directory.GetDirectories(path)
            .Select(d => (Name: Path.GetFileName(d), Path: d));
        var files = Directory.GetFiles(path)
            .Select(f => (Name: Path.GetFileName(f), Path: f));
        return dirs.Concat(files);
    }
}*/