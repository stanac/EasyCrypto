using EasyCrypto.Internal;
using FluentAssertions;
using Xunit;

namespace EasyCrypto.Tests.Internal;

public class FormattedBinaryDataTests
{
    [Fact]
    public void TwoIntegers_WrittenData_CanBeRead()
    {
        FormattedBinaryData data = new FormattedBinaryData(78, typeof(int), typeof(int));

        object[] input = { 4, 8 };

        byte[] bytes = data.ToBytes(input);

        object[] result = data.Read(bytes);

        result.Should().BeEquivalentTo(input);
    }

    [Fact]
    public void TwoIntArrays_WrittenData_CanBeRead()
    {
        FormattedBinaryData data = new FormattedBinaryData(78, typeof(int[]), typeof(int[]));

        object[] input = { new int[] { 4, 8 }, new int[] { 22, 61589 } };

        byte[] bytes = data.ToBytes(input);

        object[] result = data.Read(bytes);

        result.Should().BeEquivalentTo(input);
    }

    [Fact]
    public void TwoIntsTwoByteArrays_WrittenData_CanBeRead()
    {
        int i = CryptoRandom.Default.NextInt();
        int j = CryptoRandom.Default.NextInt();
        byte[] b1 = CryptoRandom.Default.NextBytes(128);
        byte[] b2 = CryptoRandom.Default.NextBytes(128);

        FormattedBinaryData data = new FormattedBinaryData(9001,
            typeof(int), typeof(int), typeof(byte[]), typeof(byte[])
        );

        object[] input = {i, j, b1, b2};

        byte[] written = data.ToBytes(input);

        object[] read = data.Read(written);

        read.Should().BeEquivalentTo(input);
    }

    [Fact]
    public void Combined_WrittenData_CanBeRead()
    {
        FormattedBinaryData data = new FormattedBinaryData(879,
            typeof(int),
            typeof(int[]),
            typeof(byte[]),
            typeof(int),
            typeof(byte[]),
            typeof(int)
        );

        object[] input =
        {
            14,
            new[] {9, int.MaxValue, 7, -98987},
            new byte[] {128, 255, 33, 14, 18},
            int.MinValue + 2,
            new byte[] {11, 12, 14},
            -78
        };

        byte[] bytes = data.ToBytes(input);

        object[] result = data.Read(bytes);

        result.Should().BeEquivalentTo(input);
    }
}