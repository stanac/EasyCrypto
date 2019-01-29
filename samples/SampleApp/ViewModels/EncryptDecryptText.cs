namespace SampleApp.ViewModels
{
    public class EncryptDecryptText : EncryptDecryptBase
    {
        protected override void Decrypt()
        {
            if (!ValidatePassword()) return;
            DecryptInput = EasyCrypto.AesEncryption.DecryptWithPassword(DecryptInput ?? "", Password);
        }

        protected override void Encrypt()
        {
            if (!ValidatePassword()) return;
            EncryptInput = EasyCrypto.AesEncryption.EncryptWithPassword(EncryptInput ?? "", Password);
        }
    }
}
