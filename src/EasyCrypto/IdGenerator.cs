using System;

namespace EasyCrypto
{
    /// <summary>
    /// Id generator that has two mandatory parts (time base part, random part) and
    /// one optional part (fixed part), 
    /// generated id templateId without hyphens is {timePart}{fixedPart}{randomPart}
    /// generated id templateId wit hyphens is {timePart}-{fixedPart}-{randomPart}
    /// </summary>
    public class IdGenerator
    {
        private static readonly Random _rand = new Random(GetSeed());
        private static readonly CryptoRandom _cryptoRand = new CryptoRandom();
        
        /// <summary>
        /// Default instance of Id generator with FastRandom = true, FixedPart = "", RandomPartLength = 6 and AddHyphens = false
        /// </summary>
        public static IdGenerator Default { get; } = new IdGenerator();

        /// <summary>
        /// Fixed part that is set in the middle of the generated Id
        /// </summary>
        public string FixedPart { get; }

        /// <summary>
        /// If true, System.Random is used for random part, if false EasyCrypto.CryptoRandom is used
        /// </summary>
        public bool FastRandom { get; }

        private int _randomPartLength = 6;

        /// <summary>
        /// Length of random part, default value is 6, must not be less than 4 or greater than 100
        /// </summary>
        public int RandomPartLength
        {
            get => _randomPartLength;
            set
            {
                if (value < 4) throw new ArgumentOutOfRangeException("Random part length cannot be less than 4");
                if (value > 100) throw new ArgumentOutOfRangeException("Random part length cannot be greater than 100");
                _randomPartLength = value;
            }
        }

        /// <summary>
        /// If true hyphens (-) are added between parts
        /// </summary>
        public bool AddHyphens { get; set; }

        /// <summary>
        /// Default constructor, FixedPart = "", FastRandom = true
        /// </summary>
        public IdGenerator() : this("", true)
        {
        }

        /// <summary>
        /// Constructor to accept boolean value telling the generator to use or not to use fast random
        /// </summary>
        /// <param name="fastRandom">If true System.Random is used, otherwise EasyCrypto.CryptoRandom is used</param>
        public IdGenerator(bool fastRandom): this("", fastRandom)
        {
        }

        /// <summary>
        /// Constructor to accept boolean value telling the generator to use or not to use fast random and
        /// fixed part of generated id
        /// </summary>
        /// <param name="fixedPart">Fixed part to set in middle of generated id</param>
        /// <param name="fastRandom">If true System.Random is used, otherwise EasyCrypto.CryptoRandom is used</param>
        public IdGenerator(string fixedPart, bool fastRandom)
        {
            FixedPart = (fixedPart ?? "").Trim();
            FastRandom = fastRandom;
        }

        /// <summary>
        /// Generates new id string where for time part current UTC time is used
        /// </summary>
        /// <returns>String, generated id</returns>
        public string NewId() => NewId(DateTime.UtcNow);

        /// <summary>
        /// Generates new id
        /// </summary>
        /// <param name="currentTime">Current time</param>
        /// <returns>String, generated id</returns>
        public string NewId(DateTime currentTime)
        {
            if (AddHyphens)
            {
                if (FixedPart.Length > 0)
                {
                    return $"{GetTimePart(currentTime)}-{FixedPart}-{GetRandomPart()}";
                }

                return $"{GetTimePart(currentTime)}-{GetRandomPart()}";
            }

            return GetTimePart(currentTime) + FixedPart + GetRandomPart();
        }

        private string GetTimePart(DateTime t)
        {
            long offset = DateTimeOriginOffset.GetOffset(t);
            return SystemStringBase55Converter.ToString(offset, 8);
        }

        private string GetRandomPart()
        {
            char[] ret = new char[RandomPartLength];

            int index;
            if (FastRandom)
            {
                for (int i = 0; i < ret.Length; i++)
                {
                    index = _rand.Next(SystemStringBase55Converter.Charset.Length);
                    ret[i] = SystemStringBase55Converter.Charset[index];
                }
            }
            else
            {
                for (int i = 0; i < ret.Length; i++)
                {
                    index = _cryptoRand.NextInt(SystemStringBase55Converter.Charset.Length);
                    ret[i] = SystemStringBase55Converter.Charset[index];
                }
            }

            return new string(ret);
        }

        private static int GetSeed()
        {
            int result = 0;
            byte[] g = Guid.NewGuid().ToByteArray();
            unchecked
            {
                int i0 = BitConverter.ToInt32(g, 0);
                int i1 = BitConverter.ToInt32(g, 4);
                int i2 = BitConverter.ToInt32(g, 8);
                int i3 = BitConverter.ToInt32(g, 12);

                result = i0 + i1 * 3 + i2 * 7 + i3 * 11;
                result = Math.Abs(result);
            }
            return result;
        }
    }
}
