using System;
using System.Collections.Generic;

namespace EasyCrypto
{
    /// <summary>
    /// Class for generating cryto secure passwords
    /// </summary>
    public class PasswordGenerator : IDisposable
    {
        private readonly CryptoRandom _cr = new CryptoRandom();

        /// <summary>
        /// Generates random password of 16 chars
        /// </summary>
        /// <returns>Random password</returns>
        public static string GenerateStatic() => GenerateStatic(PasswordGenerationOptions.Default);

        /// <summary>
        /// Generates random password with desired length.
        /// </summary>
        /// <param name="length">Length of the password, default is 16, cannot be less than 4</param>
        /// <returns>Random password</returns>
        public static string GenerateStatic(uint length) => GenerateStatic(PasswordGenerationOptions.Default.SetLength(length));

        /// <summary>
        /// Generates password using provided options. Options must be valid (check by calling <see cref="PasswordGenerationOptions.AreValid(out string)"/>).
        /// </summary>
        /// <param name="options">Options used for generating passwords</param>
        /// <returns>Random password</returns>
        public static string GenerateStatic(PasswordGenerationOptions options)
        {
            using (var pg = new PasswordGenerator())
            {
                return pg.Generate(options);
            }
        }

        /// <summary>
        /// Generates random password of 16 chars
        /// </summary>
        /// <returns>Random password</returns>
        public string Generate() => Generate(PasswordGenerationOptions.Default);

        /// <summary>
        /// Generates random password with desired length.
        /// </summary>
        /// <param name="length">Length of the password, default is 16, cannot be less than 4</param>
        /// <returns>Random password</returns>
        public string Generate(uint length) => Generate(PasswordGenerationOptions.Default.SetLength(length));

        /// <summary>
        /// Generates password using provided options. Options must be valid (check by calling <see cref="PasswordGenerationOptions.AreValid(out string)"/>).
        /// </summary>
        /// <param name="options">Options used for generating passwords</param>
        /// <returns>Random password</returns>
        public string Generate(PasswordGenerationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            string error = null;
            if (!options.AreValid(out error))
            {
                throw new ArgumentException(error);
            }
            return GenerateInner(options.GetActuals());
        }

        private string GenerateInner(ActualPasswordGeneratorOptions options)
        {
            // at this point we expect options are validated and not null
            List<char> password = new List<char>();
            Action<string, int> generageGroup = (chars, length) =>
            {
                for (int i = 0; i < length; i++)
                {
                    password.Add(chars[_cr.NextInt(chars.Length)]);
                }
            };

            generageGroup(options.Numbers, options.NumbersLength);
            generageGroup(options.Upper,   options.UpperLength);
            generageGroup(options.Lower,   options.LowerLength);
            generageGroup(options.Symbols, options.SymbolsLength);

            password = ShuffleCharList(password);
            return new string(password.ToArray());
        }

        private List<char> ShuffleCharList(List<char> s)
        {
            char temp;
            int nextPosition;
            for (int i = 0; i < s.Count / 2 || i == 0; i++)
            {
                for (int j = 0; j < s.Count; j++)
                {
                    temp = s[j];
                    nextPosition = _cr.NextInt(s.Count);
                    s[j] = s[nextPosition];
                    s[nextPosition] = temp;
                }
            }
            return s;
        }

        public void Dispose()
        {
            _cr.Dispose();
        }
    }
}
