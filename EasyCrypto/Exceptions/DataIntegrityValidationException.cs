using System;
using System.Security.Cryptography;

namespace EasyCrypto.Exceptions
{
    /// <summary>
    /// This exception is thrown when validation of data integrity is failing.
    /// This will occur when data that is being decrypted is changed before decrypting.
    /// </summary>
    [Serializable]
    public class DataIntegrityValidationException : CryptographicException
    {
        public DataIntegrityValidationException() { }
        public DataIntegrityValidationException(string message) : base(message) { }
        public DataIntegrityValidationException(string message, Exception inner) : base(message, inner) { }
        protected DataIntegrityValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
