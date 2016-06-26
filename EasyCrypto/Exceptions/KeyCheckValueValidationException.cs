using System;
using System.Security.Cryptography;

namespace EasyCrypto.Exceptions
{
    /// <summary>
    /// This is exception is thrown when validation of Key Check Value (KCV) is failing. 
    /// This can occur when wrong key/password is used for decryption.
    /// </summary>
    [Serializable]
    public class KeyCheckValueValidationException : CryptographicException
    {
        public KeyCheckValueValidationException() { }
        public KeyCheckValueValidationException(string message) : base(message) { }
        public KeyCheckValueValidationException(string message, Exception inner) : base(message, inner) { }
        protected KeyCheckValueValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
