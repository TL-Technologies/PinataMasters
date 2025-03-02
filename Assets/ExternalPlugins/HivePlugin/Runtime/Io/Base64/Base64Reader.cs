using System;
using System.Diagnostics;
using System.IO;

// Source: https://en.wikibooks.org/wiki/Algorithm_Implementation/Miscellaneous/Base64

namespace Modules.Hive.Io.Base64
{
    public class Base64Reader : IDisposable
    {
        int buffer = 0;
        int bufferLength = 0;
        bool isDisposed = false;


        public Stream BaseStream { get; }


        public bool AutoDisposeBaseStream { get; }


        public Base64Reader(Stream input, bool autoDisposeBaseStream = false)
        {
            BaseStream = input;
            AutoDisposeBaseStream = autoDisposeBaseStream;
        }


        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            if (AutoDisposeBaseStream)
            {
                BaseStream.Dispose();
            }
        }


        public int ReadByte()
        {
            AssertThisNotDisposed();

            if (bufferLength < 8)
            {
                // read block of 4 ascii chars
                for (int i = 0; i < 4; i++)
                {
                    int mapIndex = BaseStream.ReadByte();
                    if (mapIndex < 0)
                    {
                        // end of stream reached
                        if (i == 0)
                        {
                            return -1;
                        }

                        // unexpected end of stream
                        throw new EndOfStreamException("Unexpected end of stream.");
                    }

                    // ignore pad-symbols
                    if (mapIndex == Base64CharSet.PadCharacter)
                    {
                        continue;
                    }

                    // check
                    byte decoded = Base64CharSet.DecodeLookup[mapIndex];
                    if (decoded >= Base64CharSet.MaxSize)
                    {
                        throw new FormatException("Non-valid character in base64");
                    }

                    // extract 6-bit of real data and save it to buffer
                    buffer <<= 6;
                    bufferLength += 6;
                    buffer |= decoded;
                }
            }

            bufferLength -= 8;
            return (buffer >> bufferLength) & 0xFF;
        }


        public int Read(byte[] buffer, int offset, int count)
        {
            int edgeIndex = offset + count;
            count = 0;
            for (int i = offset; i < edgeIndex; i++)
            {
                int data = ReadByte();
                if (data < 0)
                    break;

                buffer[i] = (byte)data;
                count++;
            }

            return count;
        }


        [Conditional("DEBUG")]
        void AssertThisNotDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(Base64Reader));
        }
    }
}
