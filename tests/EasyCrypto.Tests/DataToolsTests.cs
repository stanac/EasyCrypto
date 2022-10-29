using System;
using Xunit;

namespace EasyCrypto.Tests;

public class DataToolsTests
{
    [Fact]
    public void ArrayJoinTest()
    {
        byte[] a = { 0, 1, 2 };
        byte[] b = { 3, 4, 5, 6 };
        byte[] c = { 7, 8, 9, 10, 11 };
        byte[] d = { 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };

        byte[] expected =
        {
            0,  1,  2,  3,  4,  5,  6,  7,
            8,  9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21
        };
        byte[] summed = DataTools.JoinByteArrays(a, b, c, d);

        Assert.Equal(expected, summed);
    }

    public static bool CompareByteArrays(byte[] ba1, byte[] ba2)
    {
        if (ba1 == null) throw new ArgumentNullException(nameof(ba1));
        if (ba2 == null) throw new ArgumentNullException(nameof(ba2));

        if (ba1.Length != ba2.Length) return false;
        for (int i = 0; i < ba1.Length; i++)
        {
            if (ba1[i] != ba2[i]) return false;
        }
        return true;
    }
}