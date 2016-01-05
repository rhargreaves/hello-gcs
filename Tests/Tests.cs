using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using HelloGcs;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Can_write_file_to_GCS()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("foo"));
            var writer = new GcsWriter();
            var result = writer.WriteStream(stream, "bar", "robert-hargreaves-test2");
            Assert.That(result, Is.True);
        }
    }
}
