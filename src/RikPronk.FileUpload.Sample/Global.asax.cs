using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace RikPronk.FileUpload.Sample 
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        public static CloudBlobContainer GetAzureStorageContainer(string containerName)
        {
            return AzureBlobStorageUploader.GetBlobContainer(containerName, ConfigurationManager.AppSettings["StorageConnectionString"]);
        }
    }
}
