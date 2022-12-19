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
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

using AsepriteDotNet.Core.Color;

namespace AsepriteDotNet.Core.Serialization;

public class SpritesheetConverter : JsonConverter<Spritesheet>
{
    public override Spritesheet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        int frameCount = 0;
        List<Frame> frames;
        int width = 0;
        int height = 0;

        while (reader.Read())
        {
            string propertyName = string.Empty;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                propertyName = reader.GetString()!;
            }

            if (propertyName == "frameCount")
            {
                ReadFrameCount(reader, out frameCount);
            }
            else if (propertyName == "frames")
            {
                ReadFrames(reader, out frames);
            }
        }
        return null;

        List<Frame> frames = new();

        reader.Read();

        if (reader.GetString() != "frameCount")
        {
            throw new InvalidOperationException("frame count not first");
        }
        reader.Read();


        int frameCount = reader.GetInt32();
        reader.Read();

        if (reader.GetString() != "frames")
        {
            throw new InvalidOperationException("frames not after frame count");
        }
        reader.Read();
        reader.Read();

        for (int i = 0; i < frameCount; i++)
        {
            Console.WriteLine(reader.GetString()); //  Ignore frame name
            reader.Read();
            reader.Read();
            _ = reader.GetString(); //  Ignore "frame" property name
            reader.Read();
            reader.Read();
            _ = reader.GetString(); //  Ignore "x" property name
            reader.Read();
            int frameX = reader.GetInt32();
            reader.Read();
            _ = reader.GetString(); //  Ignore "y" property name
            reader.Read();
            int frameY = reader.GetInt32();
            reader.Read();
            _ = reader.GetString(); //  Ignore "w" property name
            reader.Read();
            int frameW = reader.GetInt32();
            reader.Read();
            _ = reader.GetString(); //  Ignore "h" property name
            reader.Read();
            int frameH = reader.GetInt32();
            reader.Read();
            reader.Read();

            _ = reader.GetString();     //  Ignore "rotated" property name
            reader.Read();
            _ = reader.GetBoolean();    //  Ignore rotated property value
            reader.Read();

            _ = reader.GetString();     //  Ignore "trimmed" property name
            reader.Read();
            _ = reader.GetBoolean();    //  Ignore trimmed property value
            reader.Read();

            _ = reader.GetString();     //  Ignore "spriteSourceSize" property name
            reader.Read();
            reader.Read();
            _ = reader.GetString();     //  Ignore "spriteSourceSize" "x" property name
            reader.Read();
            _ = reader.GetInt32();      //  Ignore spriteSourceSize x value
            reader.Read();
            _ = reader.GetString();     //  Ignore "spriteSourceSize" "y" property name
            reader.Read();
            _ = reader.GetInt32();      //  Ignore spriteSourceSize y value
            reader.Read();
            _ = reader.GetString();     //  Ignore "spriteSourceSize" "w" property name
            reader.Read();
            _ = reader.GetInt32();      //  Ignore spriteSourceSize w value
            reader.Read();
            _ = reader.GetString();     //  Ignore "spriteSourceSize" "h" property name
            reader.Read();
            _ = reader.GetInt32();      //  Ignore spriteSourceSize h value
            reader.Read();
            reader.Read();

            _ = reader.GetString();     //  Ignore "sourceSize" property name
            reader.Read();
            reader.Read();
            _ = reader.GetString();     //  Ignore "sourceSize" "w" property name
            reader.Read();
            _ = reader.GetInt32();      //  Ignore sourceSize w value
            reader.Read();
            _ = reader.GetString();     //  Ignore "sourceSize" "h" property name
            reader.Read();
            _ = reader.GetInt32();      //  Ignore sourceSize h value
            reader.Read();
            reader.Read();

            _ = reader.GetString();     //  Ignore "duration" property name
            reader.Read();
            int duration = reader.GetInt32();
            reader.Read();

            Frame frame = new(new(frameX, frameY, frameW, frameH), TimeSpan.FromMilliseconds(duration));
            frames.Add(frame);
        }
        reader.Read();

        if (reader.GetString() != "meta")
        {
            throw new InvalidOperationException("meta not after frames");
        }
        reader.Read();

        _ = reader.GetString();         //  Ignore "size" property name
        reader.Read();
        reader.Read();
        _ = reader.GetString();         //  Ignore "size" "w" property name
        reader.Read();
        int width = reader.GetInt32();
        reader.Read();
        _ = reader.GetString();         //  Ignore "size" "h" property name
        reader.Read();
        int height = reader.GetInt32();
        reader.Read();
        reader.Read();
        _ = reader.GetString();         //  Ignore "scale" property name
        reader.Read();
        _ = reader.GetString();         //  Ignore scale property value
        reader.Read();
        reader.Read();

        List<uint> palette = new();

        if (reader.GetString() != "palette")
        {
            throw new InvalidOperationException("palette not after meta");
        }
        reader.Read();

        reader.Read();
        while (reader.TokenType != JsonTokenType.EndArray)
        {
            palette.Add(reader.GetUInt32());
            reader.Read();
        }
        reader.Read();

        if (reader.GetString() != "pixels")
        {
            throw new InvalidOperationException("pixels not after palette");
        }
        reader.Read();

        byte[] buffer = reader.GetBytesFromBase64();
        reader.Read();

        using (MemoryStream compressedStream = new(buffer))
        {
            //  First 2 bytes are the zlib header information, skip past them.
            _ = compressedStream.ReadByte();
            _ = compressedStream.ReadByte();

            using (MemoryStream decompressedStream = new())
            {
                using (DeflateStream deflateStream = new(compressedStream, CompressionMode.Decompress))
                {
                    deflateStream.CopyTo(decompressedStream);
                    buffer = decompressedStream.ToArray();
                }
            }
        }

        Rgba32[] pixels = new Rgba32[buffer.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Rgba32(palette[buffer[i]]);
        }

        return new Spritesheet(new(width, height), pixels, frames);
    }

    private void ReadFrameCount(Utf8JsonReader reader, out int frameCount)
    {
        _ = reader.Read();      //  Ignore property name
        frameCount = reader.GetInt32();
    }

    private void ReadFrames(Utf8JsonReader reader, out List<Frame> frames)
    {
        frames = new();

        _ = reader.Read();  //  Ignore property name
        _ = reader.Read();  //  Ignore start object token

        while (reader.TokenType != JsonTokenType.EndObject)
        {
            _ = reader.Read();  //  ignore frame name
            _ = reader.Read();  //  Ignore frame name start object token
            _ = reader.Read();  //  Ignore frame property name
            _ = reader.Read();  //  Ignore frame property start object token
            _ = reader.Read();  //  Ignore frame x property name
            int x = reader.GetInt32();
            _ = reader.Read();  //  Next token
            _ = reader.Read();  //  Ignore frame y property name
            int y = reader.GetInt32();
            _ = reader.Read();  //  Next token
            _ = reader.Read();  //  Ignore frame w property name
            int w = reader.GetInt32();
            _ = reader.Read();  //  Next token
            _ = reader.Read();  //  Ignore frame h property name
            int h = reader.GetInt32();
            _ = reader.Read();  //  Next token
            _ = reader.Read();  //  Ignore frame end object token
            _ = reader.Read();  //  Ignore rotated property name token
            _ = reader.Read();  //  Ignore rotated property value;
            _ = reader.Read();  //  Ignore trimmed property name token
            _ = reader.Read();  //  Ignore trimmed property value
            _ = reader.Read();  //  Ignore spriteSourceSize property name
            _ = reader.Read();  //  Ignore spriteSourceSize object start token
            _ = reader.Read();  //  Ignore x property name
            _ = reader.Read();  //  Ignore x value;
            _ = reader.Read();  //  Ignore y property name
            _ = reader.Read();  //  Ignore y property value
            _ = reader.Read();  //  Ignore w property name
            _ = reader.Read();  //  Ignore w property value
            _ = reader.Read();  //  Ignore h property name
            _ = reader.Read();  //  Ignore h property value
            _ = reader.Read();  //  Ignore spriteSourceSize end object token
            _ = reader.Read();  //  Ignore sourceSize name
            _ = reader.Read();  //  Ignore sourceSize object start token
            _ = reader.Read();  //  Ignore w name
            _ = reader.Read();  //  Ignore w value
            _ = reader.Read();  //  Ignore h name
            _ = reader.Read();  //  Ignore h value
            _ = reader.Read();  //  Ignore sourceSize object end token
            _ = reader.Read();  //  Ignore duration name
            int duration = reader.GetInt32();
            _ = reader.Read();  //  Next token
            _ = reader.Read();  //  Ignore end of frame object token

            Frame frame = new(new(x, y, w, h), TimeSpan.FromMilliseconds(duration));
            frames.Add(frame);
        }

    }

    public override void Write(Utf8JsonWriter writer, Spritesheet value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("frameCount", value.FrameCount);

        writer.WriteStartObject("frames");
        for (int i = 0; i < value.FrameCount; i++)
        {
            Frame frame = value.Frames[i];
            writer.WriteStartObject($"Spritesheet {i}");

            writer.WriteStartObject("frame");
            writer.WriteNumber("x", frame.X);
            writer.WriteNumber("y", frame.Y);
            writer.WriteNumber("w", frame.Width);
            writer.WriteNumber("h", frame.Height);
            writer.WriteEndObject();

            writer.WriteBoolean("rotated", false);
            writer.WriteBoolean("trimmed", false);

            writer.WriteStartObject("spriteSourceSize");
            writer.WriteNumber("x", 0);
            writer.WriteNumber("y", 0);
            writer.WriteNumber("w", frame.Width);
            writer.WriteNumber("h", frame.Height);
            writer.WriteEndObject();

            writer.WriteStartObject("sourceSize");
            writer.WriteNumber("w", frame.Width);
            writer.WriteNumber("h", frame.Height);
            writer.WriteEndObject();

            writer.WriteNumber("duration", frame.TotalMilliseconds);

            writer.WriteEndObject();
        }
        writer.WriteEndObject();

        writer.WriteStartObject("meta");
        writer.WriteStartObject("size");
        writer.WriteNumber("w", value.Width);
        writer.WriteNumber("h", value.Height);
        writer.WriteEndObject();
        writer.WriteString("scale", "1");
        writer.WriteEndObject();

        List<uint> palette = new();
        Dictionary<uint, int> map = new();
        List<byte> image = new();


        for (int i = 0; i < value.PixelCount; i++)
        {
            uint pixel = value.Pixels[i].Value;

            if (map.ContainsKey(pixel))
            {
                image.Add((byte)map[pixel]);
            }
            else
            {
                palette.Add(pixel);
                int index = palette.Count - 1;
                map.Add(pixel, index);
                image.Add((byte)map[pixel]);
            }
        }

        writer.WriteStartArray("palette");
        for (int i = 0; i < palette.Count; i++)
        {
            writer.WriteNumberValue(palette[i]);
        }
        writer.WriteEndArray();

        using MemoryStream ms = new();
        ms.WriteByte(0x78);
        ms.WriteByte(0x9C);

        using (DeflateStream deflate = new DeflateStream(ms, CompressionMode.Compress, leaveOpen: true))
        {
            byte[] buffer = image.ToArray();
            deflate.Write(buffer, 0, buffer.Length);
        }

        byte[] pixels = ms.ToArray();


        writer.WriteBase64String("pixels", pixels);

        writer.WriteEndObject();

    }
}