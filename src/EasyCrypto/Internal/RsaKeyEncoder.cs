using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EasyCrypto.Internal
{
    internal static class RsaKeyEncoder
    {
        public static RSAParameters Decode(string p)
        {
            var parts = p.Split('.');

            string version = parts[0];
            if (version != "00")
            {
                throw new ArgumentException("Version is not supported");
            }

            byte[][] pbytes = parts.Skip(1)
                .Select(x => Convert.FromBase64String(x))
                .ToArray();

            return SetParameters(pbytes);
        }

        public static string Encode(RSAParameters p)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("00"); // version

            var parms = GetParameters(p);
            foreach (var rsap in parms)
            {
                sb.Append(".");
                if (rsap != null && rsap.Length > 0)
                {
                    sb.Append(Convert.ToBase64String(rsap));
                }
            }

            return sb.ToString();
        }

        private static IEnumerable<byte[]> GetParameters(RSAParameters p)
        {
            yield return p.D;
            yield return p.DP;
            yield return p.DQ;
            yield return p.Exponent;
            yield return p.InverseQ;
            yield return p.Modulus;
            yield return p.P;
            yield return p.Q;
        }

        private static RSAParameters SetParameters(byte[][] p)
        {
            var rsap = new RSAParameters();
            if (p.Length != 8) throw new ArgumentException("Array of byte arrays does not contain valid number of elements.");

            rsap.D = p[0];
            rsap.DP = p[1];
            rsap.DQ = p[2];
            rsap.Exponent = p[3];
            rsap.InverseQ = p[4];
            rsap.Modulus = p[5];
            rsap.P = p[6];
            rsap.Q = p[7];

            if (p.Any(x => x.Length == 0))
            {
                rsap.D = null;
                rsap.DP = null;
                rsap.DQ = null;
                rsap.InverseQ = null;
                rsap.P = null;
                rsap.Q = null;
            }

            return rsap;
        }
    }
}
