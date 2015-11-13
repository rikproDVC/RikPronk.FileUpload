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
        public string Extension { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadableFile"/> class.
        /// </summary>
        /// <param name="httpFile">The file to upload</param>
        /// <exception cref="NullReferenceException"></exception>
        public UploadableFile(HttpPostedFileBase httpFile)
            : this(httpFile, httpFile.FileName)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadableFile" /> class.
        /// </summary>
        /// <param name="httpFile">The file to upload</param>
        /// <param name="saveName">Name the file will be saved as on disk</param>
        /// <exception cref="NullReferenceException"></exception>
        public UploadableFile(HttpPostedFileBase httpFile, string saveName)
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
            Extension = Path.GetExtension(httpFile.FileName);
        }

        /// <summary>
        /// Determines whether the file is smaller than the given size
        /// </summary>
        /// <param name="maxSize">The maximum size in bytes</param>
        /// <returns></returns>
        public bool IsSize(int maxSize)
        {
            return ContentLength <= maxSize;
        }

        /// <summary>
        /// Determines whether he file is of one of the specified content types
        /// </summary>
        /// <param name="types">Array of content types to check against</param>
        /// <returns>True if the content type matches. Returns false when supplied with an empty array</returns>
        public bool IsContentType(string[] types)
        {
            if (types.Any())
            {
                return types.All(mime => ContentType.Contains(mime));
            }

            return false;
        }

        /// <summary>
        /// Determines whether the file has one of the specified extensions.
        /// </summary>
        /// <param name="extensions">Array of extensions (including period) to check against</param>
        /// <returns>True if the extension matches. Returns false when supplied with an empty array</returns>
        public bool HasExtension(string[] extensions)
        {
            if (extensions.Any())
            {
                return extensions.All(ext => Extension == ext);
            }

            return false;
        }

        /// <summary>
        /// Gets the md5 hash of the file.
        /// </summary>
        /// <returns></returns>
        public byte[] GetMD5()
        {
            if (md5 == null)
            {
                md5 = new MD5Cng().ComputeHash(FileStream);
                FileStream.Position = 0;
            }
            
            return md5;
        }

        /// <summary>
        /// Gets the sha1 hash of the file.
        /// </summary>
        /// <returns></returns>
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
