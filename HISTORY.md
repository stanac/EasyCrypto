## v6.2
- Added QuickEncryption (~3000x faster encryption and ~5500x faster decryption for small data)

## v6.1
- Added target framework 8 (6 and 7 are still supported)

## v6.0
- Changed framework dependency from netstandard1.6 to net6.0

## v5.0
- Removed code marked as obsolete in previous versions

## v4.5.0
- Performance improvement in CryptoRandom using buffer (about 25% faster, see Banchmarks.md file)

## v4.4.0
- IdGenerator class added

## v4.3.0
- RsaEncryption class and other RSA relevant classes

## v 4.2.0
- Added methods `HashToken` and `ValidateTokenHash` to `TokenGenerator` class

## v 4.1.0
- Added `TokenGenerator` class

## v 4.0.0
- Simplified package to target only `netstandard1.6`

## v 3.2.1
- Fixed Core build nuget

## v 3.2
- Added AesFileEncryption class
- Added Async methods in AesEncryption class

## v 3.1
- Updated data format version

## v 3.0
- Support for .NET Core

## v 2.0
- Added additional data functionality (check readme.md)
- For encrypting KCV and additional data instead of zeros padding now it's used PKCS7. (That's the reason behind version increment, 
we cannot guarantee that data encrypted with v1 can be decrypted with v2. Zeros padding was a a good choice for KCV but not for additional data.)

## v 1.1
- Fixed KCV creation (previously KCV validation will pass with any key, although security was not compromised)
- Exposed validation API in form of methods:
  - ValidateEncryptedData
  - ValidateEncryptedDataWithEmbededIv
  - ValidateEncryptedDataWithPassword

## v 1.0
- Initial release