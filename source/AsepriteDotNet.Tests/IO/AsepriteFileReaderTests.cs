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
using AsepriteDotNet.Common;
using AsepriteDotNet.AsepriteTypes;
using AsepriteDotNet.IO;

namespace AsepriteDotNet.Tests;

public sealed class AsepriteFileReaderTest
{
    private string GetPath(string name)
    {
        return Path.Combine(Environment.CurrentDirectory, "Files", name);
    }

    [Fact]
    public void AsepriteFileReader_ReadFileTest()
    {
        string path = GetPath("read-test.aseprite");
        AsepriteFile doc = AsepriteFileReader.ReadFile(path);

        //  Expected pallette colors
        Rgba32 pal0 = Rgba32.FromRGBA(223, 7, 114, 255);
        Rgba32 pal1 = Rgba32.FromRGBA(254, 84, 111, 255);
        Rgba32 pal2 = Rgba32.FromRGBA(255, 158, 125, 255);
        Rgba32 pal3 = Rgba32.FromRGBA(255, 208, 128, 255);
        Rgba32 pal4 = Rgba32.FromRGBA(255, 253, 255, 255);
        Rgba32 pal5 = Rgba32.FromRGBA(11, 255, 230, 255);
        Rgba32 pal6 = Rgba32.FromRGBA(1, 203, 207, 255);
        Rgba32 pal7 = Rgba32.FromRGBA(1, 136, 165, 255);
        Rgba32 pal8 = Rgba32.FromRGBA(62, 50, 100, 255);
        Rgba32 pal9 = Rgba32.FromRGBA(53, 42, 85, 255);

        //  Validate palette
        Assert.True(doc.Palette.Count == 10);
        Assert.Equal(pal0, doc.Palette[0]);
        Assert.Equal(pal1, doc.Palette[1]);
        Assert.Equal(pal2, doc.Palette[2]);
        Assert.Equal(pal3, doc.Palette[3]);
        Assert.Equal(pal4, doc.Palette[4]);
        Assert.Equal(pal5, doc.Palette[5]);
        Assert.Equal(pal6, doc.Palette[6]);
        Assert.Equal(pal7, doc.Palette[7]);
        Assert.Equal(pal8, doc.Palette[8]);
        Assert.Equal(pal9, doc.Palette[9]);

        //  Validate Layers
        Assert.Equal(11, doc.Layers.Count);
        Assert.IsType<AsepriteImageLayer>(doc.Layers[0]);
        Assert.True(doc.Layers[0].IsBackgroundLayer);
        Assert.Equal("background", doc.Layers[0].Name);
        Assert.True(doc.Layers[0].IsVisible);
        Assert.Equal("hidden", doc.Layers[1].Name);
        Assert.False(doc.Layers[1].IsVisible);
        Assert.Equal("user-data", doc.Layers[2].Name);
        Assert.Equal("user-data text", doc.Layers[2].UserData.Text);
        Assert.Equal(Rgba32.FromRGBA(223, 7, 114, 255), doc.Layers[2].UserData.Color);
        Assert.Equal("reference", doc.Layers[3].Name);
        Assert.True(doc.Layers[3].IsReferenceLayer);
        Assert.Equal("75-opacity", doc.Layers[4].Name);
        Assert.Equal(75, doc.Layers[4].Opacity);
        Assert.Equal("blendmode-difference", doc.Layers[5].Name);
        Assert.Equal(BlendMode.Difference, doc.Layers[5].BlendMode);
        Assert.Equal("tilemap", doc.Layers[6].Name);
        Assert.Equal(0, Assert.IsType<AsepriteTilemapLayer>(doc.Layers[6]).Tileset.ID);
        Assert.Equal(2, Assert.IsType<AsepriteGroupLayer>(doc.Layers[7]).Children.Count);
        Assert.Equal(1, doc.Layers[8].ChildLevel);
        Assert.Equal(1, doc.Layers[9].ChildLevel);

        //  Validate Tags
        Assert.Equal(3, doc.Tags.Count);
        Assert.Equal("tag0to2forward", doc.Tags[0].Name);
        Assert.Equal(0, doc.Tags[0].From);
        Assert.Equal(2, doc.Tags[0].To);
        Assert.Equal(Rgba32.FromRGBA(0, 0, 0, 255), doc.Tags[0].Color);
        Assert.Equal(LoopDirection.Forward, doc.Tags[0].LoopDirection);
        Assert.Equal("tag3pingpong", doc.Tags[1].Name);
        Assert.Equal(LoopDirection.PingPong, doc.Tags[1].LoopDirection);
        Assert.Equal("tag4userdata", doc.Tags[2].Name);
        Assert.Equal(Rgba32.FromRGBA(11, 255, 230, 255), doc.Tags[2].Color);
        Assert.Equal(Rgba32.FromRGBA(11, 255, 230, 255), doc.Tags[2].UserData.Color);
        Assert.Equal("tag-4-user-data", doc.Tags[2].UserData.Text);

        //  Validate Frames
        Assert.Equal(7, doc.Frames.Count);
        Assert.Equal(100, doc.Frames[0].Duration);
        Assert.Equal(200, doc.Frames[1].Duration);
        Assert.Equal(123, doc.Frames[2].Duration);
        Assert.Equal(2, doc.Frames[0].Cels.Count);  //  Background and Reference Layer cels

        //  Validate Cels
        AsepriteImageCel fgCel = Assert.IsType<AsepriteImageCel>(doc.Frames[2].Cels[1]);
        Assert.Equal("foreground", fgCel.Layer.Name);
        Assert.Equal(new Dimension(16, 16), fgCel.Size);
        Assert.Equal(new Location(8, 8), fgCel.Position);
        Assert.Equal(fgCel.Size.Width * fgCel.Size.Height, fgCel.Pixels.Length);
    }

    [Fact]
    public void AsepriteFileReader_ColorModeRGBA_PixelsTest()
    {
        string path = GetPath("rgba-pixel-test.aseprite");
        AsepriteFile doc = AsepriteFileReader.ReadFile(path);

        Assert.Equal(10, doc.Palette.Count);

        Rgba32 tran = Rgba32.Transparent;
        Rgba32 pal0 = doc.Palette[0];
        Rgba32 pal1 = doc.Palette[1];
        Rgba32 pal2 = doc.Palette[2];
        Rgba32 pal3 = doc.Palette[3];
        Rgba32 pal4 = doc.Palette[4];
        Rgba32 pal5 = doc.Palette[5];
        Rgba32 pal6 = doc.Palette[6];
        Rgba32 pal7 = doc.Palette[7];
        Rgba32 pal8 = doc.Palette[8];
        Rgba32 pal9 = doc.Palette[9];

        Rgba32[] expected = new Rgba32[]
        {
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5,
            pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1,
            pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3,
            pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9,
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5,
            pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1,
            pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3,
            pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9,
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5,
            pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1,
            pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3,
            pal4, pal5, pal6, pal7, pal8, pal9, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9,
            pal0, pal9, pal1, pal8, pal2, pal7, pal3, pal6, tran, tran, tran, tran, tran, tran, tran, tran
        };

        AsepriteImageCel cel = Assert.IsType<AsepriteImageCel>(doc.Frames[0].Cels[0]);
        Assert.Equal(expected, cel.Pixels);
    }

    [Fact]
    public void AsepriteFileReader_ColorModeIndexed_PixelsTest()
    {
        string path = GetPath("indexed-pixel-test.aseprite");
        AsepriteFile doc = AsepriteFileReader.ReadFile(path);

        Assert.Equal(11, doc.Palette.Count);

        Rgba32 pal0 = Rgba32.Transparent;
        Rgba32 pal1 = doc.Palette[1];
        Rgba32 pal2 = doc.Palette[2];
        Rgba32 pal3 = doc.Palette[3];
        Rgba32 pal4 = doc.Palette[4];
        Rgba32 pal5 = doc.Palette[5];
        Rgba32 pal6 = doc.Palette[6];
        Rgba32 pal7 = doc.Palette[7];
        Rgba32 pal8 = doc.Palette[8];
        Rgba32 pal9 = doc.Palette[9];
        Rgba32 pal10 = doc.Palette[10];

        Rgba32[] expected = new Rgba32[]
        {
            pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6,
            pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2,
            pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8,
            pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4,
            pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10,
            pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6,
            pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2,
            pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8,
            pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4,
            pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10,
            pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6,
            pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2,
            pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8,
            pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4,
            pal5, pal6, pal7, pal8, pal9, pal10, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal8, pal9, pal10,
            pal1, pal10, pal2, pal9, pal3, pal8, pal4, pal7, pal0, pal0, pal0, pal0, pal0, pal0, pal0, pal0
        };

        AsepriteImageCel cel = Assert.IsType<AsepriteImageCel>(doc.Frames[0].Cels[0]);
        Assert.Equal(expected, cel.Pixels);
    }

    [Fact]
    public void AsepriteFileReader_GrayscaleModeRGBA_PixelsTest()
    {
        string path = GetPath("grayscale-pixel-test.aseprite");
        AsepriteFile doc = AsepriteFileReader.ReadFile(path);

        Assert.Equal(8, doc.Palette.Count);

        Rgba32 tran = Rgba32.Transparent;
        Rgba32 pal0 = doc.Palette[0];
        Rgba32 pal1 = doc.Palette[1];
        Rgba32 pal2 = doc.Palette[2];
        Rgba32 pal3 = doc.Palette[3];
        Rgba32 pal4 = doc.Palette[4];
        Rgba32 pal5 = doc.Palette[5];
        Rgba32 pal6 = doc.Palette[6];
        Rgba32 pal7 = doc.Palette[7];

        Rgba32[] expected = new Rgba32[]
        {
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0, pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0,
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0, pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0,
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0, pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0,
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0, pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0,
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0, pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0,
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0, pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0,
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0, pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0,
            pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7, pal0, pal1, pal2, pal3, pal4, pal5, pal6, pal7,
            pal7, pal6, pal5, pal4, pal3, pal2, pal1, pal0, tran, tran, tran, tran, tran, tran, tran, tran
        };

        AsepriteImageCel cel = Assert.IsType<AsepriteImageCel>(doc.Frames[0].Cels[0]);
        Assert.Equal(expected, cel.Pixels);
    }

    [Fact]
    public void AsepriteFileReader_TilemapTest()
    {
        string path = GetPath("tilemap-test.aseprite");
        AsepriteFile doc = AsepriteFileReader.ReadFile(path);

        Rgba32 tran = Rgba32.Transparent;
        Rgba32 pal0 = doc.Palette[0];
        Rgba32 pal1 = doc.Palette[1];
        Rgba32 pal2 = doc.Palette[2];
        Rgba32 pal3 = doc.Palette[3];
        Rgba32 pal4 = doc.Palette[4];
        Rgba32 pal5 = doc.Palette[5];
        Rgba32 pal6 = doc.Palette[6];
        Rgba32 pal7 = doc.Palette[7];
        Rgba32 pal8 = doc.Palette[8];
        Rgba32 pal9 = doc.Palette[9];

        Assert.Single(doc.Tilesets);
        AsepriteTileset tileset = doc.Tilesets[0];
        Assert.Equal("test-tileset", tileset.Name);

        Assert.Equal(0, tileset.ID);
        Assert.Equal(11, tileset.TileCount);
        Assert.Equal(new Dimension(8, 8), tileset.TileSize);

        Rgba32[] expectedTilesetPixels = new Rgba32[]
        {
            tran, tran, tran, tran, tran, tran, tran, tran,
            tran, tran, tran, tran, tran, tran, tran, tran,
            tran, tran, tran, tran, tran, tran, tran, tran,
            tran, tran, tran, tran, tran, tran, tran, tran,
            tran, tran, tran, tran, tran, tran, tran, tran,
            tran, tran, tran, tran, tran, tran, tran, tran,
            tran, tran, tran, tran, tran, tran, tran, tran,
            tran, tran, tran, tran, tran, tran, tran, tran,

            pal0, pal0, pal0, pal0, pal0, pal0, pal0, pal0,
            pal0, tran, tran, tran, tran, tran, tran, pal0,
            pal0, tran, tran, tran, tran, tran, tran, pal0,
            pal0, tran, tran, tran, tran, tran, tran, pal0,
            pal0, tran, tran, tran, tran, tran, tran, pal0,
            pal0, tran, tran, tran, tran, tran, tran, pal0,
            pal0, tran, tran, tran, tran, tran, tran, pal0,
            pal0, pal0, pal0, pal0, pal0, pal0, pal0, pal0,

            pal1, pal1, pal1, pal1, pal1, pal1, pal1, pal1,
            pal1, tran, tran, tran, tran, tran, tran, pal1,
            pal1, tran, tran, tran, tran, tran, tran, pal1,
            pal1, tran, tran, tran, tran, tran, tran, pal1,
            pal1, tran, tran, tran, tran, tran, tran, pal1,
            pal1, tran, tran, tran, tran, tran, tran, pal1,
            pal1, tran, tran, tran, tran, tran, tran, pal1,
            pal1, pal1, pal1, pal1, pal1, pal1, pal1, pal1,

            pal2, pal2, pal2, pal2, pal2, pal2, pal2, pal2,
            pal2, tran, tran, tran, tran, tran, tran, pal2,
            pal2, tran, tran, tran, tran, tran, tran, pal2,
            pal2, tran, tran, tran, tran, tran, tran, pal2,
            pal2, tran, tran, tran, tran, tran, tran, pal2,
            pal2, tran, tran, tran, tran, tran, tran, pal2,
            pal2, tran, tran, tran, tran, tran, tran, pal2,
            pal2, pal2, pal2, pal2, pal2, pal2, pal2, pal2,

            pal3, pal3, pal3, pal3, pal3, pal3, pal3, pal3,
            pal3, tran, tran, tran, tran, tran, tran, pal3,
            pal3, tran, tran, tran, tran, tran, tran, pal3,
            pal3, tran, tran, tran, tran, tran, tran, pal3,
            pal3, tran, tran, tran, tran, tran, tran, pal3,
            pal3, tran, tran, tran, tran, tran, tran, pal3,
            pal3, tran, tran, tran, tran, tran, tran, pal3,
            pal3, pal3, pal3, pal3, pal3, pal3, pal3, pal3,

            pal4, pal4, pal4, pal4, pal4, pal4, pal4, pal4,
            pal4, tran, tran, tran, tran, tran, tran, pal4,
            pal4, tran, tran, tran, tran, tran, tran, pal4,
            pal4, tran, tran, tran, tran, tran, tran, pal4,
            pal4, tran, tran, tran, tran, tran, tran, pal4,
            pal4, tran, tran, tran, tran, tran, tran, pal4,
            pal4, tran, tran, tran, tran, tran, tran, pal4,
            pal4, pal4, pal4, pal4, pal4, pal4, pal4, pal4,

            pal5, pal5, pal5, pal5, pal5, pal5, pal5, pal5,
            pal5, tran, tran, tran, tran, tran, tran, pal5,
            pal5, tran, tran, tran, tran, tran, tran, pal5,
            pal5, tran, tran, tran, tran, tran, tran, pal5,
            pal5, tran, tran, tran, tran, tran, tran, pal5,
            pal5, tran, tran, tran, tran, tran, tran, pal5,
            pal5, tran, tran, tran, tran, tran, tran, pal5,
            pal5, pal5, pal5, pal5, pal5, pal5, pal5, pal5,

            pal6, pal6, pal6, pal6, pal6, pal6, pal6, pal6,
            pal6, tran, tran, tran, tran, tran, tran, pal6,
            pal6, tran, tran, tran, tran, tran, tran, pal6,
            pal6, tran, tran, tran, tran, tran, tran, pal6,
            pal6, tran, tran, tran, tran, tran, tran, pal6,
            pal6, tran, tran, tran, tran, tran, tran, pal6,
            pal6, tran, tran, tran, tran, tran, tran, pal6,
            pal6, pal6, pal6, pal6, pal6, pal6, pal6, pal6,

            pal7, pal7, pal7, pal7, pal7, pal7, pal7, pal7,
            pal7, tran, tran, tran, tran, tran, tran, pal7,
            pal7, tran, tran, tran, tran, tran, tran, pal7,
            pal7, tran, tran, tran, tran, tran, tran, pal7,
            pal7, tran, tran, tran, tran, tran, tran, pal7,
            pal7, tran, tran, tran, tran, tran, tran, pal7,
            pal7, tran, tran, tran, tran, tran, tran, pal7,
            pal7, pal7, pal7, pal7, pal7, pal7, pal7, pal7,

            pal8, pal8, pal8, pal8, pal8, pal8, pal8, pal8,
            pal8, tran, tran, tran, tran, tran, tran, pal8,
            pal8, tran, tran, tran, tran, tran, tran, pal8,
            pal8, tran, tran, tran, tran, tran, tran, pal8,
            pal8, tran, tran, tran, tran, tran, tran, pal8,
            pal8, tran, tran, tran, tran, tran, tran, pal8,
            pal8, tran, tran, tran, tran, tran, tran, pal8,
            pal8, pal8, pal8, pal8, pal8, pal8, pal8, pal8,

            pal9, pal9, pal9, pal9, pal9, pal9, pal9, pal9,
            pal9, tran, tran, tran, tran, tran, tran, pal9,
            pal9, tran, tran, tran, tran, tran, tran, pal9,
            pal9, tran, tran, tran, tran, tran, tran, pal9,
            pal9, tran, tran, tran, tran, tran, tran, pal9,
            pal9, tran, tran, tran, tran, tran, tran, pal9,
            pal9, tran, tran, tran, tran, tran, tran, pal9,
            pal9, pal9, pal9, pal9, pal9, pal9, pal9, pal9,
        };

        Assert.Equal(expectedTilesetPixels, tileset.Pixels);


        AsepriteTilemapLayer tilesLayer = Assert.IsType<AsepriteTilemapLayer>(doc.Layers[1]);
        Assert.Equal(tileset, tilesLayer.Tileset);
        // Assert.Equal(tileset.ID, tilesLayer.TilesetIndex);

        AsepriteTilemapCel tilesCel = Assert.IsType<AsepriteTilemapCel>(doc.Frames[0].Cels[1]);

        Assert.Equal(32, tilesCel.BitsPerTile);
        Assert.Equal((uint)0x1fffffff, tilesCel.TileIdBitmask);
        Assert.Equal((uint)0x20000000, tilesCel.XFlipBitmask);
        Assert.Equal((uint)0x40000000, tilesCel.YFlipBitmask);
        Assert.Equal(0x80000000, tilesCel.RotationBitmask);
        Assert.Equal(16, tilesCel.Tiles.Length);

        uint[] ids = new uint[]
        {
            1, 2, 3, 1,
            3, 4, 5, 2,
            2, 6, 7, 3,
            1, 3, 2, 1
        };

        Assert.Equal(ids, tilesCel.Tiles.Select(tile => tile.TileID).ToArray());

        //  Can't test Tile.XFlip, Tile.YFlip, and Tile.Rotate90 because these
        //  aren't actually implemented in Aseprite yet.
    }
}