using System.Configuration;
using System.IO;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LDevelopment.Helpers
{
    public class AzureBlobHelper
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly string _containerName;

        public AzureBlobHelper()
        {
            _storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobConnection"]);
            _containerName = ConfigurationManager.AppSettings["BlobContainer"];
        }

        public CloudBlockBlob UploadBlob(string fileName, Stream stream)
        {
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(_containerName.ToLower());
            var blockBlob = container.GetBlockBlobReference(fileName);

            blockBlob.UploadFromStream(stream);

            return blockBlob;
        }

        public static string UploadPhoto(HttpPostedFileBase image)
        {
            var utility = new AzureBlobHelper();

            var result = utility.UploadBlob(image.FileName, image.InputStream);

            return result != null ? result.Uri.ToString() : string.Empty;
        }
    }
}