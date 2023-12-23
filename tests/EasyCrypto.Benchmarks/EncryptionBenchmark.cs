using BenchmarkDotNet.Attributes;

namespace EasyCrypto.Benchmarks;

public class EncryptionBenchmark
{
    private const string PlainText = "Test text 1, a test text 123 AB";
    private const string Password = "password";
    private static readonly string Encrypted;
    private static readonly string EncryptedQuick;
    private static readonly QuickEncryptionKey Key = QuickEncryptionKey.Parse("aBNChrdnbgsNNeIEGTri1tVDhd0QbQQl");

    static EncryptionBenchmark()
    {
        Encrypted = AesEncryption.EncryptWithPassword(PlainText, Password);
        EncryptedQuick = QuickEncryption.Encrypt(PlainText, Key);
    }

    [Benchmark]
    public void AesEncrypt()
    {
        AesEncryption.EncryptWithPassword(PlainText, Password);
    }

    [Benchmark]
    public void AesDecrypt()
    {
        AesEncryption.DecryptWithPassword(Encrypted, Password);
    }

    [Benchmark]
    public void AesEncryptQuick()
    {
        QuickEncryption.Encrypt(PlainText, Key);
    }

    [Benchmark]
    public void AesDecryptQuick()
    {
        QuickEncryption.Decrypt(EncryptedQuick, Key);
    }
}