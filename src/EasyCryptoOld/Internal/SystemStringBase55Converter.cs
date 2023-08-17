using System;
using System.Linq;

namespace EasyCrypto.Internal
{
    internal static class SystemStringBase55Converter
    {
        public const string Charset = "23456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz";

        public static string ToString(long value, int pad = 0)
        {
            string result = "";
            int targetBase = Charset.Length;

            do
            {
                result = Charset[(int)(value % targetBase)] + result;
                value /= targetBase;
            }
            while (value > 0);

            if (pad > result.Length)
            {
                result = result.PadLeft(pad, Charset[0]);
            }

            return result;
        }

        public static long ToLong(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            value = new string(value.Reverse().ToArray());

            long result = 0;
            double baseValue = Charset.Length;

            for (long index = 0; index < value.Length; index++)
            {
                long charValue = Charset.IndexOf(value[(int)index]);
                result += charValue * (long)Math.Pow(baseValue, index);
            }

            return result;
        }
    }
}
