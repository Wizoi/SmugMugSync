using Microsoft.VisualBasic.Logging;
using SmugMug.Net.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.IO.Abstractions;
using System.Security.RightsManagement;

namespace SmugMugCoreSync.Data
{
    public class SourceMediaData
    {

        private readonly IFileSystemInfo _sourceFileSystemInfo;
        private readonly IFileInfo _sourceFileInfo;
        private readonly IFileSystem _fileSystem;
        private string? _md5CacheData;


        public static SourceMediaData[] LoadFrom(IFileSystem fileSystem, IEnumerable<IFileSystemInfo> fileList)
        {
            List<SourceMediaData> list = new ();
            foreach (var f in fileList)
            {
                list.Add(new SourceMediaData (fileSystem: fileSystem, mediaFile: f));
            }
            return list.ToArray();
        }

        public SourceMediaData(IFileSystem fileSystem, IFileSystemInfo mediaFile) {
            _sourceFileSystemInfo = mediaFile;
            _sourceFileInfo = fileSystem.FileInfo.New(mediaFile.FullName);
            _fileSystem = fileSystem;
        }

        public IFileSystem FileSystem{
            get { return _fileSystem; }
        }

        public bool IsImageUpdateable()
        {
            return Path.GetExtension(_sourceFileSystemInfo.Name).ToUpper() != ".TIF";
        }

        public long FileLength {
            get { return _sourceFileInfo.Length; }
        }

        public string FileName {
            get { return _sourceFileSystemInfo.Name.ToUpper(); } 
        }

        public string FullFileName
        {
            get { return _sourceFileSystemInfo.FullName.ToUpper(); }
        }

        public string FileNameBase {
            get { 
                return Path.GetFileNameWithoutExtension(_sourceFileSystemInfo.Name).ToUpper(); 
            }
        }

        public DateTime LastWriteTime
        {
            get { return _sourceFileInfo.LastWriteTime;  }
        }

        /// <summary>
        /// This is generated, but cached since it only changes when the file would be changed, or  reloaded.
        /// </summary>
        /// <returns></returns>
        public async Task<string> LoadMd5Checksum()
        {
            _md5CacheData ??= await ImageUploaderService.GetMd5Checksum(_sourceFileInfo);
            return _md5CacheData;
        }
    }
}
