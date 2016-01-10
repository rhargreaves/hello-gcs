using System;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HelloGcs
{
    // Based on: https://cloud.google.com/storage/docs/access-control?hl=en#signing-code-csharp
    public class GcsSignedUrlGenerator
    {
        private const string P12_PASSWORD = "notasecret";
        private readonly X509Certificate2 _key;
        private readonly string _serviceAccountEmail;

        public GcsSignedUrlGenerator(string serviceAccountEmail, string p12fileName)
        {
            _serviceAccountEmail = serviceAccountEmail;
            _key = new X509Certificate2(p12fileName, P12_PASSWORD);
        }

        public Uri CreateGetObjectRequestUrl(string name, string bucketName, TimeSpan expiration)
        {
            var url = CreateSignedUrl("GET", bucketName, name, expiration);
            return new Uri(url);
        }

        private string CreateSignedUrl(string verb, string bucketName, string name, TimeSpan expires)
        {
            var secondsSinceEpoch = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var expiration = Convert.ToInt64(secondsSinceEpoch + expires.TotalSeconds);
            var urlSignature = SignString(
                string.Format("{0}\n\n\n{1}\n/{2}/{3}",
                    verb,
                    expiration,
                    bucketName,
                    name
                    )
                );
            var signedUrl = string.Format(
                "https://storage.googleapis.com/{0}/{1}?GoogleAccessId={2}&Expires={3}&Signature={4}",
                bucketName,
                name,
                _serviceAccountEmail,
                expiration,
                WebUtility.UrlEncode(urlSignature)
                );
            return signedUrl;
        }

        private string SignString(string stringToSign)
        {
            var cp = new CspParameters(24, "Microsoft Enhanced RSA and AES Cryptographic Provider",
                ((RSACryptoServiceProvider) _key.PrivateKey).CspKeyContainerInfo.KeyContainerName);
            var provider = new RSACryptoServiceProvider(cp);
            var buffer = Encoding.UTF8.GetBytes(stringToSign);
            var signature = provider.SignData(buffer, "SHA256");
            return Convert.ToBase64String(signature);
        }
    }
}