using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyCrypto.Internal
{
    internal class FormattedBinaryData
    {
        internal const int WriterFormatVersion = 1;
        private readonly List<Type> _types;
        private readonly int _numberOfArrays;
        private readonly int _magicNumber;

        private static readonly IReadOnlyList<Type> _supportedTypes = new List<Type>
        {
            typeof(int),
            typeof(int[]),
            typeof(byte[])
        };

        public FormattedBinaryData(int magicNumber, params Type[] types)
            : this(magicNumber, types?.ToList())
        {
        }

        public FormattedBinaryData(int magicNumber, List<Type> types)
        {
            _types = types ?? throw new ArgumentNullException(nameof(types));
            _magicNumber = magicNumber;

            if (types.Count == 0)
            {
                throw new ArgumentException("Empty collection not supported.", nameof(types));
            }

            foreach (Type type in _types)
            {
                if (type == null)
                {
                    throw new ArgumentException("Collection cannot contain null type", nameof(types));
                }

                if (!_supportedTypes.Contains(type))
                {
                    throw new ArgumentException($"{type.Name} is not supported types.", nameof(type));
                }

                if (IsArray(type))
                {
                    _numberOfArrays++;
                }
            }
        }
        
        // Binary format:
        //  Header:
        //      4 bytes - Magic number
        //      4 bytes - writer format
        //      4 bytes - number of arrays
        //      x bytes - used for array lengths
        //          if array types are present they will be set here
        //  Data:
        //      x bytes according to array lengths

        public byte[] ToBytes(params object[] data)
        {
            ValidateObjects(data);
            
            int length = 12; // magic number + writer format + number of arrays
            length += _numberOfArrays * 4; // arrays lengths
            
            for (int i = 0; i < data.Length; i++)
            {
                if (IsArray(_types[i]))
                {
                    length += GetArrayLength(data[i]);
                }
                else
                {
                    length += 4;
                }
            }

            byte[] ret = new byte[length];

            WriteHeader(ret, data);
            WriteData(ret, data);

            return ret;
        }

        public object[] Read(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length < 12)
            {
                throw new ArgumentException("data too short", nameof(data));
            }

            int magicNumber = BitConverter.ToInt32(data, 0);
            int writerFormat = BitConverter.ToInt32(data, 4);
            int numberOfArrays = BitConverter.ToInt32(data, 8);

            if (magicNumber != _magicNumber) throw new InvalidOperationException($"Magic number does not match, expected {_magicNumber}, got {magicNumber}");
            if (writerFormat != WriterFormatVersion) throw new InvalidOperationException($"Magic number does not match, expected {_magicNumber}, got {magicNumber}");
            if (numberOfArrays != _numberOfArrays) throw new InvalidOperationException($"Array number does not match, expected {_numberOfArrays}, got {numberOfArrays}");

            Queue<int> arrayLengths = new Queue<int>();

            int position = 12;

            for (int i = 0; i < numberOfArrays; i++)
            {
                arrayLengths.Enqueue(BitConverter.ToInt32(data, position));
                position += 4;
            }

            object[] result = new object[_types.Count];
            
            for (int i = 0; i < result.Length; i++)
            {
                if (IsArray(_types[i]))
                {
                    int length = arrayLengths.Dequeue();

                    if (_types[i] == typeof(int[]))
                    {
                        length /= 4;
                    }

                    result[i] = ReadArray(_types[i], length, data, ref position);
                }
                else
                {
                    result[i] = ReadScalar(_types[i], data, ref position);
                }
            }

            return result;
        }

        private void ValidateObjects(object[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.Length != _types.Count)
            {
                throw new InvalidOperationException("Data length does not match types length");
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] is null)
                {
                    throw new ArgumentException("Collection cannot contain null objects", nameof(data));
                }

                Type t = data[i].GetType();

                if (t != _types[i])
                {
                    throw new ArgumentException($"Object at index `{i}` does not match to provided type", nameof(data));
                }
            }
        }

        private bool IsArray(Type type)
        {
            return type == typeof(int[]) || type == typeof(byte[]);
        }

        private int GetArrayLength(object o)
        {
            if (o is int[] intArray)
            {
                return intArray.Length * 4;
            }

            if (o is byte[] byteArray)
            {
                return byteArray.Length;
            }

            throw new InvalidOperationException("Unsupported type " + o.GetType().Name);
        }

        private void WriteHeader(byte[] result, object[] data)
        {
            BitConverter.GetBytes(_magicNumber).CopyTo(result, 0);
            BitConverter.GetBytes(WriterFormatVersion).CopyTo(result, 4);
            BitConverter.GetBytes(_numberOfArrays).CopyTo(result, 8);

            int position = 12;

            for (var i = 0; i < data.Length; i++)
            {
                if (IsArray(_types[i]))
                {
                    int length = GetArrayLength(data[i]);

                    BitConverter.GetBytes(length).CopyTo(result, position);
                    position += 4;
                }
            }
        }

        private void WriteData(byte[] result, object[] data)
        {
            int position = 12 + _numberOfArrays * 4;

            for (int i = 0; i < data.Length; i++)
            {
                if (IsArray(_types[i]))
                {
                    WriteArray(data[i], result, ref position);
                }
                else
                {
                    WriteScalar(data[i], result, ref position);
                }
            }
        }

        private void WriteArray(object o, byte[] result, ref int position)
        {
            if (o is int[] intArray)
            {
                foreach (int i in intArray)
                {
                    BitConverter.GetBytes(i).CopyTo(result, position);
                    position += 4;
                }
            }
            else if (o is byte[] byteArray)
            {
                byteArray.CopyTo(result, position);
                position += byteArray.Length;
            }
            else
            {
                throw new InvalidOperationException("Unsupported type " + o.GetType().Name);
            }
        }

        private void WriteScalar(object o, byte[] result, ref int position)
        {
            if (o is int i)
            {
                BitConverter.GetBytes(i).CopyTo(result, position);
                position += 4;
            }
            else
            {
                throw new InvalidOperationException("Unsupported type: " + o.GetType().Name);
            }
        }

        private object ReadArray(Type t, int length, byte[] data, ref int position)
        {
            if (t == typeof(int[]))
            {
                int[] ret = new int[length];

                for (int i = 0; i < length; i++)
                {
                    ret[i] = (int)ReadScalar(typeof(int), data, ref position);
                }

                return ret;
            }
            
            if (t == typeof(byte[]))
            {
                byte[] ret = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    ret[i] = (byte)ReadScalar(typeof(byte), data, ref position);
                }

                return ret;
            }
            
            throw new NotSupportedException("Unsupported type " + t.Name);
        }

        private object ReadScalar(Type t, byte[] data, ref int position)
        {
            if (t == typeof(int))
            {
                object result = BitConverter.ToInt32(data, position);
                position += 4;
                return result;
            }

            if (t == typeof(byte))
            {
                object result = data[position];
                position += 1;
                return result;
            }

            throw new InvalidOperationException("Unsupported type " + t.Name);
        }
    }
}
