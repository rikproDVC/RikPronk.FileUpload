using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace RikPronk.FileUpload.Core
{
    public class UploadableFile : IUploadableFile
    {
        public Stream FileStream { get; private set; }
        public string FileName { get; private set; }
        public string SaveName { get; set; }
        public int ContentLength { get; private set; }
        public string ContentType { get; private set; }
        
        public UploadableFile(HttpPostedFileWrapper httpFile)
        {
            if (httpFile == null)
            {
                throw new NullReferenceException();
            }
            FileStream = httpFile.InputStream;
            FileName = httpFile.FileName;
            SaveName = httpFile.FileName;
            ContentLength = httpFile.ContentLength;
            ContentType = httpFile.ContentType;
        }
        
        public UploadableFile(HttpPostedFileWrapper httpFile, string saveName)
        {
            if (httpFile == null)
            {
                throw new NullReferenceException();
            }
            FileStream = httpFile.InputStream;
            FileName = httpFile.FileName;
            SaveName = saveName;
            ContentLength = httpFile.ContentLength;
            ContentType = httpFile.ContentType;
        }

        public bool IsSize(int maxSize)
        {
            return ContentLength <= maxSize;
        }

        public bool IsContentType(string[] types)
        {
            if (types.Any())
            {
                return types.All(mime => ContentType.Contains(mime));
            }

            return true;
        }

        public byte[] GetMD5()
        {
            if (md5 == null)
            {
                md5 = new MD5Cng().ComputeHash(FileStream);
                FileStream.Position = 0;
            }
            
            return md5;
        }

        public byte[] GetSHA1()
        {
            if (sha1 == null)
            {
                sha1 = new SHA1Cng().ComputeHash(FileStream);
                FileStream.Position = 0;
            }

            return sha1;
        }

        private byte[] md5;
        private byte[] sha1;
    }
}
