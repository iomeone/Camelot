﻿using System;
using System.IO;
using Camelot.Extensions;
using Camelot.FileSystemWatcherWrapper.Interfaces;

namespace Camelot.FileSystemWatcherWrapper.Implementations
{
    public class FileSystemWatcherWrapper : IFileSystemWatcherWrapper
    {
        private readonly FileSystemWatcher _fileSystemWatcher;

        public event EventHandler<FileSystemEventArgs> Created;

        public event EventHandler<FileSystemEventArgs> Changed;

        public event EventHandler<FileSystemEventArgs> Deleted;

        public event EventHandler<RenamedEventArgs> Renamed;

        public bool EnableRaisingEvents
        {
            get => _fileSystemWatcher.EnableRaisingEvents;
            set => _fileSystemWatcher.EnableRaisingEvents = value;
        }

        public FileSystemWatcherWrapper(FileSystemWatcher fileSystemWatcher)
        {
            _fileSystemWatcher = fileSystemWatcher;

            SubscribeToEvents();
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();

            _fileSystemWatcher.Dispose();
        }

        private void SubscribeToEvents()
        {
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;
        }

        private void UnsubscribeFromEvents()
        {
            _fileSystemWatcher.Created -= FileSystemWatcherOnCreated;
            _fileSystemWatcher.Changed -= FileSystemWatcherOnChanged;
            _fileSystemWatcher.Deleted -= FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed -= FileSystemWatcherOnRenamed;
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            Created.Raise(sender, e);
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            Changed.Raise(sender, e);
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            Deleted.Raise(sender, e);
        }

        private void FileSystemWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            Renamed.Raise(sender, e);
        }
    }
}