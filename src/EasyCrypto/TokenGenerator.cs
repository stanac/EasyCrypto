using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCrypto
{
    /// <summary>
    /// Utility for generating string tokens for different purposes (e.g. password reset, email address confirmation, ...)
    /// </summary>
    public class TokenGenerator
    {
        private static readonly CryptoRandom _rand = new CryptoRandom();

        public const string DefaultAllowedChars = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
        
        private readonly string _allowedChars;

        /// <summary>
        /// Default constructor, uses English upper case and lower case letters and numeric characters for allowedChar,
        /// which are used for token generation. <see cref="DefaultAllowedChars"/>
        /// </summary>
        public TokenGenerator()
        {
            _allowedChars = DefaultAllowedChars;
        }

        /// <summary>
        /// Constructor that allows defining allowed characters to be used for token generation
        /// </summary>
        /// <param name="allowedChars">
        /// Characters to be used for token generation, cannot be null or empty, must include at least 10
        /// distinct characters, white space characters (space, tab, new line, ...) are ignored
        /// </param>
        public TokenGenerator(string allowedChars)
        {
            if (string.IsNullOrWhiteSpace(allowedChars))
            {
                throw new ArgumentException("Parameter cannot be null or empty", nameof(allowedChars));
            }
            
            allowedChars = new string(allowedChars.Where(c => !char.IsWhiteSpace(c)).Distinct().ToArray());

            if (allowedChars.Length < 10)
            {
                throw new ArgumentException("Parameter needs to have at least 10 distinct characters.", nameof(allowedChars));
            }

            _allowedChars = allowedChars;
        }

        /// <summary>
        /// Generates random string token of specified length
        /// </summary>
        /// <param name="length">Token length must be greater than 0.</param>
        /// <returns>Generated random token with specified length.</returns>
        public string GenerateToken(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Parameter length must be greater than 0.");
            }

            char[] chars = new char[length];
            int[] indexes = new int[length];
            _rand.FillIntArrayWithRandomValues(indexes, 0, _allowedChars.Length);
            for (int i = 0; i < length; i++)
            {
                chars[i] = _allowedChars[indexes[i]];
            }

            return new string(chars);
        }
    }
}
