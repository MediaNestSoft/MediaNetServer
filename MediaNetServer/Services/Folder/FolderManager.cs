using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaNetServer.Data.media.Models;

namespace MediaNetServer.Services.Folder;

public interface FolderManager
{
    Task<IEnumerable<Folders>> GetAllFoldersAsync();
    Task<Folders?> GetFolderByIdAsync(Guid id);
    Task<Folders> CreateFolderAsync(string name, string path);
    Task<bool> DeleteFolderAsync(Guid id);
    Task<bool> RenameFolderAsync(Guid id, string newName);
    Task<IEnumerable<Files>> ListFolderItemsAsync(Guid folderId);
    Task<bool> AddItemToFolderAsync(Guid folderId, Files item);
    Task<bool> RemoveItemFromFolderAsync(Guid folderId, Guid itemId);
}