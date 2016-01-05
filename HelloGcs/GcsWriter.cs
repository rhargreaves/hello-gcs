using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Storage.v1;

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

        public bool WriteStream(Stream stream, string name, string bucketName)
        {
            StorageService service = new StorageService(
               new BaseClientService.Initializer()
               {
                   HttpClientInitializer = GetApplicationDefaultCredentials(),
                   ApplicationName = "GCS Test",
               });

            var fileobj = new Google.Apis.Storage.v1.Data.Object()
                {
                    Name = name
                };

            service.Objects.Insert(fileobj, bucketName, stream, "text/plain").Upload();


            return true;
        }
    }
}