using RikPronk.FileUpload.Core;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RikPronk.FileUpload.Sample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BasicUpload()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> BasicUpload(BasicViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var fileCollection = UploadableFileCollection.From<UploadableFile>(viewModel.File);

            var uploader = new AzureBlobStorageUploader(fileCollection, MvcApplication.GetAzureStorageContainer("basic-container"));
            await uploader.UploadAsync();

            return View();
        }
    }
}