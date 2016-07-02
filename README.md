# EasyCrypto

[![Build status](https://ci.appveyor.com/api/projects/status/6py1gx536fn0tu2j?svg=true)](https://ci.appveyor.com/project/ProjectMona/easycrypto)

EasyCrypto is .NET library that helps with
- Encryption and decryption of streams, byte arrays and strings
- Password generating, hashing and validating
- Generating crypto secure random bytes, integers and doubles

Implementation details:
- For encryption AES265 is used, IVs are 128 bits large and every 
result of the encryption is embedded with [KCV](https://en.wikipedia.org/wiki/Key_checksum_value) (just first three bytes)
and [MAC](https://en.wikipedia.org/wiki/Message_authentication_code). MAC is calculated using HMACSHA384.
- CryptoRandom and PasswordGenerator is using [RNGCryptoServiceProvider](https://msdn.microsoft.com/en-us/library/system.security.cryptography.rngcryptoserviceprovider.aspx)
- Hashing of password is done with [Rfc2898DeriveBytes](msdn.microsoft.com/en-us/library/system.security.cryptography.rfc2898derivebytes.aspx)
with default hash and salt size of 256 bits and 25K iterations.
- Asymmetric (public key) encryption is currently not supported.

## Install from nuget

```
Install-Package EasyCrypto
```

## Examples of usage

**For full API see the [pages](https://stanac.github.io/EasyCrypto/).**

---

### Static class AesEncryption

AesEncryption class can work with streams, byte arrays and strings. 

Available methods:

```cs
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
```

---

### Class CryptoRanom : IDisposable

Every method in CryptoRandom class has static equivalent method which is called [MethodName]Static.
This class is disposable and if you are generating multiple random values it's recommended to use 
instance methods of one instance instead of calling static methods.

Available methods:

```cs
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

// note: there is room for performance improvement in this class, we don't use any buffer at the moment
```

---

### Class PasswordGenerator : IDisposable

PasswordGenerator has static methods in the same manner as CryptoRanom, following examples will
show only calls to instance methods. For all available options check [PasswordGenerationOptions class](https://stanac.github.io/EasyCrypto/EasyCrypto/PasswordGenerationOptions.htm).

```cs
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

### Class PasswordHasher
This class can be used for hashing and validating passwords, see constructors and methods:
```cs
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

#### Future improvements
- Validating keys and encrypted data integrity in AesEncryption (refactor and open up closed APIs)
- Performance improvements on CryptoRandom (with buffer)
- Make it compatible with .NET Core 1
- Extract interfaces so you can replace one or more class implementations (v2, might introduce breaking changes)
- Asymmetric (public key) encryption 