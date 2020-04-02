using System.IO.Compression;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;

namespace BlobStorage
{
    class Program
    {
        static void Main(string[] args)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureStorageConnectionString"));
            // Stored on App.config or Azure WebApp UI Settings. CloudConfigurationManager is able to retrieve from these two places) 
            //<appSettings>
            //  <add key="StorageConnectionString" value="DefaultEndpointsProtocol=https;AccountName=storageAccountName;AccountKey=storageAccountKey" />
            //</appSettings>

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("cirdevcontainer");
            // Retrieve reference to a blob name
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("CirZipWithImage.zip");

            // Save blob contents to a Memory Stream.
            using (var msZippedBlob = new MemoryStream())
            {
                blockBlob.DownloadToStream(msZippedBlob);
                using (ZipArchive zip = new ZipArchive(msZippedBlob))
                {
                    foreach (var file in zip.Entries)
                    {
                        var entry = file;
                        //CloudBlockBlob blockBlob = container.GetBlockBlobReference(directoryName);
                        CloudBlockBlob binaryDataBlockBlob = container.GetBlockBlobReference(file.ToString());
                        //byte[] blobBytes = Convert.FromBase64String(entry.ToString());
                        //blockBlob.UploadFromByteArray(blobBytes, 0, blobBytes.Length);
                        //return binaryDataBlockBlob;
                        // var entry = zip.Entries.First();
                        using (StreamReader sr = new StreamReader(entry.Open()))
                        {
                            string result = sr.ReadToEnd();
                            binaryDataBlockBlob.UploadText(result);
                            System.Console.WriteLine("# Characters: " + result.Length);
                        }
                    }
                }
            }


            string[] filesToZip = new string[] { @"C:\Users\gosig\AppData\Local\Temp\CIR.txt", @"C:\Users\gosig\AppData\Local\Temp\CIR1.txt" };
            FileInfo zipFile = CreateZip(@"C:\Users\gosig\AppData\Local\Temp\myzip.zip", filesToZip);

            UploadToBlobStorage(zipFile, "cirdevcontainer");

            //ArrayList array = new ArrayList(); //{ @"c:\temp\table.txt", @"c:\temp\bcp.fmt" };
            //array.Add(container.Uri + "/" + binaryDataFileName);
            //string result = Path.GetTempPath();

            //using (FileStream f1 = new FileStream(@"C:\Official\Vestas_Gearbox_Version1.zip", FileMode.Create))
            //using (ZipArchive arch = new ZipArchive(f1, ZipArchiveMode.Create))
            //{
            //    arch.CreateEntryFromFile(@"C:\Users\gosig\AppData\Local\Temp\CIR.txt", "Test.txt");
            //}
            //string zipPath = @"C:\Official\Vestas_Gearbox_Version1.zip";
            //string extractPath = @"C:\Official\Extract";
            //string newFile = @"C:\Official\Vestas_Gearbox_Version1.json";
            //string zipFileName = @"C:\Users\gosig\AppData\Local\Temp\2d2a6db9-a0e6-400d-b7ea-c46513850b66.txt";
            //string fileNameOnly = "e1354819-3bf1-48c5-9c54-dd7b43dce3ca.txt";
            //array.Add(zipFileName);

            //using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
            //{
            //    archive.CreateEntryFromFile(newFile, "NewEntry.txt");
            //    archive.ExtractToDirectory(extractPath);
            //}

            //FileInfo zipFile = new FileInfo(zipFileName);

            //FileStream fs = zipFile.Create();
            ////fs.Close();
            //using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
            //{
            //    fs.Close();
            //    foreach (string fileName in array)
            //    {
            //        zip.CreateEntryFromFile(fileName, fileNameOnly);

            //    }

            //}
            //Program.CompressFile();
            //return zipFile;
        }
        private static void UploadToBlobStorage(FileInfo zipFile,  string blobContainerName)
        {
            // Connect to the storage account's blob endpoint 
            CloudStorageAccount account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureStorageConnectionString"));
            CloudBlobClient client = account.CreateCloudBlobClient();

            // Create the blob storage container 
            CloudBlobContainer container = client.GetContainerReference(blobContainerName);
            container.CreateIfNotExists();

            // Create the blob in the container 
            CloudBlockBlob blob = container.GetBlockBlobReference(zipFile.Name);

            // Upload the zip and store it in the blob 
            using (FileStream fs = zipFile.OpenRead())
                blob.UploadFromStream(fs);
        }
        private static FileInfo CreateZip(string zipFileName, string[] filesToZip)
        {
            FileInfo zipFile = new FileInfo(zipFileName);
            FileStream fs = zipFile.Create();
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                foreach (string fileName in filesToZip)
                    zip.CreateEntryFromFile(fileName, Path.GetFileName(fileName));
            }

            return zipFile;
        }
        public static string CompressFile()
        {
            string path = @"C:\Users\gosig\AppData\Local\Temp\2d2a6db9-a0e6-400d-b7ea-c46513850b66.txt";
            string relativeTodir = @"C:\Users\gosig\AppData\Local\Temp\e1354819-3bf1-48c5-9c54-dd7b43dce3ca.txt";
            string zipFilePath = Path.GetTempFileName();
            using (FileStream zipStream = new FileStream(zipFilePath, FileMode.Create))
            using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                string entryName = Path.GetFileName(path);
                //if (!string.IsNullOrWhiteSpace(relativeTodir))
                //{
                //    entryName = path.Re(relativeTodir).Replace('\\', '/');
                //}
                archive.CreateEntryFromFile(path, entryName);
            }
            return zipFilePath;
        }
    }
}
