using Microsoft.VisualBasic.Logging;
using SmugMug.Net.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace SmugMugCoreSync.Data
{
    internal class SourceMediaData
    {

        private readonly FileSystemInfo _sourceFileSystemInfo;
        private readonly FileInfo _sourceFileInfo;
        private string? _md5CacheData;


        public static SourceMediaData[] LoadFrom(IEnumerable<FileSystemInfo> fileList)
        {
            List<SourceMediaData> list = new ();
            foreach (var f in fileList)
            {
                list.Add(new SourceMediaData (f));
            }
            return list.ToArray();
        }

        public SourceMediaData(FileSystemInfo mediaFile) {
            _sourceFileSystemInfo = mediaFile;
            _sourceFileInfo = new FileInfo(mediaFile.FullName);
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
        public string LoadMd5Checksum()
        {
            _md5CacheData ??= ImageUploaderService.GetMd5Checksum(_sourceFileInfo);
            return _md5CacheData;
        }
    }
}
