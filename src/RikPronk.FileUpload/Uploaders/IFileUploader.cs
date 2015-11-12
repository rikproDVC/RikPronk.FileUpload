using System.IO;
using System.Threading.Tasks;

namespace RikPronk.FileUpload.Core
{
    public interface IFileUploader
    {
        void Upload();
        Task UploadAsync();
    }
}
