using Xunit;

namespace EasyCrypto.Tests
{
    public class SystemStringBase55ConverterTests
    {
        [Fact]
        public void ConvertLongToStringBackToLong_GivesEqualValue()
        {
            long[] testValues = new long[]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,
                101, 102, 111, 123, 144, 156, 154, 748, 999,
                1000, 1001, 1002, 1003, 1004, 1005, 9999,
                1231421, 456465, 987878, 10132134
            };

            foreach (long l in testValues)
            {
                string s = SystemStringBase55Converter.ToString(l);
                long l2 = SystemStringBase55Converter.ToLong(s);

                Assert.Equal(l, l2);
            }
        }
    }
}
