using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCrypto
{
    public class SubStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly long _length;
        private readonly long _startIndex;

        public SubStream(Stream baseStream, long startIndex)
            : this(baseStream, startIndex, baseStream.Length - startIndex)
        { }

        public SubStream(Stream baseStream, long startIndex, long length)
        {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));
            if (!baseStream.CanRead) throw new ArgumentException($"{nameof(baseStream)} must be readable.");
            if (!baseStream.CanSeek) throw new ArgumentException($"{nameof(baseStream)} must be seekable.");
            _baseStream = baseStream;
            _startIndex = startIndex;
            _length = length;
            _baseStream.Position = startIndex;
        }

        public override bool CanRead { get; } = true;

        public override bool CanSeek { get; } = true;

        public override bool CanWrite { get; } = false;

        public override long Length { get { return _length; } }

        public override long Position
        {
            get
            {
                return _baseStream.Position - _startIndex;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }
        
        public override void Flush()
        {
            throw new InvalidOperationException("Stream does not support this method.");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset + count >= _startIndex + _length)
            {
                count = (int)(_length - offset);
            }
            if (count <= 0)
            {
                return 0;
            }

            return _baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long position = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:   position = 0L + offset; break;
                case SeekOrigin.Current: position = Position + offset; break;
                case SeekOrigin.End:     position = _length + offset; break;
            }
            if (position < 0) position = 0;
            if (position >= _length) position = _length - 1L;

            _baseStream.Position = position + _startIndex;
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("Stream does not support this method.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException("Stream does not support this method.");
        }
    }
}
