using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePut
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage: AzurePut.exe <container> <file> [<file> ...]");
                return;
            }

            var connStr = System.Environment.GetEnvironmentVariable("StorageConnectionString");
            if (connStr == null)
            {
                System.Console.WriteLine("Could not find StorageConnectionString environment variable.");
                return;
            }

            CloudStorageAccount storageAccount;
            CloudBlobClient blobClient;
            try
            {
                storageAccount = CloudStorageAccount.Parse(connStr);
                blobClient = storageAccount.CreateCloudBlobClient();
            }
            catch
            {
                System.Console.WriteLine("Could not find connect to Azure with the provided StorageConnectionString environment variable.");
                return;
            }

            var container = blobClient.GetContainerReference(args[0]);
            if (!container.Exists())
            {
                System.Console.WriteLine(String.Format("WARNING: container {0} does not exist. Creating", container.Name));
                container.Create();
            }

            foreach (var path in args.Skip(1))
            {
                //var file = File.OpenRead(path);
                System.Console.WriteLine(path);
                var filename = Path.GetFileName(path);
                var blob = container.GetBlockBlobReference(filename);
                if (blob.Exists())
                {
                    System.Console.WriteLine(String.Format("Blob already exists for {0}. Skipping", blob.Name));
                    return;
                }
                blob.UploadFromFile(path, FileMode.Open);
            }
        }
    }
}
