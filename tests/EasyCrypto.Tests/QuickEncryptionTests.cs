using System.Text;
using FluentAssertions;
using Xunit;

namespace EasyCrypto.Tests;

public class QuickEncryptionTests
{
    private readonly string _plainText;
    private readonly QuickEncryptionKey _key = QuickEncryptionKey.CreateNew();

    public QuickEncryptionTests()
    {
        StringBuilder sb = new();

        for (int i = 0; i < 10; i++)
        {
            sb.Append(TokenGenerator.Default.GenerateToken(120));
        }

        _plainText = sb.ToString();
    }

    [Fact]
    public void EncryptText_Decrypt_GivesSameText()
    {
        string encrypted = QuickEncryption.Encrypt(_plainText, _key);
        string decrypted = QuickEncryption.Decrypt(encrypted, _key);

        decrypted.Should().BeEquivalentTo(_plainText);
    }
}