using System.Collections.Generic;

namespace RikPronk.FileUpload.Core
{
    public abstract class FileUploaderBase
    {
        public FileUploaderBase(UploadableFileCollection files)
        {
            _files = files;
        }

        protected UploadableFileCollection _files;
    }
}
