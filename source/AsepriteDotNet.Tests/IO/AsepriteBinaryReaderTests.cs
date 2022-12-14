/* ----------------------------------------------------------------------------
MIT License

Copyright (c) 2022 Christopher Whitley

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
---------------------------------------------------------------------------- */
using System.Text;

using AsepriteDotNet.IO;

namespace AsepriteDotNet.Tests;

public class StreamExtensionTests
{

    [Fact]
    public void StreamExtensions_EndOfStreamTest_Negative()
    {
        ValidateEndOfStreamException(writer => writer.Write(byte.MinValue), stream => stream.ReadByteEx());
        ValidateEndOfStreamException(writer => writer.Write(byte.MaxValue), stream => stream.ReadByteEx());
        ValidateEndOfStreamException(writer => writer.Write(new byte[] { 0 }), stream => stream.ReadBytes(1));
        ValidateEndOfStreamException(writer => writer.Write(ushort.MinValue), stream => stream.ReadWord());
        ValidateEndOfStreamException(writer => writer.Write(ushort.MaxValue), stream => stream.ReadWord());
        ValidateEndOfStreamException(writer => writer.Write(short.MinValue), stream => stream.ReadShort());
        ValidateEndOfStreamException(writer => writer.Write(short.MaxValue), stream => stream.ReadShort());
        ValidateEndOfStreamException(writer => writer.Write(uint.MinValue), stream => stream.ReadDword());
        ValidateEndOfStreamException(writer => writer.Write(uint.MaxValue), stream => stream.ReadDword());
        ValidateEndOfStreamException(writer => writer.Write(int.MinValue), stream => stream.ReadLong());
        ValidateEndOfStreamException(writer => writer.Write(int.MaxValue), stream => stream.ReadLong());
        ValidateEndOfStreamException(writer => writer.Write(float.MinValue), stream => stream.ReadFixed());
        ValidateEndOfStreamException(writer => writer.Write(float.MaxValue), stream => stream.ReadFixed());
        ValidateEndOfStreamException(writer => WriteValidAsepriteString(writer, string.Empty), stream => stream.ReadString());
        ValidateEndOfStreamException(writer => WriteValidAsepriteString(writer, "Hello World"), stream => stream.ReadString());

    }

    [Fact]
    public void StreamExtensions_ReadByteEx()
    {
        ValidateRead(writer => writer.Write(byte.MinValue), stream => stream.ReadByteEx(), byte.MinValue);
        ValidateRead(writer => writer.Write(byte.MaxValue), stream => stream.ReadByteEx(), byte.MaxValue);
    }

    [Fact]
    public void StreamExtensions_ReadBytes()
    {
        byte[] expected = new byte[] { 0x0000, 0x0001, 0x0002, 0x0003 };

        ValidateRead(writer =>
        {
            for (int i = 0; i < expected.Length; i++)
            {
                writer.Write(expected[i]);
            }
        }, reader => reader.ReadBytes(expected.Length), expected);
    }

    [Fact]
    public void StreamExtensions_ReadWord()
    {
        ValidateRead(writer => writer.Write(ushort.MinValue), stream => stream.ReadWord(), ushort.MinValue);
        ValidateRead(writer => writer.Write(ushort.MaxValue), stream => stream.ReadWord(), ushort.MaxValue);
    }

    [Fact]
    public void StreamExtensions_ReadShort()
    {
        ValidateRead(writer => writer.Write(short.MinValue), stream => stream.ReadShort(), short.MinValue);
        ValidateRead(writer => writer.Write(short.MaxValue), stream => stream.ReadShort(), short.MaxValue);
    }

    [Fact]
    public void StreamExtensions_ReadDword()
    {
        ValidateRead(writer => writer.Write(uint.MinValue), stream => stream.ReadDword(), uint.MinValue);
        ValidateRead(writer => writer.Write(uint.MaxValue), stream => stream.ReadDword(), uint.MaxValue);
    }

    [Fact]
    public void StreamExtensions_ReadLong()
    {
        ValidateRead(writer => writer.Write(int.MinValue), stream => stream.ReadLong(), int.MinValue);
        ValidateRead(writer => writer.Write(int.MaxValue), stream => stream.ReadLong(), int.MaxValue);
    }

    [Fact]
    public void StreamExtensions_ReadFixed()
    {
        ValidateRead(writer => writer.Write(float.MinValue), stream => stream.ReadFixed(), float.MinValue);
        ValidateRead(writer => writer.Write(float.MaxValue), stream => stream.ReadFixed(), float.MaxValue);
    }

    [Fact]
    public void StreamExtensions_ReadString()
    {
        string expected = "hello world";
        ValidateRead(writer => WriteValidAsepriteString(writer, expected), stream => stream.ReadString(), expected);
    }


    private void ValidateRead<T>(Action<BinaryWriter> writeAction, Func<Stream, T> readAction, T expected)
    {
        UTF8Encoding encoding = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        MemoryStream stream = new();

        //  Write value once to stream.
        BinaryWriter writer = new(stream, encoding, leaveOpen: true);
        writeAction(writer);

        //  Stream will be left open
        writer.Close();

        //  Ensure the stream was populated.
        Assert.True(stream.Length > 0);

        //  Reset stream position
        stream.Position = 0;

        //  Read the value back
        T actual = readAction(stream);
        Assert.Equal(expected, actual);
        Assert.IsType<T>(actual);
    }

    private void WriteValidAsepriteString(BinaryWriter writer, string s)
    {
        UTF8Encoding encoding = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        byte[] arr = encoding.GetBytes(s);
        writer.Write((ushort)arr.Length);

        for (int i = 0; i < arr.Length; i++)
        {
            writer.Write(arr[i]);
        }
    }

    private void ValidateEndOfStreamException(Action<BinaryWriter> writeAction, Action<Stream> readAction)
    {
        UTF8Encoding encoding = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        MemoryStream stream = new();

        //  Write twice to the memory stream.
        BinaryWriter writer = new(stream, encoding, leaveOpen: true);
        writeAction(writer);
        writeAction(writer);
        writer.Close(); //  Stream will be left open

        //  Ensure that the stream was populated
        Assert.True(stream.Length > 0);

        //  Reset the position within the stream
        stream.Position = 0;

        //  Truncate the last byte
        stream.SetLength(stream.Length - 1);

        //  First one should always succeed.
        readAction(stream);

        Exception ex = Record.Exception(() => readAction(stream));
        Assert.NotNull(ex);
        Assert.IsType<EndOfStreamException>(ex);

        //  Second one should fail since we truncated the last byte
        //Assert.Throws<EndOfStreamException>(() => readAction(stream));
    }
}