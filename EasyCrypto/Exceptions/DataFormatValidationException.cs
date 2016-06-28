using System;

namespace EasyCrypto.Exceptions
{
    [Serializable]
    public class DataFormatValidationException : Exception
    {
        public DataFormatValidationException() { }
        public DataFormatValidationException(string message) : base(message) { }
        public DataFormatValidationException(string message, DataValidationErrors error) : base(message) { ValidationError = error; }
        public DataFormatValidationException(string message, Exception inner) : base(message, inner) { }
        protected DataFormatValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public DataValidationErrors? ValidationError { get; set; }

        public enum DataValidationErrors
        {
            DataIsTooShort,
            InvalidMagicNumber,
            UnsupportedDataVersion,
            DataIntegrityValidationError,
            KeyCheckValueValidationError
        }
    }
}
