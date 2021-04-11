using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCRBot.Services.AzureStorage
{
    public class AzureStorageService: IAzureStorageService
    {
        public async Task<string> Execute(byte[] data, string type)
        {
            string connectionString = "INRESAR TU CADENA DE CONEXIÓN";
            var account = CloudStorageAccount.Parse(connectionString);

            var client = account.CreateCloudBlobClient();
            string containerName = "images";

            var container = client.GetContainerReference(containerName);
            string fileName = $"{Guid.NewGuid()}.{type}";

            var validateType = MimeTypes.GetMimeType(fileName);

            if (await container.CreateIfNotExistsAsync())
                await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob});

            var blob = container.GetBlockBlobReference(fileName);
            blob.Properties.ContentType = validateType;
            await blob.UploadFromByteArrayAsync(data, 0, data.Length);

            return blob.Uri.AbsoluteUri;

        }
    }
}
