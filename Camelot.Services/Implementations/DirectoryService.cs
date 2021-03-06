using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Camelot.Extensions;
using Camelot.Services.EventArgs;
using Camelot.Services.Interfaces;
using Camelot.Services.Models;

namespace Camelot.Services.Implementations
{
    public class DirectoryService : IDirectoryService
    {
        private readonly IPathService _pathService;
        private const string ParentDirectoryName = "[..]";

        private string _directory;

        public string SelectedDirectory
        {
            get => _directory;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _directory = value;

                var args = new SelectedDirectoryChangedEventArgs(_directory);
                SelectedDirectoryChanged.Raise(this, args);
            }
        }

        public event EventHandler<SelectedDirectoryChangedEventArgs> SelectedDirectoryChanged;

        public DirectoryService(IPathService pathService)
        {
            _pathService = pathService;
        }

        public bool CreateDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                return false;
            }

            try
            {
                Directory.CreateDirectory(directory);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public IReadOnlyCollection<DirectoryModel> GetDirectories(string directory)
        {
            var parentDirectory = new DirectoryInfo(directory).Parent;
            var parentDirectories = parentDirectory is null
                ? Enumerable.Empty<DirectoryModel>()
                : new[] {CreateParentDirectory(parentDirectory)};
            var directories = Directory
                .GetDirectories(directory)
                .Select(CreateFrom);

            return parentDirectories.Concat(directories).ToArray();
        }

        public bool CheckIfDirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }

        public string GetAppRootDirectory()
        {
            return _pathService.GetPathRoot(Directory.GetCurrentDirectory());
        }

        public IReadOnlyCollection<string> GetFilesRecursively(string directory)
        {
            return Directory
                .EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                .ToArray();
        }

        public void RemoveDirectoryRecursively(string directory)
        {
            Directory.Delete(directory, true);
        }

        private static DirectoryModel CreateFrom(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var directoryModel = new DirectoryModel
            {
                Name = directoryInfo.Name,
                FullPath = directoryInfo.FullName,
                LastModifiedDateTime = directoryInfo.LastWriteTime
            };

            return directoryModel;
        }

        private static DirectoryModel CreateParentDirectory(FileSystemInfo directoryInfo)
        {
            var directoryModel = new DirectoryModel
            {
                Name = ParentDirectoryName,
                FullPath = directoryInfo.FullName,
                LastModifiedDateTime = directoryInfo.LastWriteTime
            };

            return directoryModel;
        }
    }
}