using FluentAssertions;
using Xunit;

namespace EasyCrypto.Tests;

public class QuickEncryptionKeyTests
{
    [Fact]
    public void Create_ToString_Parse_GivesSameKey()
    {
        QuickEncryptionKey key1 = QuickEncryptionKey.CreateNew();
        string s = key1.ToString();
        QuickEncryptionKey key2 = QuickEncryptionKey.Parse(s);

        key2.ToString().Should().BeEquivalentTo(s);
        key2.Key.Should().BeEquivalentTo(key1.Key);
    }
}