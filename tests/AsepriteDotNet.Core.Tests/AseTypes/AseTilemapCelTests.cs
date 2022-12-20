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
using AsepriteDotNet.Core.AseTypes;
using AsepriteDotNet.Core.Color;
using AsepriteDotNet.Core.Primitives;

namespace AsepriteDotNet.Core.Tests;

public class AseTilemapCelTests
{

    [Fact]
    public void AseTilemapCel_ConstructorTest()
    {
        Size size = new(1, 2);
        Size tileSize = new(3, 4);
        List<AseTile> tiles = new();
        AseLayer layer = new(true, true, true, 0, BlendMode.Normal, 255, "Layer");
        Point position = new(3, 4);
        int opacity = 5;
        AseTilemapCel cel = new(size, tileSize, tiles, layer, position, opacity);

        Assert.Equal(size, cel.Size);
        Assert.Equal(size.Width, cel.Size.Width);
        Assert.Equal(size.Height, cel.Size.Height);
        Assert.Equal(tileSize, cel.TileSize);
        Assert.Equal(tileSize.Width, cel.TileWidth);
        Assert.Equal(tileSize.Height, cel.TileHeight);
        Assert.Equal(tiles.Count, cel.TileCount);
        Assert.Empty(cel.Tiles);
        Assert.Equal(layer, cel.Layer);
        Assert.Equal(position, cel.Position);
        Assert.Equal(position.X, cel.X);
        Assert.Equal(position.Y, cel.Y);
        Assert.Equal(opacity, cel.Opacity);
        Assert.Null(cel.UserData);

        string text = "Hello World";
        Rgba32 color = Rgba32.FromRGBA(1, 2, 3, 4);
        AseUserData userdata = new(text, color);

        cel = new(size, tileSize, tiles, layer, position, opacity, userdata);
        Assert.NotNull(cel.UserData);
        Assert.Equal(color, cel.UserData.Color);
        Assert.Equal(text, cel.UserData.Text);
    }

    [Fact]
    public void AseTilemapCel_IndexerTest()
    {
        AseTile tile = new(1, 0, 0, 0);
        List<AseTile> tiles = new() { tile };
        AseLayer layer = new(true, true, true, 0, BlendMode.Normal, 255, "Layer");

        AseTilemapCel cel = new(Size.Empty, Size.Empty, tiles, layer, Point.Empty, 255);

        Assert.Equal(tiles[0], cel[0]);
    }

}