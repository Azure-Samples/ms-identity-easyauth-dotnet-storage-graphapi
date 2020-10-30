using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using Azure.Identity;

namespace WebApp_EasyAuth_DotNet.Helpers
{
    public static class StorageHelper
    {
        static public async Task UploadBlob(string accountName, string containerName, string blobName, string blobContents)
        {
            // Construct the blob container endpoint from the arguments.
            string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
                                                        accountName,
                                                        containerName);

            // Get a credential and create a client object for the blob container.
            BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                            new DefaultAzureCredential());


            try
            {
                // Create the container if it does not exist.
                await containerClient.CreateIfNotExistsAsync();

                // Upload text to a new block blob.
                byte[] byteArray = Encoding.ASCII.GetBytes(blobContents);

                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    await containerClient.UploadBlobAsync(blobName, stream);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        static public async Task DeleteBlob(string accountName, string containerName, string blobName)
        {

            // Construct the blob container endpoint from the arguments.
            string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
                                                        accountName,
                                                        containerName);

            // Get a credential and create a client object for the blob container.
            BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                            new DefaultAzureCredential());

            try
            {

                var blob = containerClient.GetBlobClient(blobName);
                blob.DeleteIfExists();
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        static public async Task<List<CommentBlobDTO>> GetBlobs(string accountName, string containerName)
        {
            List<CommentBlobDTO> blobs = new List<CommentBlobDTO>();

            // Construct the blob container endpoint from the arguments.
            string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
                                                        accountName,
                                                        containerName);

            // Get a credential and create a client object for the blob container.
            BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                            new DefaultAzureCredential());

            await containerClient.CreateIfNotExistsAsync();

            try
            {
                // List all the blobs                
                await foreach (BlobItem blob in containerClient.GetBlobsAsync())
                {
                    // Download the blob's contents and save it to a file
                    // Get a reference to a blob named "sample-file"
                    BlobClient blobClient = containerClient.GetBlobClient(blob.Name);
                    BlobDownloadInfo download = await blobClient.DownloadAsync();

                    byte[] bytes;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await download.Content.CopyToAsync(stream);
                        bytes = stream.ToArray();

                    }

                    String txt = new String(Encoding.ASCII.GetString(bytes));

                    CommentBlobDTO blobDTO;
                    blobDTO.Name = blob.Name;
                    blobDTO.Contents = txt;
                    blobs.Add(blobDTO);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return blobs;

        }
    }

    public struct CommentBlobDTO
    {
        public string Name;
        public string Contents;

    }
}
