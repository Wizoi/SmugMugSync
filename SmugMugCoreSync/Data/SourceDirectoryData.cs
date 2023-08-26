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
            AlbumId = 0;
            IsLinked = false;
            ReadIni();
        }

        public IDirectoryInfo DirectoryInfo { get; private set; }
        public string FolderName { get; set; }
        public int AlbumId { get; private   set; }
        public string AlbumKey { get; private set; }
        public string Path { get; private set; }

        public bool IsLinked { get; private set; }

        public void LinkToAlbum(int albumId, string albumKey)
        {
            if (LinkToAlbumInternal(albumId, albumKey))
            {
                SaveIni();
            }
        }

        public void UnlinkFromAlbum()
        {
            this.IsLinked = false;
            AlbumKey = string.Empty;
            AlbumId = 0;

            // Do not save the updated INI until it  is overwritten by  a valid album
        }

        private bool LinkToAlbumInternal(int albumId, string albumKey)
        {
            if (!IsLinked)
            {
                if (AlbumKey != albumKey)
                {
                    AlbumId = albumId;
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
                var albumIdNode = root.Element("albumId");
                var albumKeyNode = root.Element("albumKey");
                if (albumIdNode != null && albumKeyNode != null)
                {
                    _ = LinkToAlbumInternal(albumId: int.Parse(albumIdNode.Value), albumKey: albumKeyNode.Value);
                }
            }
        }

        public void SaveIni()
        {
            // Look for Smugmug ID Linkage File
            var smugMugIniPath = System.IO.Path.Combine(Path, META_FILE_SUFFIX);

            if (IsLinked)
            {
                XElement root = GenerateSyncXmNode();

                // If it is being remapped to a new folder (allbum removed to be resynced), then this may exist and should be removed.
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
            var elementAlbumId = new System.Xml.Linq.XElement("albumId") { Value = AlbumId.ToString() };
            var elementAlbumKey = new System.Xml.Linq.XElement("albumKey") { Value = AlbumKey };
            root.Add(elementAlbumId);
            root.Add(elementAlbumKey);
            return root;
        }
    }
}
