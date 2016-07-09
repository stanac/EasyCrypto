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