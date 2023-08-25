using SmugMugCoreSync.Configuration;
using SmugMugCoreSync.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmugMugCoreSync.Repositories
{
    internal class SourceFolderRepository
    {
        private readonly FolderSyncPathsConfig _folderSyncPathsConfig;
        private readonly string _rootSyncFolder;
        private readonly ConcurrentDictionary<string, SourceDirectoryData>  _sourceLinkedFolders = new ();
        private readonly ConcurrentDictionary<string, SourceDirectoryData>  _sourceUnlinkedFolders = new ();
        private readonly HashSet<string> _directoriesToSkip;
        private readonly HashSet<string> _extensionsToSkip;
        private readonly Func<IFileSystemInfo, bool> _filterMediaFiles;
        private IFileSystem _filesystem;

        public SourceFolderRepository(IFileSystem fileSystem, FolderSyncPathsConfig folderConfig) 
        {
            _folderSyncPathsConfig = folderConfig;
            _filesystem = fileSystem;

            // Set the root folder
            if (Directory.Exists(folderConfig.RootLocal))
                _rootSyncFolder = folderConfig.RootLocal;
            else if (Directory.Exists(folderConfig.RootRemote))
                _rootSyncFolder = folderConfig.RootRemote;
            else
                throw new Exception("Invalid local or remote folders for reading source fiels.");

            // Load the Hashtables for skipping folders and extensions. Capitalize these and add the period for the extension for 1:1 with a file extension
            _directoriesToSkip = folderConfig.FolderNamesToSkip.Select(x => x.ToUpper()).ToHashSet();
            _extensionsToSkip = folderConfig.ExtensionsToSkip.Select(x => "." + x.ToUpper()).ToHashSet();

            // Use a standard filter for querying the media files
            _filterMediaFiles = x => !_extensionsToSkip.Contains(x.Extension.ToUpper())
                && !x.Extension.ToUpper().Contains(SourceDirectoryData.META_FILE_SUFFIX)
                && !x.Attributes.HasFlag(FileAttributes.Hidden)
                && !x.Attributes.HasFlag(FileAttributes.Directory)
                && x.Extension != x.Name;
        }

        public void PopulateSourceFoldersAndFiles()
        {
            Trace.Write("Populating Source Folder Details...");
            this.LoadFoldersAndFiles(rootSyncFolder: _rootSyncFolder);
            //Trace.WriteLine($"Stopwatch: {sw.ElapsedMilliseconds}, found {_sourceLinkedFolders.Count }");
            Trace.WriteLine($"{_sourceLinkedFolders.Count} existing found, {_sourceUnlinkedFolders.Count} new found.");
        }

        internal SourceDirectoryData? RetrieveLinkedFolderByKey(string albumKey)
        {
            if (_sourceLinkedFolders.ContainsKey(albumKey))
                return _sourceLinkedFolders[albumKey];

            return null;
        }

        internal IEnumerable<SourceDirectoryData> RetrieveUnlinkedFolders()
        {
            return _sourceUnlinkedFolders.Values.OrderBy(o => o.FolderName);
        }
        internal IEnumerable<SourceDirectoryData> RetrieveLinkedFolders()
        {
            return _sourceLinkedFolders.Values.OrderBy(o => o.FolderName);
        }

        public SourceMediaData[] LoadFolderMediaFiles(SourceDirectoryData sourceDirectory)
        {
            return SourceMediaData.LoadFrom(fileSystem: _filesystem, fileList: sourceDirectory.DirectoryInfo.GetFileSystemInfos().AsQueryable().Where(_filterMediaFiles));
        }

        public void LoadFoldersAndFiles(string rootSyncFolder)
        {
            var currentDirectory = _filesystem.DirectoryInfo.New(rootSyncFolder);
            if (_directoriesToSkip.Contains(currentDirectory.Name.ToUpper()))
                return;

            // We ony need one file to to determine if this folder is important
            bool filesFound = currentDirectory.GetFileSystemInfos().AsQueryable().Where(_filterMediaFiles).Any();

            if (filesFound)
            {
                if (_folderSyncPathsConfig.FilterFolderName.Any() && currentDirectory.FullName.Contains(_folderSyncPathsConfig.FilterFolderName))
                {
                    var data = new SmugMugCoreSync.Data.SourceDirectoryData(fileSystem:_filesystem, directory: currentDirectory);
                    if (data.IsLinked)
                        _sourceLinkedFolders.TryAdd(data.AlbumKey, data);
                    else
                        _sourceUnlinkedFolders.TryAdd(data.AlbumKey, data);
                }
            }
            else // When there are no folders, recursively loop through the subfolders
            {
                // If there are a  lot of directories, process them in parallel 
                var directories = Directory.GetDirectories(rootSyncFolder).ToList();
                if (directories.Count > 10)
                {
                    directories.AsParallel().ForAll(x => LoadFoldersAndFiles(x));                    
                }
                else
                {
                    directories.ForEach(x => LoadFoldersAndFiles(x));
                }
            }
        }

        internal void AddNewLinkedFolder(SourceDirectoryData newLinkedFolder)
        {
            _sourceUnlinkedFolders.Remove(newLinkedFolder.AlbumKey, out _);
            _sourceLinkedFolders.TryAdd(newLinkedFolder.AlbumKey, newLinkedFolder);
        }

        internal void RemoveLinkedFolder(SourceDirectoryData newLinkedFolder)
        {
            _sourceLinkedFolders.Remove(newLinkedFolder.AlbumKey, out _);
            _sourceUnlinkedFolders.TryAdd(newLinkedFolder.AlbumKey, newLinkedFolder);
        }

    }
}
