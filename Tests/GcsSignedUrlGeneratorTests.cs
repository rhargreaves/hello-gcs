using System;
using HelloGcs;
using NUnit.Framework;

namespace HelloGcs.Tests
{
    public class GcsSignedUrlGeneratorTests
    {
        [Test]
        public void CreateGetObjectRequestUrl_returns_valid_url()
        {
            var signer = new GcsSignedUrlGenerator("test-bucket-account@fluid-advantage-118120.iam.gserviceaccount.com",
                Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            var url = signer.CreateGetObjectRequestUrl("bar", "robert-hargreaves-test2", TimeSpan.FromMinutes(30));
            Assert.That(url.AbsoluteUri, 
                Does.StartWith("https://storage.googleapis.com/robert-hargreaves-test2/" +
                    "bar?GoogleAccessId=test-bucket-account@fluid-advantage-118120.iam.gserviceaccount.com&" +
                    "Expires="));
        }
    }
}