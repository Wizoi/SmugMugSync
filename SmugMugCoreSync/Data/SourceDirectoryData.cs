using System.IO;
using System.Windows;
using System.Xml;
using System.IO.Abstractions;
using System.Xml.Linq;
using SmugMugCoreSync.Utility;

namespace SmugMugCoreSync.Data
{
    public class SourceDirectoryData
    {
        public const string META_FILE_SUFFIX = ".SMUGMUG.INI";
        private IFileSystem _fileSystem;
        private XmlSystem _xmlSystem;

        public SourceDirectoryData(IFileSystem fileSystem, XmlSystem xmlSystem, IDirectoryInfo directory) : base()
        {
            this.DirectoryInfo = directory;

            _fileSystem = fileSystem;
            _xmlSystem = xmlSystem;
            FolderName = System.IO.Path.GetFileName(directory.FullName);
            Path = directory.FullName;
            AlbumKey = string.Empty;
            IsLinked = false;
            ReadIni();
        }

        public IDirectoryInfo DirectoryInfo { get; private set; }
        public string FolderName { get; set; }
        public string AlbumKey { get; private set; }
        public string Path { get; private set; }

        public bool IsLinked { get; private set; }

        public virtual void LinkToAlbum(string albumKey)
        {
            if (LinkToAlbumInternal(albumKey))
            {
                SaveIni();
            }
        }

        public virtual void UnlinkFromAlbum()
        {
            this.IsLinked = false;
            AlbumKey = string.Empty;

            // Do not save the updated INI until it  is overwritten by  a valid album
        }

        private bool LinkToAlbumInternal(string albumKey)
        {
            if (!IsLinked)
            {
                if (AlbumKey != albumKey)
                {
                    AlbumKey = albumKey;

                    // Save the new  settings to the INI for the folder.
                    IsLinked = true;
                    return true;
                }
                else
                    throw new Exception($"This directory is already linked to album key: {AlbumKey}");
            }

            return false;
        }

        private void ReadIni()
        {
            // Look for Smugmug ID Linkage File
            var smugMugIniPath = System.IO.Path.Combine(Path, META_FILE_SUFFIX);
            if (_fileSystem.File.Exists(smugMugIniPath))
            {
                string xmlData = _fileSystem.File.ReadAllText(smugMugIniPath);
                System.Xml.Linq.XElement root = System.Xml.Linq.XElement.Parse(xmlData);
                var albumKeyNode = root.Element("albumKey");
                if (albumKeyNode != null)
                {
                    _ = LinkToAlbumInternal(albumKey: albumKeyNode.Value);
                }
            }
        }

        public virtual void SaveIni()
        {
            // Look for Smugmug ID Linkage File
            var smugMugIniPath = System.IO.Path.Combine(Path, META_FILE_SUFFIX);

            if (IsLinked)
            {
                XElement root = GenerateSyncXmNode();

                // If it is being remapped to a new folder (album removed to be resynced), then this may exist and should be removed.
                if (_fileSystem.File.Exists(smugMugIniPath))
                    _fileSystem.File.Delete(smugMugIniPath);

                _xmlSystem.OutputXmlToFile(smugMugIniPath, root);
                _fileSystem.File.SetAttributes(smugMugIniPath, FileAttributes.Hidden);
            }
            else
                throw new Exception("Cannot save an INI, there is no album currently linked.");
        }

        private XElement GenerateSyncXmNode()
        {
            System.Xml.Linq.XElement root = System.Xml.Linq.XElement.Parse("<smugMugSyncData />");
            var elementAlbumKey = new System.Xml.Linq.XElement("albumKey") { Value = AlbumKey };
            root.Add(elementAlbumKey);
            return root;
        }
    }
}
