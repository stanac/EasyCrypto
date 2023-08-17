using System;
using System.Security.Cryptography;

namespace EasyCrypto.Exceptions
{
    /// <summary>
    /// This exception is thrown when validation of data integrity is failing.
    /// This will occur when data that is being decrypted is changed before decrypting.
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.CryptographicException" />
    public class DataIntegrityValidationException : CryptographicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataIntegrityValidationException"/> class.
        /// </summary>
        public DataIntegrityValidationException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DataIntegrityValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataIntegrityValidationException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DataIntegrityValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DataIntegrityValidationException(string message, Exception inner) : base(message, inner) { }
        
    }
}
