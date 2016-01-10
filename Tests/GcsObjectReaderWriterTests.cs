using System.IO;
using System.Text;
using NUnit.Framework;

namespace HelloGcs.Tests
{
    [TestFixture]
    public class GcsObjectReaderWriterTests
    {
        private const string BUCKET_NAME = "robert-hargreaves-test2";
        private const string DATA = "foo";
        private const string KEY = "bar";

        [Test]
        public void Write_and_read_object()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(DATA));
            var readerWriter = new GcsObjectReaderWriter();
            readerWriter.WriteObject(stream, KEY, BUCKET_NAME);
        }

        [Test]
        public void Get_object()
        {
            var stream = new MemoryStream();
            var readerWriter = new GcsObjectReaderWriter();
            readerWriter.ReadObject(stream, KEY, BUCKET_NAME);
            var str = Encoding.UTF8.GetString(stream.ToArray());
            Assert.That(str, Is.EqualTo(DATA));
        }
    }
}
