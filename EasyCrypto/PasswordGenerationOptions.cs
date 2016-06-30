using System;
using System.Linq;

namespace EasyCrypto
{
    /// <summary>
    /// Options that are used for <see cref="PasswordGenerator"/>
    /// </summary>
    public class PasswordGenerationOptions
    {
        /// <summary>
        /// Length of password, default is 16
        /// </summary>
        public uint Length { get; private set; } = 16;

        /// <summary>
        /// Upper case chars to use, default is "QWERTYUIOPASDFGHJKLZXCVBNM"
        /// </summary>
        public string ValidUpperCase { get; private set; } = "QWERTYUIOPASDFGHJKLZXCVBNM";

        /// <summary>
        /// Lower case chars to use, default is "qwertyuiopasdfghjklzxcvbnm"
        /// </summary>
        public string ValidLowerCase { get; private set; } = "qwertyuiopasdfghjklzxcvbnm";
        
        /// <summary>
        /// Symbols to use, default is "!@#$%^&amp;*_+-" (without quotes)
        /// </summary>
        public string ValidSymbols { get; private set; } = "!@#$%^&*_+-";

        /// <summary>
        /// Minimum number of upper case to use, maximum will be set to at least twice as much (zero means don't use it), default is 4
        /// </summary>
        public uint MinUpperCase { get; private set; } = 4;

        /// <summary>
        /// Minimum number of lower case to use, maximum will be set to at least twice as much (zero means don't use it), default is 4
        /// </summary>
        public uint MinLowerCase { get; private set; } = 4;

        /// <summary>
        /// Minimum number of numbers to use, maximum will be set to at least twice as much (zero means don't use it), default is 2
        /// </summary>
        public uint MinNumbers { get; private set; } = 2;

        /// <summary>
        /// Minimum number of symbols to use, maximum will be set to at least twice as much (zero means don't use it), default is 2
        /// </summary>
        public uint MinSymbols { get; private set; } = 2;

        /// <summary>
        /// Sets length and adjusts MinUpperCase, MinLowerCase, MinNumbers and MinSymbols randomly if needed
        /// </summary>
        /// <param name="length">length of the password to generate, cannot be less than 4</param>
        /// <returns>this instance</returns>
        public PasswordGenerationOptions SetLength(uint length)
        {
            if (length < 4) throw new ArgumentException($"{nameof(length)} cannot be less than 4");
            Length = length;
            SetMinimumsByLength();
            return this;
        }

        /// <summary>
        /// Sets upper case chars to use
        /// </summary>
        /// <param name="upperCase"></param>
        /// <returns>this instance</returns>
        public PasswordGenerationOptions UseUpperCase(string upperCase)
        {
            ValidateParameter(upperCase, nameof(upperCase));
            ValidUpperCase = upperCase;
            return this;
        }

        /// <summary>
        /// Sets lower case chars to use
        /// </summary>
        /// <param name="lowerCase"></param>
        /// <returns>this instance</returns>
        public PasswordGenerationOptions UseLowerCase(string lowerCase)
        {
            ValidateParameter(lowerCase, nameof(lowerCase));
            ValidLowerCase = lowerCase;
            return this;
        }

        /// <summary>
        /// Sets symbol chars to use
        /// </summary>
        /// <param name="symbols"></param>
        /// <returns>this instance</returns>
        public PasswordGenerationOptions UseSymbols(string symbols)
        {
            ValidateParameter(symbols, nameof(symbols));
            ValidSymbols = symbols;
            return this;
        }

        /// <summary>
        /// Sets <see cref="MinUpperCase"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns>this instance</returns>
        public PasswordGenerationOptions SetMinUpperCase(uint value)
        {
            MinUpperCase = value;
            return this;
        }

        /// <summary>
        /// Sets <see cref="MinLowerCase"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns>this instance</returns>
        public PasswordGenerationOptions SetMinLowerCase(uint value)
        {
            MinLowerCase = value;
            return this;
        }

        /// <summary>
        /// Sets <see cref="MinNumbers"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns>this instance</returns>
        public PasswordGenerationOptions SetMinNumbers(uint value)
        {
            MinNumbers = value;
            return this;
        }

        /// <summary>
        /// Sets <see cref="MinUpperCase"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns>this instance</returns>
        public PasswordGenerationOptions SetMinSymbols(uint value)
        {
            MinSymbols = value;
            return this;
        }

        /// <summary>
        /// Default options, length 16, min 4 lower case, min 4 upper case, min 2 number and min 2 symbols
        /// </summary>
        public static PasswordGenerationOptions Default => new PasswordGenerationOptions();

        /// <summary>
        /// Validates options
        /// </summary>
        /// <param name="message">returns message containing error</param>
        /// <returns>Boolean, true if options are valid</returns>
        public bool AreValid(out string message)
        {
            message = null;
            if (MinLowerCase + MinNumbers + MinSymbols + MinUpperCase > Length)
            {
                message = "Following condition is not satisfied: MinLowerCase + MinNumbers + MinSymbols + MinUpperCase <= Length";
            }
            return message == null;
        }

        /// <summary>
        /// Validates char array parameter
        /// </summary>
        /// <param name="value">Value of the parameter</param>
        /// <param name="name">Name of the parameter</param>
        private void ValidateParameter(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)
                || value.Any(char.IsWhiteSpace)
                || value.Length != value.Distinct().Count()
                || value.Length < 2)
            {
                throw new ArgumentException($"Parameter {name} must have at least 2 distinct characters and no whitespace. Any repeating character is not allowed.");
            }
        }

        /// <summary>
        /// Generates actual options from this
        /// </summary>
        /// <returns>ActualPasswordGeneratorOptions</returns>
        internal ActualPasswordGeneratorOptions GetActuals()
        {
            var options = new ActualPasswordGeneratorOptions
            {
                Length = (int)Length,
                Lower = ValidLowerCase,
                Upper = ValidUpperCase,
                Symbols = ValidSymbols,
                LowerLength = (int)MinLowerCase,
                UpperLength = (int)MinUpperCase,
                NumbersLength = (int)MinNumbers,
                SymbolsLength = (int)MinSymbols
            };

            int maxLower = 2 * options.LowerLength;
            int maxUpper = 2 * options.UpperLength;
            int maxNumbers = 2 * options.NumbersLength;
            int maxSymbols = 2 * options.SymbolsLength;

            using (var cr = new CryptoRandom())
            {
                while (maxLower + maxUpper + maxNumbers + maxSymbols < options.Length)
                {
                    switch (cr.NextInt(4))
                    {
                        case 0:
                            if (maxLower != 0) maxLower++;
                            break;
                        case 1:
                            if (maxUpper != 0) maxUpper++;
                            break;
                        case 2:
                            if (maxSymbols != 0) maxSymbols++;
                            break;
                        case 3:
                            if (maxNumbers != 0) maxNumbers++;
                            break;

                        default:
                            throw new IndexOutOfRangeException();
                    }
                }

                Func<bool> canAddLower = () => maxLower > options.LowerLength;
                Func<bool> canAddUpper = () => maxUpper > options.UpperLength;
                Func<bool> canAddNumber = () => maxNumbers > options.NumbersLength;
                Func<bool> canAddSymbol = () => maxSymbols > options.SymbolsLength;

                while (options.Length != options.UpperLength + options.LowerLength + options.NumbersLength + options.SymbolsLength)
                {
                    switch(cr.NextInt(4))
                    {
                        case 0:
                            if (canAddLower()) options.LowerLength++;
                            break;
                        case 1:
                            if (canAddUpper()) options.UpperLength++; 
                                break;
                        case 2:
                            if (canAddSymbol()) options.SymbolsLength++;
                                break;
                        case 3:
                            if (canAddNumber()) options.NumbersLength++;
                                break;

                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
            }

            return options;
        }

        private void SetMinimumsByLength()
        {
            Action decrementUpper = () => { if (MinUpperCase > 0) MinUpperCase--; };
            Action decrementLower = () => { if (MinLowerCase > 0) MinLowerCase--; };
            Action decrementSymbols = () => { if (MinSymbols > 0) MinSymbols--; };
            Action decrementNumbers = () => { if (MinNumbers > 0) MinNumbers--; };

            using (var cr = new CryptoRandom())
            {
                while (MinLowerCase + MinUpperCase + MinSymbols + MinNumbers > Length)
                {
                    switch (cr.NextInt(4))
                    {
                        case 0:
                            decrementLower();
                            break;
                        case 1:
                            decrementNumbers();
                            break;
                        case 2:
                            decrementSymbols();
                            break;
                        case 3:
                            decrementUpper();
                            break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }
        }
    }
}
