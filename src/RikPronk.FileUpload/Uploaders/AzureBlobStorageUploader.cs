using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using RikPronk.FileUpload.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RikPronk.FileUpload
{
    public class AzureBlobStorageUploader : FileUploaderBase, IFileUploader
    {
        public AzureBlobStorageUploader(UploadableFileCollection files, CloudBlobContainer container)
            : base(files)
        {
            _container = container;
            var existingFileNames = new List<string>(container.ListBlobs(null, false)
                .Select(n => WebUtility.UrlDecode(n.Uri.Segments.Last()))).ToArray();
            files.AssertAndResolveUniqueSaveNames(existingFileNames);
        }

        public static CloudBlobContainer GetBlobContainer(string containerName, string storageConnectionString)
        {
            CloudBlobContainer container;
            try
            {
                container = _getBlobStorageClient(storageConnectionString).GetContainerReference(_toUrlSlug(containerName));
                container.CreateIfNotExists();
                container.SetPermissions(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
            }
            catch(Exception e)
            {
                throw e;
            }
            
            return container;
        }

        public void Upload()
        {
            foreach (var file in _files)
            {
                using (var fileStream = file.FileStream)
                {
                    UploadStreamToStorage(fileStream, file.ContentType, file.SaveName);
                    fileStream.Dispose();
                }
            }
        }

        public async Task UploadAsync()
        {
            foreach(var file in _files)
            {
                using (var fileStream = file.FileStream)
                {
                    await UploadStreamToStorageAsync(fileStream, file.ContentType, file.SaveName);
                    fileStream.Dispose();
                }
            }
        }

        private static CloudBlobClient _getBlobStorageClient(string storageConnectionString)
        {
            if (_storageAccount == null)
            {
                _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }

            if (_blobClient == null)
            {
                _blobClient = _storageAccount.CreateCloudBlobClient();
            }

            return _blobClient;
        }

        private void UploadStreamToStorage(Stream stream, string contentType, string key)
        {
            var block = _container.GetBlockBlobReference(key);
            block.Properties.ContentType = contentType;
            block.UploadFromStream(stream);
        }

        private async Task UploadStreamToStorageAsync(Stream stream, string contentType, string key)
        {
            var block = _container.GetBlockBlobReference(key);
            block.Properties.ContentType = contentType;
            await block.UploadFromStreamAsync(stream);
        }

        private static string _toUrlSlug(string s)
        {
            return Regex.Replace(s, @"[^a-z0-9]+", "-", RegexOptions.IgnoreCase)
                .Trim(new char[] { '-' })
                .ToLower();
        }


        private static CloudStorageAccount _storageAccount;
        private static CloudBlobClient _blobClient;
        private CloudBlobContainer _container;
    }
}
