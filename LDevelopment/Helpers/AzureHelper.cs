using System.Configuration;
using System.IO;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LDevelopment.Helpers
{
    public class AzureHelper
    {
        public CloudBlockBlob UploadBlob(string containerName, string fileName, Stream stream)
        {
            var blobConnection = ConfigurationManager.AppSettings["BlobConnection"];

            if (string.IsNullOrEmpty(blobConnection) || string.IsNullOrEmpty(containerName))
            {
                return null;
            }

            var storageAccount = CloudStorageAccount.Parse(blobConnection);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName.ToLower());
            var blob = container.GetBlockBlobReference(fileName);

            blob.UploadFromStream(stream);

            return blob;
        }

        public string UploadPhoto(HttpPostedFileBase image)
        {
            var container = ConfigurationManager.AppSettings["BlobContainer"];
            var result = UploadBlob(container, image.FileName, image.InputStream);

            return result != null ? result.Uri.ToString() : string.Empty;
        }
    }
}