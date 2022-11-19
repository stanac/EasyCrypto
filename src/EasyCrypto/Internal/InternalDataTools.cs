using System.IO;
using System.Linq;
using System;

namespace EasyCrypto.Internal
{
    public static class InternalDataTools
    {
        /// <summary>
        /// Skips and take desired number of bytes.
        /// </summary>
        /// <param name="array">The array used for source.</param>
        /// <param name="skip">Number of bytes to skip (start index).</param>
        /// <param name="take">Number of bytes to take.</param>
        /// <returns>Byte array</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static byte[] SkipTake(this byte[] array, int skip, int take)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            byte[] ret = new byte[take];
            for (int i = skip, j = 0; i < skip + take; i++, j++)
            {
                ret[j] = array[i];
            }
            return ret;
        }

        /// <summary>
        /// Joins the byte arrays.
        /// </summary>
        /// <param name="byteArrays">The byte arrays to join.</param>
        /// <returns>Joined byte array to one byte array</returns>
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

        /// <summary>
        /// Compares two byte arrays.
        /// </summary>
        /// <param name="ba1">Byte array 1.</param>
        /// <param name="ba2">Byte array 2.</param>
        /// <returns>Bool, true if arrays are equal.</returns>
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

        /// <summary>
        /// Reads whole stream and returns bytes of the stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>Byte array, from the stream</returns>
        public static byte[] ToBytes(this Stream stream)
        {
            long position = stream.Position;
            byte[] data = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(data, 0, data.Length);
            stream.Position = position;
            return data;
        }
    }
}
