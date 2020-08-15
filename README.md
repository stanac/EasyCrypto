# EasyCrypto

 [![.NET Core](https://github.com/stanac/EasyCrypto/workflows/.NET%20Core/badge.svg)](https://github.com/stanac/EasyCrypto/actions?query=workflow%3A%22.NET+Core%22)
 [![Coverage Status](https://img.shields.io/coveralls/stanac/EasyCrypto/master.svg?maxAge=900)](https://coveralls.io/github/stanac/EasyCrypto?&branch=master)
 [![NuGet](https://img.shields.io/nuget/v/EasyCrypto.svg)](https://www.nuget.org/packages/EasyCrypto/)
 [![downloads](https://img.shields.io/nuget/dt/EasyCrypto.svg?style=flat)](https://www.nuget.org/packages/EasyCrypto/)
 [![license](https://img.shields.io/github/license/stanac/EasyCrypto.svg)](https://github.com/stanac/EasyCrypto/blob/master/LICENSE)
 
Primary goal of this library is to enable users with little or no cryptography knowledge to encrypt and decrypt data in an easy and
safe manner as well work with passwords and random values.

EasyCrypto is .NETStandard (1.6+) library that helps with
- Encryption and decryption of streams, byte arrays, strings and files
- Password generating, hashing and validating
- Generating crypto secure random bytes, integers and doubles
- Generating crypto secure random string tokens and string identifiers

Implementation details:
- For symetric encryption AES265 is used, IVs are 128 bits large and every 
result of the encryption is embedded with [KCV](https://en.wikipedia.org/wiki/Key_checksum_value) (just first three bytes)
and [MAC](https://en.wikipedia.org/wiki/Message_authentication_code). MAC is calculated using HMACSHA384.
- CryptoRandom and PasswordGenerator is using [RNGCryptoServiceProvider](https://msdn.microsoft.com/en-us/library/system.security.cryptography.rngcryptoserviceprovider.aspx)
- Hashing of password is done with [Rfc2898DeriveBytes](msdn.microsoft.com/en-us/library/system.security.cryptography.rfc2898derivebytes.aspx)
with default hash and salt size of 256 bits and 25K iterations (by default).
- Asymmetric (public key) encryption is using RSA with 2048 bits keys (by default).

---

For changes see [history](https://github.com/stanac/EasyCrypto/blob/master/HISTORY.md).

## Install from nuget

[nuget.org link](https://www.nuget.org/packages/EasyCrypto/)

```
Install-Package EasyCrypto
```

## Docs

Table of contents:
- [AesEncryption](#static-class-aesencryption)
- [AesFileEncryption](#static-class-aesfileencryption)
- [AesEncryptionAdditionalData](#static-class-aesencryptionadditionaldata)
- [CryptoRandom](#class-cryptorandom--idisposable)
- [PasswordGenerator](#class-passwordgenerator--idisposable)
- [PasswordHasher](#class-passwordhasher)
- [TokenGenerator](#class-tokengenerator)
- [IdGenerator](#class-idgenerator)
- [RsaEncryption](#static-class-rsaencryption)

---

### Static class AesEncryption

AesEncryption class can work with streams, byte arrays and strings. 

Available methods:

```csharp
static void Encrypt(Stream dataToEncrypt, byte[] key, byte[] iv, Stream destination)
static void Decrypt(Stream dataToDecrypt, byte[] key, byte[] iv, Stream destination)

static byte[] Decrypt(byte[] dataToDecrypt, byte[] key, byte[] iv) 
static byte[] Encrypt(byte[] dataToEncrypt, byte[] key, byte[] iv)


// following methods are generating random IV and embedding it into the encrypted data
// so encrypted data can be decrypted with just the key

static void EncryptAndEmbedIv(Stream dataToEncrypt, byte[] key, Stream destination)
static void DecryptWithEmbeddedIv(Stream dataToDecrypt, byte[] key, Stream destination)

static byte[] EncryptAndEmbedIv(byte[] dataToEncrypt, byte[] key)
static byte[] DecryptWithEmbeddedIv(byte[] dataToDecrypt, byte[] key) 


// following methods are generating random salt and random IV
// calculating hash from the password
// then generated salt and hash are embeded into the encrypted data
// so data can be decrypted using just the password

static void EncryptWithPassword(Stream dataToEncrypt, string password, Stream destination)
static void DecryptWithPassword(Stream dataToDecrypt, string password, Stream destination)

static byte[] EncryptWithPassword(byte[] dataToEncrypt, string password)
static byte[] DecryptWithPassword(byte[] dataToDecrypt, string password)

static string EncryptWithPassword(string dataToEncrypt, string password)
static string DecryptWithPassword(string dataToDecrypt, string password)


// validation methods (from v1.1.0, used to verify key/password and data integrity):

static ValidationResult ValidateEncryptedData(byte[] encryptedData, byte[] key, byte[] iv)
static ValidationResult ValidateEncryptedData(Stream encryptedData, byte[] key, byte[] iv)

static ValidationResult ValidateEncryptedDataWithEmbededIv(byte[] encryptedData, byte[] key)
static ValidationResult ValidateEncryptedDataWithEmbededIv(Stream encryptedData, byte[] key)

static ValidationResult ValidateEncryptedDataWithPassword(string encryptedData, string password)
static ValidationResult ValidateEncryptedDataWithPassword(byte[] encryptedData, string password)
static ValidationResult ValidateEncryptedDataWithPassword(Stream encryptedData, string password)
```
---

### Static class AesFileEncryption

From v3.2 we have API for file encryption in order to avoid out of memory exceptions

```csharp
// methods for encryption of files
void Encrypt(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile)
async Task EncryptAsync(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile)
void EncryptWithPassword(string sourceFilePath, string destinationFilePath, string password, bool overwriteExistingFile)
async Task EncryptWithPasswordAsync(string sourceFilePath, string destinationFilePath, string password, bool overwriteExistingFile)

// methods for decryption of files
void Decrypt(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile)
async Task DecryptAsync(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile)
void DecryptWithPassword(string sourceFilePath, string destinationFilePath, string password, bool overwriteExistingFile)
async Task DecryptWithPasswordAsync(string sourceFilePath, string destinationFilePath, string password, bool overwriteExistingFile)
``` 

---

### Static class AesEncryptionAdditionalData

From v2 this class can be used for adding additional data to encrypted package. Added additional data is
encrypted with hard-coded key and IV, so it's not realy secure. It can be used for embedding password hint
into the package or any other data that can fit into Dictionary<string, string>. Note that additional data
is Dictionary<string, string> and **entries where key or value is null or empty will be ignored**. This
might be a chance for improvement. Also note that encrypted data with embedded additional data can be
normally decrypted as encrypted data without embedded additional data. Here are available methods:

```csharp
// methods for adding additional data
static string AddAdditionalData(string encryptedData, Dictionary<string, string> additionalData)
static byte[] AddAdditionalData(byte[] encryptedData, Dictionary<string, string> additionalData)
static void AddAdditionalData(Stream encryptedData, Dictionary<string, string> additionalData, Stream destination)

// methods for reading additional data
static Dictionary<string, string> ReadAdditionalData(string encryptedData)
static Dictionary<string, string> ReadAdditionalData(byte[] encryptedData) 
static Dictionary<string, string> ReadAdditionalData(Stream encryptedData)
```



---

### Class CryptoRandom : IDisposable

Every method in CryptoRandom class has static equivalent method which is called [MethodName]Static.
This class is disposable and if you are generating multiple random values it's recommended to use 
instance methods of one instance instead of calling static methods.

Available methods:

```csharp
static byte[] NextBytesStatic(uint length)
byte[] NextBytes(uint length)

static int NextIntStatic() => NextIntStatic(0, int.MaxValue)
static int NextIntStatic(int maxExclusive) => NextIntStatic(0, maxExclusive)
static int NextIntStatic(int minInclusive, int maxExclusive)
int NextInt() => NextInt(0, int.MaxValue)
int NextInt(int maxExclusive) => NextInt(0, maxExclusive)
int NextInt(int minInclusive, int maxExclusive)

static double NextDoubleStatic()
double NextDouble()

void FillIntArrayWithRandomValues(int[] arrayToFill, int minInclusive, int maxExclusive)

void Dispose()
```

---

### Class PasswordGenerator : IDisposable

PasswordGenerator has static methods in the same manner as CryptoRandom, following examples will
show only calls to instance methods.

```csharp
using (var pg = new PasswordGenerator())
{
    string pass1 = pg.Generate(); // 16 chars, includes symbols, numbers, lower and upper case letters
    string pass2 = pg.Generate(8); // 8 chars, includes symbols, numbers, lower and upper case letters
    string pass3 = pg.Generate(
        PasswordGenerationOptions.Default
            .SetMinNumbers(4)   // at least one number
            .SetMinSymbols(4)   // at least one symbol
            .UseSymbols("!@#$") // only those symbols will be used
            .SetLength(12)      // 12 chars output
            // always call SetLength last it will lower the number of min values for 
            // number, symbols, lower case and upper case if needed
            // otherwise you could get an exception thrown
        );
    )
}
```

---

### Class PasswordHasher
This class can be used for hashing and validating passwords, see constructors and methods:
```csharp
// constructors:

PasswordHasher() // 32 bytes of salt, 32 bytes of hash and 25000 hash iterations
PasswordHasher(uint hashAndSaltLengthsInBytes) // 25000 hash iterations
PasswordHasher(uint hashAndSaltLengthsInBytes, uint hashIterations)


// methods:

byte[] GenerateRandomSalt()

byte[] HashPassword(string password, byte[] salt)
byte[] HashPasswordAndGenerateSalt(string password, out byte[] salt)
bool ValidatePassword(string password, byte[] hash, byte[] salt)

byte[] HashPasswordAndGenerateEmbeddedSalt(string password)
bool ValidatePasswordWithEmbeddedSalt(string password, byte[] hashAndEmbeddedSalt)

string HashPasswordAndGenerateEmbeddedSaltAsString(string password)
bool ValidatePasswordWithEmbeddedSalt(string password, string hashAndEmbeddedSalt)
```

---

### Class TokenGenerator

This class can used for generating random string tokens for e.g. password reset, email address confirmation, etc... It also provides methods for 
hashing tokens and validating token hashes (it's not recommended to store plain text tokens in db)

```csharp
// default chars used for token generation
const string DefaultAllowedChars = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";

// constructors:

TokenGenerator() // default constructor; uses DefaultAllowedChars
TokenGenerator(string allowedChars) // Constructor that allows defining allowed characters to be used for token generation
// allowedChars parameter must have at least 10 distinct characters, white space characters are ignored

// methods:

// Generates random string token of desired length
// Parameter length must be greater than 0
string GenerateToken(int length)

// Hashes token (with random salt) so you don't have to store plain text token
string HashToken(string token)

// Validates token hash that is created by calling HashToken(string)
bool ValidateTokenHash(string token, string hash)
```

---

### Class IdGenerator

This class generates identifiers that are by default:
- shorter than GUIDs (both as strings and as bytes)
- mostly incremental

IdGenerator generates identifiers in following format:

```
{timeBasedPart, 8 chars}{fixedPart, default is empty}{randomPart 4-100 chars, default is 6}
```

IdGenerator can optional add fixed part between time based part and random part and can optionally
add hyphens between parts. Optional fixed part is useful if you want to know which (micro)service has 
generated specific ID (where each service, or service instance has specific fixed part set)

How it works:
- Custom base 55 charset (numbers and English alphabet lower case and upper case excludes chars `I`, `i`,
`O`, `o`, `l`, `0` and `1`) is defined for converting time and generating random parts
- Time base part is generated by taking `DateTime.UtcNow` or overridden time and calculating total miliseconds
since `UTC 2010-01-01 00:00`, that value is converted to chars with custom base 55 charset
- Fixed part (if defined) is appended
- Random part is generated and appended

Some examples of generated identifiers:
```
default generator:                   2Qj4AbNMz7NHzP
with hyphens:                        2Qj4AbNM-z7NHzP
with fixed part (ABC):               2Qj4AbNMABCz7NHzP
with fixed part (ABC) and hyphens:   2Qj4AbNM-ABC-z7NHzP
with longer random part:             2Qj4AbNMABCqAAA9N7F5aBUSm3z
with longer random part and hyphens: 2Qj4AbNM-ABC-qAAA9N7F5aBUSm3z
```

Constructors:
```csharp
// Default constructor, FixedPart = "", FastRandom = true
IdGenerator() : this("", true)
{
}

// Constructor to accept boolean value telling the generator to use or not to use fast random
// If fastRandom is true System.Random is used, otherwise EasyCrypto.CryptoRandom is used
IdGenerator(bool fastRandom): this("", fastRandom)
{
}

// Constructor to accept boolean value telling the generator to use or not to use fast random and fixed part of generated id
// fixedPart is fixed part to set in middle of generated id
// If fastRandom is true System.Random is used, otherwise EasyCrypto.CryptoRandom is used
IdGenerator(string fixedPart, bool fastRandom)
{
    FixedPart = (fixedPart ?? "").Trim();
    FastRandom = fastRandom;
}
```

Properties:
```csharp
// Default instance of Id generator with FastRandom = true, FixedPart = "", RandomPartLength = 6 and AddHyphens = false
static IdGenerator Default { get; } = new IdGenerator();
// Warning: Default is static property, changing it's properties will affect every call of IdGenerator.Default

// Length of random part, default value is 6, must not be less than 4 or greater than 100
int RandomPartLength { get; set; }

// If true hyphens (-) are added between parts
bool AddHyphens { get; set; }
```

Methods:
```csharp
// Generates new id string where for time part current UTC time is used
string NewId() => NewId(DateTime.UtcNow);

// Generates new id using specified time
// warning: when using this override, make sure to specify very precise time, including milliseconds
string NewId(DateTime time);
```

Usage examples:
```csharp
string id1 = IdGenerator.Default.NewId(); // example: 2Qj4AbNMz7NHzP

var gen0 = new IdGenerator();
string id0  = gen0.NewId(); // example: 2Qj4AbNMz7NHzP

var gen1 = new IdGenerator { AddHyphens = true };
string id1  = gen1.NewId(); // example: 2Qj4AbNM-z7NHzP

var gen2 = new IdGenerator("ABC", true) { AddHyphens = true };
string id2 = gen2.NewId(); // example: 2Qj4AbNM-ABC-z7NHzP

var gen3 = new IdGenerator("ABC", true) { AddHyphens = true, RandomPartLength = 16 };
string id3 = gen3.NewId(); // example: 2Qj4AbNM-ABC-qAAA9N7F5aBUSm3z

// example with specific time
DateTime specificTime = new DateTime(2011, 01, 08, 16, 35, 37, 461, DateTimeKind.Utc);
string idWithSpecificTime1 = gen0.NewId(specificTime); // 2EZ7UwPZatcZCe
string idWithSpecificTime2 = gen0.NewId(specificTime); // 2EZ7UwPZanDYS8
// warning: make sure when using specific time to use very precise time, including milliseconds
```

---

### Static Class RsaEncryption

This class can be used for asymmetric encryption. Public key can be used for encryption and private key for decryption.
It's not recommended to store private keys in plain text on disk. Data that is passed to encryption methods must be 
smaller than the key size used.

```csharp
// Methods for generating key pairs
static RsaKeyPair GenerateKeyPairs() => GenerateKeyPairs(RsaKeySizes.Rsa2048);
static RsaKeyPair GenerateKeyPairs(RsaKeySizes keySize)

// encryption methods
static byte[] Encrypt(byte[] data, RsaPublicKey key)
static string Encrypt(string data, RsaPublicKey key)

// decryption methods
static byte[] Decrypt(byte[] data, RsaPrivateKey key)
static string Decrypt(string data, RsaPrivateKey key)
```

Example for usage of RsaEncryption:

```csharp
[Fact]
public void String_EncryptDecrypt_GivesEqualString()
{
    var keys = RsaEncryption.GenerateKeyPairs();

    var plainText = Guid.NewGuid().ToString();
    string encrypted = RsaEncryption.Encrypt(plainText, keys.PublicKey);
    string decrypted = RsaEncryption.Decrypt(encrypted, keys.PrivateKey);
    
    Assert.Equal(plainText, decrypted);
}
```
