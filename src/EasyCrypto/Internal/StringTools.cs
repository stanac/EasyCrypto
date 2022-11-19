namespace EasyCrypto.Internal
{
    internal static class StringTools
    {
        public static string BeautifyBase64(this string s)
        {
            if (s == null) return null;

            return s
                .Replace("=", "")
                .Replace("/", "_")
                .Replace("+", "-");
        }

        public static string UglifyBase64(this string s)
        {
            if (s == null) return null;

            s = s.Replace("_", "/").Replace("-", "+");

            int mod = s.Length % 4;
            if (mod == 2) s += "==";
            if (mod == 3) s += "=";

            return s;
        }
    }
}
