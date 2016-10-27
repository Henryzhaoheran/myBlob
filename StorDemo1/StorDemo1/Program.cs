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
            string destContainer = ConfigurationManager.AppSettings["destContainer"];

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
            string[] fileEntries = Directory.GetFiles(localfolder);
            foreach (string filePath in fileEntries)
            {
                //Get the date to use with the key
                string key = DateTime.UtcNow.ToString("yyyy-MM-dd-HH:mm:ss") + "-" + Path.GetFileName(filePath);

                UploadBlob(container, key, filePath, false);
            }

            Console.WriteLine(@"Upload processing complete. Press any key to exit...");
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
    }
}
