using System;
using System.Diagnostics;
using System.IO;

// Source: https://en.wikibooks.org/wiki/Algorithm_Implementation/Miscellaneous/Base64

namespace Modules.Hive.Io.Base64
{
    public class Base64Writer : IDisposable
    {
        int buffer = 0;
        int bufferIndex = 0;
        bool isDisposed = false;
        bool isFlushed = true;


        public Stream BaseStream { get; }


        public bool AutoDisposeBaseStream { get; }


        public Base64Writer(Stream output, bool autoDisposeBaseStream = false)
        {
            BaseStream = output;
            AutoDisposeBaseStream = autoDisposeBaseStream;
        }


        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            Flush();
            isDisposed = true;

            if (AutoDisposeBaseStream)
            {
                BaseStream.Dispose();
            }
        }


        public void Write(byte value)
        {
            AssertThisNotDisposed();

            // save to buffer and convert to big endian
            isFlushed = false;
            buffer |= value << (16 - bufferIndex * 8);
            bufferIndex++;

            // flush completed buffer
            if (bufferIndex == 3)
            {
                Flush();
            }
        }


        public void Write(byte[] buffer, int offset, int count)
        {
            int edgeIndex = offset + count;
            for (int i = offset; i < edgeIndex; i++)
            {
                Write(buffer[i]);
            }
        }


        public void Flush()
        {
            if (isFlushed)
            {
                return;
            }

            switch (bufferIndex % 3)
            {
                case 0:
                    BaseStream.WriteByte((byte)Base64CharSet.EncodeLookup[(buffer & 0x00FC0000) >> 18]);
                    BaseStream.WriteByte((byte)Base64CharSet.EncodeLookup[(buffer & 0x0003F000) >> 12]);
                    BaseStream.WriteByte((byte)Base64CharSet.EncodeLookup[(buffer & 0x00000FC0) >> 6]);
                    BaseStream.WriteByte((byte)Base64CharSet.EncodeLookup[(buffer & 0x0000003F)]);
                    bufferIndex = 0;
                    buffer = 0;
                    break;

                case 1:
                    long pos = BaseStream.Position; // keep position to overwrite incomplete buffer
                    BaseStream.WriteByte((byte)Base64CharSet.EncodeLookup[(buffer & 0x00FC0000) >> 18]);
                    BaseStream.WriteByte((byte)Base64CharSet.EncodeLookup[(buffer & 0x0003F000) >> 12]);
                    BaseStream.WriteByte(Base64CharSet.PadCharacter);
                    BaseStream.WriteByte(Base64CharSet.PadCharacter);
                    BaseStream.Position = pos;
                    break;

                case 2:
                    pos = BaseStream.Position; // keep position to overwrite incomplete buffer
                    BaseStream.WriteByte((byte)Base64CharSet.EncodeLookup[(buffer & 0x00FC0000) >> 18]);
                    BaseStream.WriteByte((byte)Base64CharSet.EncodeLookup[(buffer & 0x0003F000) >> 12]);
                    BaseStream.WriteByte((byte)Base64CharSet.EncodeLookup[(buffer & 0x00000FC0) >> 6]);
                    BaseStream.WriteByte(Base64CharSet.PadCharacter);
                    BaseStream.Position = pos;
                    break;
            }

            isFlushed = true;
        }


        [Conditional("DEBUG")]
        void AssertThisNotDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(Base64Writer));
        }
    }
}
