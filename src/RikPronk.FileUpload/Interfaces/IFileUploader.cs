using System.Collections;
using System.Threading.Tasks;

namespace RikPronk.FileUpload.Core
{
    public interface IFileUploader
    {
        void Upload();
    }

    public interface IAsyncFileUploader : IFileUploader
    {
        Task UploadAsync();
    }
}