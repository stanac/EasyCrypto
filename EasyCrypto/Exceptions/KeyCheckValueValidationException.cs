using System;
using System.Security.Cryptography;

namespace EasyCrypto.Exceptions
{
    /// <summary>
    /// This is exception is thrown when validation of Key Check Value (KCV) is failing.
    /// This can occur when wrong key/password is used for decryption.
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.CryptographicException" />
    [Serializable]
    public class KeyCheckValueValidationException : CryptographicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCheckValueValidationException"/> class.
        /// </summary>
        public KeyCheckValueValidationException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCheckValueValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public KeyCheckValueValidationException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCheckValueValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public KeyCheckValueValidationException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCheckValueValidationException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected KeyCheckValueValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
