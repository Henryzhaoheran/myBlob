using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.IO;

namespace StorDemo1
{
    class Program
    {
        static void Main(string[] args)
        {
            string connString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string localfolder = ConfigurationManager.AppSettings["sourcefolder"];
            string destContainer = "mybooks"; //ConfigurationManager.AppSettings["destContainer"];

            //Get a reference to the storage account
            Console.WriteLine(@"Connecting to storage account");
            CloudStorageAccount sa = CloudStorageAccount.Parse(connString);
            CloudBlobClient bc = sa.CreateCloudBlobClient();

            //Get a reference to the container (create it if necessary)
            Console.WriteLine(@"Getting reference to container");
            CloudBlobContainer container = bc.GetContainerReference(destContainer);

            //Create container if it doesn't exist
            container.CreateIfNotExists();

            //upload files
            Console.ReadKey();
            string[] fileEntries = Directory.GetFiles(localfolder);
            foreach (string filePath in fileEntries)
            {
                //Get the date to use with the key
                // string key = DateTime.UtcNow.ToString("yyyy-MM-dd-HH:mm:ss") + "-" + Path.GetFileName(filePath);
                string key = Path.GetFileName(filePath);

                UploadBlob(container, key, filePath, false);
            }
            Console.WriteLine(@"Upload processing complete, listing all the blobs in the destination container");


            // List all the blobs
            foreach (IListBlobItem item in container.ListBlobs(useFlatBlobListing: true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;
                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);
                }
                //else if (item.GetType() == typeof(CloudBlobDirectory))
                //{
                //    CloudBlobDirectory directory = (CloudBlobDirectory)item;
                //    Console.WriteLine("Directory: {0}", directory.Uri);
                //}
            }

            ListBlobsFromServiceClientAsync(bc, "demoblob/");

            //// List pic1.bmp properties
            //CloudBlobContainer container2 = bc.GetContainerReference(destContainer);
            //Console.WriteLine("LastModifiedUTC: " + container2.Properties.LastModified);
            //Console.WriteLine("ETag: " + container2.Properties.ETag);

            Console.WriteLine(@"List container processing complete. Press any key to exit...");
            Console.ReadKey();
        }

        static void UploadBlob(CloudBlobContainer container, string key, string fileName, bool deleteAfter)
        {
            Console.WriteLine(@"uploading file to container: key=" + key + " source file=" + fileName);

            // Get a blob reference to write this file to
            CloudBlockBlob b = container.GetBlockBlobReference(key);

            // write the file
            using (var fs = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                b.UploadFromStream(fs);
            }

            // if delete of file is requested, then do that
            if (deleteAfter)
                File.Delete(fileName);
        }

        private static async Task ListBlobsFromServiceClientAsync(CloudBlobClient blobClient, string prefix)
        {
            Console.WriteLine("List blobs by prefix. Prefix must include container name:");

            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            try
            {
                do
                {
                    // The prefix is required when listing blobs from the service client. The prefix must include
                    // the container name.
                    resultSegment = await blobClient.ListBlobsSegmentedAsync(prefix, continuationToken);
                    foreach (var blob in resultSegment.Results)
                    {
                        Console.WriteLine("\tBlob:" + blob.Uri);
                    }

                    Console.WriteLine();

                    // Get the continuation token.
                    continuationToken = resultSegment.ContinuationToken;

                } while (continuationToken != null);

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}