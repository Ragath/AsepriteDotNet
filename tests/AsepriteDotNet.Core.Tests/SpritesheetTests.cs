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
using AsepriteDotNet.Core.Color;
using AsepriteDotNet.Core.Primitives;

namespace AsepriteDotNet.Core.Tests;

public sealed class SpritesheetTests
{
    [Fact]
    public void Spritesheet_ConstructorTest()
    {
        Size size = new(10, 20);
        Rgba32[] pixels = new Rgba32[]
        {
            Rgba32.FromRGBA(255, 0, 0, 255),
            Rgba32.FromRGBA(0, 255, 0, 255),
            Rgba32.FromRGBA(0, 0, 255, 255)
        };

        List<Frame> frames = new()
        {
            new(Rectangle.Empty, TimeSpan.Zero)
        };

        List<Tag> tags = new()
        {
            new("Tag1", 0, 1, LoopDirection.Forward, Rgba32.FromRGBA(255, 0, 0, 255)),
            new("Tag2", 2, 3, LoopDirection.Reverse, Rgba32.FromRGBA(0, 255, 0, 255))
        };

        List<Slice> slices = new()
        {
            new("Slice1", 1, Rectangle.Empty, Rgba32.FromRGBA(255, 0, 0, 255)),
            new("Slice2", 2, Rectangle.Empty, Rgba32.FromRGBA(0, 255, 0, 255)),
            new("Slice3", 3, Rectangle.Empty, Rgba32.FromRGBA(0, 0, 255, 255)),
        };

        Spritesheet spritesheet = new(size, pixels, frames, tags, slices);

        Assert.Equal(size, spritesheet.Size);
        Assert.Equal(size.Width, spritesheet.Width);
        Assert.Equal(size.Height, spritesheet.Height);
        Assert.Equal(pixels, spritesheet.Pixels);
        Assert.Equal(pixels.Length, spritesheet.PixelCount);
        Assert.Equal(frames, spritesheet.Frames);
        Assert.Equal(frames.Count, spritesheet.FrameCount);
        Assert.Equal(tags, spritesheet.Tags);
        Assert.Equal(tags.Count, spritesheet.TagCount);
        Assert.Equal(slices, spritesheet.Slices);
        Assert.Equal(slices.Count, spritesheet.SliceCount);
    }

    [Fact]
    public void Spritesheet_IndexerTest()
    {
        List<Frame> frames = new()
        {
            new(Rectangle.Empty, TimeSpan.Zero)
        };

        Spritesheet spritesheet = new(Size.Empty, Array.Empty<Rgba32>(), frames, new List<Tag>(), new List<Slice>());

        Assert.Equal(frames[0], spritesheet[0]);
    }
}