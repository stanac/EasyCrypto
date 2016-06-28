using System;
using System.Linq;

namespace EasyCrypto
{
    public static class DataTools
    {
        public static byte[] SkiptTake(this byte[] array, int skip, int take)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            byte[] ret = new byte[take];
            for (int i = skip, j = 0 ; i < skip + take; i++, j++)
            {
                ret[j] = array[i];
            }
            return ret;
        }

        public static byte[] JoinByteArrays(params byte[][] byteArrays)
        {
            byte[] ret = new byte[byteArrays.Sum(x => x.Length)];
            int position = 0;
            for (int i = 0; i < byteArrays.Length; i++)
            {
                byteArrays[i].CopyTo(ret, position);
                position += byteArrays[i].Length;
            }
            return ret;
        }

        public static bool CompareByteArrays(byte[] ba1, byte[] ba2)
        {
            if (ba1 == null) throw new ArgumentNullException(nameof(ba1));
            if (ba2 == null) throw new ArgumentNullException(nameof(ba2));

            if (ba1.Length != ba2.Length) return false;
            for (int i = 0; i < ba1.Length; i++)
            {
                if (ba1[i] != ba2[i]) return false;
            }
            return true;
        }
    }
}
