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
using System.Reflection;

using AsepriteDotNet.MonoGame.Pipeline.Processors;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace AsepriteDotNet.MonoGame.Pipeline.Writers;

[ContentTypeWriter]
public sealed class AsepriteFileWriter : ContentTypeWriter<AsepriteFileProcessorResult>
{

    public byte[] Write(AsepriteFileProcessorResult input)
    {
        using MemoryStream ms = new();
        using BinaryWriter writer = new(ms);
        InternalWrite(writer, input);
        return ms.ToArray();
    }

    protected override void Write(ContentWriter output, AsepriteFileProcessorResult input) => InternalWrite(output, input);

    private void InternalWrite(BinaryWriter writer, AsepriteFileProcessorResult input)
    {
        WriteSpritesheetChunk(writer, input.SpriteSheetWidth, input.SpriteSheetHeight, input.SpriteSheetPixels);
    }

    //  ------------------------------------------------------------------------
    //  Spritesheet Chunk
    //  
    //  [uint] Width of spritesheet, in number of pixels.
    //  [uint] Height of spritesheet, in number of pixels.
    //  +   For each pixel
    //      [uint]  Color of the pixel as a packed MonoGame color value
    //
    //  When reading the chunk, to know how many pixels to read just multiply
    //  width and height
    //  ------------------------------------------------------------------------
    private void WriteSpritesheetChunk(BinaryWriter writer, uint width, uint height, uint[] data)
    {
        writer.Write(width);
        writer.Write(height);
        for (int i = 0; i < data.Length; i++)
        {
            writer.Write(data[i]);
        }
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return "AsepriteDotNet.MonoGame.ContentReaders.AtlasTypeReader, AsepriteDotNet.MonoGame";
    }
}