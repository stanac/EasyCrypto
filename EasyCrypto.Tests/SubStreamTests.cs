using System.IO;
using Xunit;

namespace EasyCrypto.Tests
{
    public class SubStreamTests
    {
        [Fact]
        public void RangeCheckIsOk()
        {
            byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            using (Stream memStream = new MemoryStream())
            {
                memStream.Write(data, 0, data.Length);
                using (Stream subStream = new SubStream(memStream, 4))
                {
                    subStream.Position = 0;
                    byte[] buffer = new byte[4];
                    subStream.Read(buffer, 0, 4);
                    Assert.Equal(new byte[] { 4, 5, 6, 7 }, buffer);
                }
            }
        }
    }
}
