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

public class SliceTests
{

    [Fact]
    public void Slice_ConstructorTest()
    {
        string name = "Slice";
        int frameIndex = 1;
        Rectangle bounds = new(10, 20, 30, 40);
        Rgba32 color = Rgba32.FromRGBA(255, 0, 0, 255);
        Rectangle? center = default;
        Point? pivot = default;

        Slice slice = new(name, frameIndex, bounds, color, center, pivot);

        Assert.Equal(name, slice.Name);
        Assert.Equal(frameIndex, slice.FrameIndex);
        Assert.Equal(bounds, slice.Bounds);
        Assert.Equal(bounds.Size, slice.Size);
        Assert.Equal(bounds.Width, slice.Width);
        Assert.Equal(bounds.Height, slice.Height);
        Assert.Equal(bounds.Location, slice.Location);
        Assert.Equal(bounds.X, slice.X);
        Assert.Equal(bounds.Y, slice.Y);
        Assert.Equal(bounds.Top, slice.Top);
        Assert.Equal(bounds.Bottom, slice.Bottom);
        Assert.Equal(bounds.Left, slice.Left);
        Assert.Equal(bounds.Right, slice.Right);

        Assert.False(slice.IsNinePatch);
        Assert.False(slice.HasPivot);

        Assert.Null(slice.CenterBounds);
        Assert.Null(slice.CenterSize);
        Assert.Null(slice.CenterWidth);
        Assert.Null(slice.CenterHeight);
        Assert.Null(slice.CenterLocation);
        Assert.Null(slice.CenterX);
        Assert.Null(slice.CenterY);
        Assert.Null(slice.CenterTop);
        Assert.Null(slice.CenterBottom);
        Assert.Null(slice.CenterLeft);
        Assert.Null(slice.CenterRight);

        Assert.Null(slice.Pivot);
        Assert.Null(slice.PivotX);
        Assert.Null(slice.PivotY);

        center = new(50, 60, 70, 80);
        slice = new(name, frameIndex, bounds, color, center, pivot);

        Assert.True(slice.IsNinePatch);

        Assert.NotNull(slice.CenterBounds);
        Assert.NotNull(slice.CenterSize);
        Assert.NotNull(slice.CenterWidth);
        Assert.NotNull(slice.CenterHeight);
        Assert.NotNull(slice.CenterLocation);
        Assert.NotNull(slice.CenterX);
        Assert.NotNull(slice.CenterY);
        Assert.NotNull(slice.CenterTop);
        Assert.NotNull(slice.CenterBottom);
        Assert.NotNull(slice.CenterLeft);
        Assert.NotNull(slice.CenterRight);

        Assert.Equal(center, slice.CenterBounds);
        Assert.Equal(center?.Size, slice.CenterSize);
        Assert.Equal(center?.Width, slice.CenterWidth);
        Assert.Equal(center?.Height, slice.CenterHeight);
        Assert.Equal(center?.Location, slice.CenterLocation);
        Assert.Equal(center?.X, slice.CenterX);
        Assert.Equal(center?.Y, slice.CenterY);
        Assert.Equal(center?.Top, slice.CenterTop);
        Assert.Equal(center?.Bottom, slice.CenterBottom);
        Assert.Equal(center?.Left, slice.CenterLeft);
        Assert.Equal(center?.Right, slice.CenterRight);

        pivot = new(90, 100);
        slice = new(name, frameIndex, bounds, color, center, pivot);

        Assert.True(slice.HasPivot);

        Assert.NotNull(slice.Pivot);
        Assert.NotNull(slice.PivotX);
        Assert.NotNull(slice.PivotY);

        Assert.Equal(pivot, slice.Pivot);
        Assert.Equal(pivot?.X, slice.PivotX);
        Assert.Equal(pivot?.Y, slice.PivotY);
    }
}