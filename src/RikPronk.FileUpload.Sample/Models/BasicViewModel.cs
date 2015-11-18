using System.ComponentModel.DataAnnotations;
using System.Web;

namespace RikPronk.FileUpload.Sample
{
    public class BasicViewModel
    {
        [Required]
        public HttpPostedFileBase[] File { get; set; }
    }
}
