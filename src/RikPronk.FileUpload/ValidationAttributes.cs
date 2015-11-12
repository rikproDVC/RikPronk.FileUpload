using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using RikPronk.FileUpload.Extensions;

namespace RikPronk.FileUpload.ValidationAttributes
{
    public class IsFileTypesAttribute : ValidationAttribute
    {
        public IsFileTypesAttribute(string[] mimes)
        {
            _mimes = mimes.Any() ? mimes : new string[] { };
        }

        public override bool IsValid(object value)
        {
            if (value.GetType().IsArrayOf<HttpPostedFileWrapper>())
            {
                HttpPostedFileWrapper[] files = value as HttpPostedFileWrapper[];
                foreach (var file in files)
                {
                    if (file != null && _mimes.Any())
                    {
                        return _mimes.All(mime => file.ContentType.Contains(mime));
                    }
                }
            }
            else
            {
                HttpPostedFileWrapper file = value as HttpPostedFileWrapper;
                if (file != null && _mimes.Any())
                {
                    return _mimes.All(mime => file.ContentType.Contains(mime));
                }
            }
            

            return true;
        }

        private string[] _mimes;
    }

    public class FileSizeAttribute : ValidationAttribute
    {
        public FileSizeAttribute(int size)
        {
            _maxSize = size;
        }

        public override bool IsValid(object value)
        {
            if (value.GetType().IsArrayOf<HttpPostedFileWrapper>())
            {
                HttpPostedFileWrapper[] files = value as HttpPostedFileWrapper[];
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        return file.ContentLength <= _maxSize;
                    }
                }
            }
            else
            {
                HttpPostedFileWrapper file = value as HttpPostedFileWrapper;
                if (file != null)
                {
                    return file.ContentLength <= _maxSize;
                }
            }
            
            return true;
        }

        private int _maxSize;
    }

    
}
