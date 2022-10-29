using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace EasyCrypto.Tests;

public class AesEncryptionAdditionalDataTests
{
    private const int NumberOrTestRuns = 5;
    private readonly Dictionary<string, string> additionalData = new Dictionary<string, string>
    {
        { "key1", "value1" },
        { "key2", "value2" }
    };

    [Fact]
    public void EncryptedDataWithAdditionalDataCanBeDecrypted()
    {
        for (int i = 0; i < NumberOrTestRuns; i++)
        {
            using (CryptoRandom cr = new CryptoRandom())
            {
                string password = Guid.NewGuid().ToString();
                byte[] plainText = cr.NextBytes((uint)cr.NextInt(44, 444));
                byte[] encrypted = AesEncryption.EncryptWithPassword(plainText, password);
                byte[] encryptedWithAdditionalData = AesEncryptionAdditionalData.AddAdditionalData(encrypted, additionalData);

                byte[] decrypted = AesEncryption.DecryptWithPassword(encrypted, password);
                Assert.Equal(Convert.ToBase64String(plainText), Convert.ToBase64String(decrypted));
            }
        }
    }

    [Fact]
    public void SerializedAdditionalDataCanBeDeserialized()
    {
        Type additionalDataType = GetAdditionalDataType();
        var serializeMethod = additionalDataType.GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Static);
        var deserializeMethod = additionalDataType.GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Static);
        byte[] serialized = (byte[])serializeMethod.Invoke(null, new object[] { additionalData });
        Dictionary<string, string> deserialized = (Dictionary<string, string>)deserializeMethod.Invoke(null, new object[] { serialized });

        AssertAdditionalData(deserialized);
    }

    [Fact]
    public void SerializedEncryptedAdditionalDataCanBeDeserialized()
    {
        Type additionalDataType = GetAdditionalDataType();
        ConstructorInfo constructor = additionalDataType.GetConstructor(new[] { typeof(Dictionary<string, string>) });
        object additionalDataObj = constructor.Invoke(new object[] { additionalData });
        byte[] serialized = (byte[])additionalDataType.GetMethod("GetBytes", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(additionalDataObj, new object[0]);
        var dataProperty = additionalDataType.GetProperty("Data", BindingFlags.Instance | BindingFlags.Public);
        var additionalDataDeserializerObj = additionalDataType.GetMethod("LoadFromBytes", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { serialized });
        Dictionary<string, string> deserialized = (Dictionary<string, string>)dataProperty.GetMethod.Invoke(additionalDataDeserializerObj, null);

        AssertAdditionalData(deserialized);
    }

    [Fact]
    public void EncryptedAdditionalDataCanBeDecrypted()
    {
        Type additionalDataType = GetAdditionalDataType();
        var encryptMethod = additionalDataType.GetMethod("EncryptAdditionalData", BindingFlags.NonPublic | BindingFlags.Static);
        var decryptMethod = additionalDataType.GetMethod("DecryptAddtionalData", BindingFlags.NonPublic | BindingFlags.Static);

        byte[] data = { 1, 2, 3, 4, 5 };
        byte[] encrypted = (byte[])encryptMethod.Invoke(null, new object[] { data });
        byte[] decrypted = (byte[])decryptMethod.Invoke(null, new object[] { encrypted });

        Assert.Equal(Convert.ToBase64String(data), Convert.ToBase64String(decrypted));
    }

    [Fact]
    public void AdditionalDataCanBeRead()
    {
        for (int i = 0; i < NumberOrTestRuns; i++)
        {
            using (CryptoRandom cr = new CryptoRandom())
            {
                string password = Guid.NewGuid().ToString();
                string plainText = PasswordGenerator.GenerateStatic();
                string encrypted = AesEncryption.EncryptWithPassword(plainText, password);
                string encryptedWithAdditionalData = AesEncryptionAdditionalData.AddAdditionalData(encrypted, additionalData);
                string decrypted = AesEncryption.DecryptWithPassword(encrypted, password);
                Assert.Equal(plainText, decrypted);

                Dictionary<string, string> data = AesEncryptionAdditionalData.ReadAdditionalData(encryptedWithAdditionalData);
                Assert.Equal(2, data.Count);
                Assert.Equal("value1", data["key1"]);
                Assert.Equal("value2", data["key2"]);
            }
        }
    }

    [Fact]
    public void DataCanBeValidatedWithAdditionalData()
    {
        string password = Guid.NewGuid().ToString();
        string plainText = PasswordGenerator.GenerateStatic();
        string encrypted = AesEncryption.EncryptWithPassword(plainText, password);
        string encryptedWithAdditionalData = AesEncryptionAdditionalData.AddAdditionalData(encrypted, additionalData);
        var validationResult = AesEncryption.ValidateEncryptedDataWithPassword(encrypted, password);
        Assert.True(validationResult.IsValid);
    }

    private void AssertAdditionalData(Dictionary<string, string> data)
    {
        Assert.Equal(additionalData.Count, data.Count);
        foreach (var key in additionalData.Keys)
        {
            Assert.Equal(additionalData[key], data[key]);
        }
    }

    private Type GetAdditionalDataType()
    {

#if core
            Type additionalDataType = typeof(AesEncryptionAdditionalData).GetTypeInfo().Assembly.GetTypes().Single(x => x.Name == "AdditionalData");
#else
        Type additionalDataType = Assembly.GetAssembly(typeof(AesEncryptionAdditionalData)).GetTypes().Single(x => x.Name == "AdditionalData");
#endif
        return additionalDataType;
    }
}