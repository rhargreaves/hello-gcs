using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace HelloGcs
{
    // Based on: https://cloud.google.com/storage/docs/access-control?hl=en#signing-code-csharp
    public class GcsSigner
    {
        private const string P12_PASSWORD = "notasecret";

        private readonly string _credsFilePath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        private readonly string _serviceAccountEmail;
        private readonly X509Certificate2 _key;

        public GcsSigner(string serviceAccountEmail)
        {
            _serviceAccountEmail = serviceAccountEmail;
            _key = new X509Certificate2(_credsFilePath, P12_PASSWORD);
        }

        public Uri GetObjectUrl(string name, string bucketName, TimeSpan expiration)
        {
            var url = GetSigningURL("GET", bucketName, name, expiration);
            return new Uri(url);
        }

        private string GetSigningURL(string verb, string bucketName, string name, TimeSpan expires)
        {
            var secondsSinceEpoch = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var expiration = Convert.ToInt64(secondsSinceEpoch + expires.TotalSeconds);
            string urlSignature = SignString(
                string.Format("{0}\n\n\n{1}\n/{2}/{3}",
                    verb,
                    expiration,
                    bucketName,
                    name
                    )
                );
            string signed_url = string.Format(
                "https://storage.googleapis.com/{0}/{1}?GoogleAccessId={2}&Expires={3}&Signature={4}",
                bucketName,
                name,
                _serviceAccountEmail,
                expiration,
                HttpUtility.UrlEncode(urlSignature)
                );
            return signed_url;
        }

        private string SignString(string stringToSign)
        {
            CspParameters cp = new CspParameters(24, "Microsoft Enhanced RSA and AES Cryptographic Provider",
                ((RSACryptoServiceProvider) _key.PrivateKey).CspKeyContainerInfo.KeyContainerName);
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cp);
            byte[] buffer = Encoding.UTF8.GetBytes(stringToSign);
            byte[] signature = provider.SignData(buffer, "SHA256");
            return Convert.ToBase64String(signature);
        }
    }
}