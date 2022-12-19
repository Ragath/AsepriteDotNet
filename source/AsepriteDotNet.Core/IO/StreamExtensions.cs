/* -----------------------------------------------------------------------------
Copyright 2022 Christopher Whitley

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
----------------------------------------------------------------------------- */
using System.Buffers.Binary;

namespace AsepriteDotNet.Core.IO;

internal static class StreamExtensions
{
    private static readonly byte[] s_buffer = new byte[16];

    internal static byte ReadByteEx(this Stream stream)
    {
        ReadExactly(stream, s_buffer.AsSpan(0, 1));
        return s_buffer[0];
    }

    internal static byte[] ReadBytes(this Stream stream, int nBytes)
    {
        if (nBytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nBytes), "{nameof(count)} must be greater then 0");
        }

        if (nBytes == 0)
        {
            return Array.Empty<byte>();
        }

        byte[] result = new byte[nBytes];
        ReadExactly(stream, result);

        return result;
    }

    internal static byte[] ReadTo(this Stream stream, long pos)
    {
        int nBytes = (int)(pos - stream.Position);
        return ReadBytes(stream, nBytes);
    }

    internal static ushort ReadWord(this Stream stream)
    {
        ReadExactly(stream, s_buffer.AsSpan(0, 2));
        return BinaryPrimitives.ReadUInt16LittleEndian(s_buffer);
    }

    internal static short ReadShort(this Stream stream)
    {
        ReadExactly(stream, s_buffer.AsSpan(0, 2));
        return BinaryPrimitives.ReadInt16LittleEndian(s_buffer);
    }

    internal static uint ReadDword(this Stream stream)
    {
        ReadExactly(stream, s_buffer.AsSpan(0, 4));
        return BinaryPrimitives.ReadUInt32LittleEndian(s_buffer);
    }

    internal static int ReadLong(this Stream stream)
    {
        ReadExactly(stream, s_buffer.AsSpan(0, 4));
        return BinaryPrimitives.ReadInt32LittleEndian(s_buffer);
    }

    internal static float ReadFixed(this Stream stream)
    {
        ReadExactly(stream, s_buffer.AsSpan(0, 4));
        return BinaryPrimitives.ReadSingleLittleEndian(s_buffer);
    }

    internal static string ReadString(this Stream stream)
    {
        int nBytes = ReadWord(stream);              //  Length of string
        byte[] bytes = ReadBytes(stream, nBytes);   //  String as bytes

        try
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch (Exception ex)
        {
            throw new Exception("An exception occurred while encoding the data as a string. Please see inner exception for details", ex);
        }
    }

    internal static void ReadExactly(this Stream stream, Span<byte> buffer)
    {
        buffer.Clear();

        int total = 0;
        int read = 0;

        while (total < buffer.Length)
        {
            if ((read = stream.Read(buffer.Slice(total))) == 0)
            {
                throw new EndOfStreamException();
            }

            total += read;
        }
    }
}