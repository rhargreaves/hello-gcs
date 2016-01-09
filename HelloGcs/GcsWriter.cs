using System;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Crc32C;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Discovery;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Storage.v1;
using Google.Apis.Upload;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace HelloGcs
{
    public class GcsWriter
    {
        private IConfigurableHttpClientInitializer GetApplicationDefaultCredentials()
        {
            // Loads credentials from file pointed to by GOOGLE_APPLICATION_CREDENTIALS environmental variable.
            var credential = GoogleCredential.GetApplicationDefaultAsync().Result;
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(StorageService.Scope.DevstorageReadWrite);
            }
            return credential;
        }

        public void ReadObject(Stream destination, string name, string bucketName)
        {
            var service = CreateStorageService();
            var get = service.Objects.Get(bucketName, name);
            get.Download(destination);
        }

        public bool WriteStream(Stream stream, string name, string bucketName)
        {
            var hash = CalculateCrc32c(stream);
            var service = CreateStorageService();

            var fileobj = new Google.Apis.Storage.v1.Data.Object()
                {
                    Name = name, Crc32c = hash 
            };

            var insert = service.Objects.Insert(fileobj, bucketName, stream, "text/plain");
            var upload = insert.Upload();
            if (upload.Status == UploadStatus.Failed)
                throw upload.Exception;
            return true;
        }

        private StorageService CreateStorageService()
        {
            StorageService service = new StorageService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetApplicationDefaultCredentials(),
                    ApplicationName = "GCS Test",
                });
            return service;
        }

        private static string CalculateCrc32c(Stream stream)
        {
            var algorithm = new Crc32CAlgorithm();
            var hash = algorithm.ComputeHash(stream);
            stream.Seek(0, SeekOrigin.Begin);
            Array.Reverse(hash);
            return Convert.ToBase64String(hash);
        }
    }
}