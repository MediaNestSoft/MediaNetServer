using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaNetServer.Models.Files;
using MediaNetServer.Data.media;
using MediaNetServer.Data.media.Models;
using MediaNetServer.Services.Directory;

namespace MediaNetServer.Services.Folder;
/*
public class FileSystemFolderManager : FolderManager
{
        private readonly IFolderRepository _repo;
        private readonly IDirectoryService _fs;

        public SimpleFolderManager(
            IFolderRepository repository,
            IDirectoryService fileSystem)
        {
            _repo = repository;
            _fs = fileSystem;
        }

        public async Task<IEnumerable<FolderInfo>> GetAllFoldersAsync() =>
            (await _repo.GetFoldersAsync())
                .Select(f => new FolderInfo { Id = f.Id, Name = f.Name, Path = f.Path });

        public async Task<FolderInfo?> GetFolderByIdAsync(Guid id)
        {
            var f = await _repo.FindFolderAsync(id);
            return f is null
                ? null
                : new FolderInfo { Id = f.Id, Name = f.Name, Path = f.Path };
        }

        public async Task<FolderInfo> CreateFolderAsync(string name, string path)
        {
            _fs.CreateDirectory(path);
            var entity = new Folders { Id = Guid.NewGuid(), Name = name, Path = path };
            await _repo.SaveFolderAsync(entity);
            return new FolderInfo { Id = entity.Id, Name = name, Path = path };
        }

        public Task<bool> DeleteFolderAsync(Guid id)
        {
            return DeleteAndRemoveAsync(id);
        }

        private async Task<bool> DeleteAndRemoveAsync(Guid id)
        {
            var f = await _repo.FindFolderAsync(id);
            if (f == null) return false;
            _fs.DeleteDirectory(f.Path, true);
            return await _repo.DeleteFolderAsync(id);
        }

        public async Task<bool> RenameFolderAsync(Guid id, string newName)
        {
            var f = await _repo.FindFolderAsync(id);
            if (f == null) return false;
            var newPath = _fs.RenameDirectory(f.Path, newName);
            f.Name = newName;
            f.Path = newPath;
            await _repo.SaveFolderAsync(f);
            return true;
        }

        public async Task<IEnumerable<ItemInfo>> ListFolderItemsAsync(Guid folderId)
        {
            var f = await _repo.FindFolderAsync(folderId);
            if (f == null) return Enumerable.Empty<ItemInfo>();
            return _fs.GetFileSystemEntries(f.Path)
                      .Select(e => new ItemInfo(Guid.Empty, e.Name, e.Path)); // 若要持久化可查实体
        }

        public async Task<bool> AddItemToFolderAsync(Guid folderId, ItemInfo item)
        {
            var entity = new ItemEntity
            {
                Id = Guid.NewGuid(),
                Name = item.Name,
                Path = item.Path,
                FolderId = folderId
            };
            return await _repo.AddItemAsync(folderId, entity);
        }

        public Task<bool> RemoveItemFromFolderAsync(Guid folderId, Guid itemId) =>
            _repo.RemoveItemAsync(folderId, itemId);
}*/