using System;
using System.Diagnostics;
using System.IO;

namespace Modules.Hive.Io.Base64
{
    public class Base64Stream : Stream
    {
        readonly Stream baseStream;
        readonly Base64StreamMode mode;
        readonly Base64Reader reader;
        readonly Base64Writer writer;
        bool isDisposed = false;


        public override bool CanRead => mode == Base64StreamMode.Decode && baseStream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => mode == Base64StreamMode.Encode && baseStream.CanWrite;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }


        public Base64Stream(Stream baseStream, Base64StreamMode mode)
        {
            this.baseStream = baseStream;
            this.mode = mode;

            switch (mode)
            {
                case Base64StreamMode.Decode:
                    reader = new Base64Reader(baseStream, false);
                    break;

                case Base64StreamMode.Encode:
                    writer = new Base64Writer(baseStream, false);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                isDisposed = true;
                reader?.Dispose();
                writer?.Dispose();
            }

            base.Dispose(disposing);
        }


        public override int ReadByte()
        {
            AssertThisNotDisposed();
            AssertCanRead();

            return reader.ReadByte();
        }


        public override void WriteByte(byte value)
        {
            AssertThisNotDisposed();
            AssertCanWrite();

            writer.Write(value);
        }


        public override int Read(byte[] buffer, int offset, int count)
        {
            AssertThisNotDisposed();
            AssertCanRead();

            return reader.Read(buffer, offset, count);
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            AssertThisNotDisposed();
            AssertCanWrite();

            writer.Write(buffer, offset, count);
        }


        public override void Flush()
        {
            AssertThisNotDisposed();
            AssertCanWrite();

            writer.Flush();
            baseStream.Flush();
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }


        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }


        [Conditional("DEBUG")]
        void AssertThisNotDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(Base64Stream));
        }


        [Conditional("DEBUG")]
        void AssertCanRead()
        {
            if (!CanRead)
                throw new NotSupportedException(nameof(Base64Stream));
        }


        [Conditional("DEBUG")]
        void AssertCanWrite()
        {
            if (!CanWrite)
                throw new NotSupportedException(nameof(Base64Stream));
        }
    }
}
