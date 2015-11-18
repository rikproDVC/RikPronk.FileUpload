using System.IO;

namespace RikPronk.FileUpload.Core
{
    public interface IUploadableFile
    {
        /// <summary>
        /// The stream of the uploaded file
        /// </summary>
        Stream FileStream { get; }

        /// <summary>
        /// The name of the file. Note that this is the name as saved on the end user's system, not neccessarily the name the file will be saved as.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// The name that the uploaded file will be saved as. Note that this is not neccessarily the name by which registered on the end user's system
        /// </summary>
        string SaveName { get; set; }

        /// <summary>
        /// The length of the uploaded file in bytes
        /// </summary>
        int ContentLength { get; }

        /// <summary>
        /// The content type of the uploaded file
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the file extension
        /// </summary>
        string Extension { get; }
    }
}
