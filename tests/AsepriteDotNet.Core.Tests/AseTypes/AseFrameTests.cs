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

public class AseFrameTests
{

    [Fact]
    public void AseFrame_ConstructorTest()
    {
        Size size = new Size(1, 2);
        int duration = 3;
        List<AseCel> cels = new();

        AseFrame frame = new(size, duration, cels);

        Assert.Equal(size, frame.Size);
        Assert.Equal(size.Width, frame.Width);
        Assert.Equal(size.Height, frame.Height);
        Assert.Equal(cels, frame.Cels);
        Assert.Equal(cels.Count, frame.CelCount);
    }

    [Fact]
    public void AseFrame_IndexerPropertyTest()
    {
        AseLayer layer = new(true, false, false, 0, BlendMode.Normal, 255, "Layer");
        AseCel cel = new AseImageCel(Size.Empty, Array.Empty<Rgba32>(), layer, Point.Empty, 255);

        List<AseCel> cels = new() { cel };
        AseFrame frame = new(Size.Empty, 0, cels);

        Assert.Equal(frame[0], cel);
    }

    [Fact]
    public void AseFrame_FlattenFrameTest()
    {
        AseLayer bottomLayer = new(true, false, false, 0, BlendMode.Normal, 255, "bottom");
        AseLayer middleNotVisibleLayer = new(false, false, false, 0, BlendMode.Normal, 255, "middleNotVisible");
        AseLayer topLayer = new(true, false, false, 0, BlendMode.Normal, 255, "top");

        Size celSize = new Size(2, 2);
        Point celPosition = new Point(0, 0);

        Rgba32[] bottomCelPixels = new Rgba32[]
        {
            Rgba32.FromRGBA(255, 0, 0, 255), Rgba32.Transparent,
            Rgba32.Transparent, Rgba32.Transparent
        };

        Rgba32[] middleCelPixels = new Rgba32[]
        {
            Rgba32.Transparent, Rgba32.FromRGBA(0, 255, 0, 255),
            Rgba32.Transparent, Rgba32.Transparent
        };

        Rgba32[] topCelPixels = new Rgba32[]
        {
            Rgba32.Transparent, Rgba32.Transparent,
            Rgba32.FromRGBA(0, 0, 255, 255), Rgba32.Transparent
        };


        AseCel bottomCel = new AseImageCel(celSize, bottomCelPixels, bottomLayer, celPosition, 255);
        AseCel middleCel = new AseImageCel(celSize, middleCelPixels, middleNotVisibleLayer, celPosition, 255);
        AseCel topCel = new AseImageCel(celSize, topCelPixels, topLayer, celPosition, 255);

        List<AseCel> cels = new() { topCel, middleCel, bottomCel };
        AseFrame frame = new(celSize, 0, cels);

        Rgba32[] allExpected = new Rgba32[]
        {
            Rgba32.FromRGBA(255, 0, 0, 255), Rgba32.FromRGBA(0, 255, 0, 255),
            Rgba32.FromRGBA(0, 0, 255, 255), Rgba32.Transparent
        };

        Rgba32[] allFlattened = frame.FlattenFrame(onlyVisibleLayers: false);

        Assert.Equal(allExpected, allFlattened);

        Rgba32[] onlyVisibleExpected = new Rgba32[]
        {
            Rgba32.FromRGBA(255, 0, 0, 255), Rgba32.Transparent,
            Rgba32.FromRGBA(0, 0, 255, 255), Rgba32.Transparent
        };

        Rgba32[] onlyVisibleFlattened = frame.FlattenFrame(onlyVisibleLayers: true);

        Assert.Equal(onlyVisibleExpected, onlyVisibleFlattened);
    }
}