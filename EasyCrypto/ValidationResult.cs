using EasyCrypto.Exceptions;
using static EasyCrypto.Exceptions.DataFormatValidationException;

namespace EasyCrypto
{
    public class ValidationResult
    {
        public bool KeyIsValid { get; internal set; }
        public bool DataIntegrityIsValid { get; internal set; }
        public bool DataFormatIsValid { get; internal set; }
        public bool DataFormatVersionIsValid { get; internal set; }
        public bool DataFormatVersionIsExact { get; internal set; }

        public bool IsValid => KeyIsValid && DataFormatIsValid && DataIntegrityIsValid && DataFormatVersionIsValid;

        internal DataFormatValidationException ExceptionToThrow { get; private set; }
        public string ErrorMessage { get; set; }

        internal void SetException(DataValidationErrors error)
        {
            string message = "Unknown error";
            switch (error)
            {
                case DataValidationErrors.DataIntegrityValidationError:
                    message = "Data integrity is compromised.";
                    break;
                case DataValidationErrors.DataIsTooShort:
                    message = "Data does not appear to be encrypted by EasyCrypto";
                    break;
                case DataValidationErrors.InvalidMagicNumber:
                    message = "Data does not appear to be encrypted by EasyCrypto";
                    break;
                case DataValidationErrors.KeyCheckValueValidationError:
                    message = "Key/IV or password is not valid";
                    break;
                case DataValidationErrors.UnsupportedDataVersion:
                    message = "Data is encrypted by newer version of the EasyCrypto";
                    break;
            }
            ExceptionToThrow = new DataFormatValidationException(message, error);
            ErrorMessage = message;
        }
    }
}
