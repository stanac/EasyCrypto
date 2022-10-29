using System.Security.Cryptography;
using Xunit;

namespace EasyCrypto.Tests;

public class RsaKeyEncoderTests
{
    [Fact]
    public void PrivateEncodedKey_DecodeKey_GivesEqualKey()
    {
        var k1 = GetKey(true);
        string s = RsaKeyEncoder.Encode(k1);
        var k2 = RsaKeyEncoder.Decode(s);
        AssertKeysAreEqual(k1, k2);
    }

    [Fact]
    public void PublicEncodedKey_DecodeKey_GivesEqualKey()
    {
        var k1 = GetKey(false);
        string s = RsaKeyEncoder.Encode(k1);
        var k2 = RsaKeyEncoder.Decode(s);
        AssertKeysAreEqual(k1, k2);

    }

    private RSAParameters GetKey(bool includePrivate)
    {
        using (var rsa = RSA.Create())
        {
            rsa.KeySize = 2048;
            return rsa.ExportParameters(includePrivate);
        }
    }

    private void AssertKeysAreEqual(RSAParameters k1, RSAParameters k2)
    {
        Assert.True(CompareByteArraysAllowNull(k1.D, k2.D), "D is equal");
        Assert.True(CompareByteArraysAllowNull(k1.DP, k2.DP), "DP is equal");
        Assert.True(CompareByteArraysAllowNull(k1.DQ, k2.DQ), "DQ is equal");
        Assert.True(CompareByteArraysAllowNull(k1.Exponent, k2.Exponent), "E is equal");
        Assert.True(CompareByteArraysAllowNull(k1.InverseQ, k2.InverseQ), "IQ is equal");
        Assert.True(CompareByteArraysAllowNull(k1.Modulus, k2.Modulus), "M is equal");
        Assert.True(CompareByteArraysAllowNull(k1.P, k2.P), "P is equal");
        Assert.True(CompareByteArraysAllowNull(k1.Q, k2.Q), "Q is equal");            
    }

    private bool CompareByteArraysAllowNull(byte[] b1, byte[] b2)
    {
        if (b1 == null && b2 == null) return true;

        return DataTools.CompareByteArrays(b1, b2);
    }
}