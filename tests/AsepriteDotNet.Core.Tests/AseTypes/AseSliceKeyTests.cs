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

public class AseSliceKeyTests
{

    [Fact]
    public void AseSliceKey_ConstructorTest()
    {
        int frame = 1;
        Rectangle bounds = new(2, 3, 4, 5);
        AseSliceKey key = new(frame, bounds);

        Assert.False(key.IsNinePatch);
        Assert.False(key.HasPivot);
        Assert.Equal(frame, key.Frame);
        Assert.Equal(bounds, key.Bounds);
        Assert.Equal(bounds.Size, key.Size);
        Assert.Equal(bounds.Width, key.Width);
        Assert.Equal(bounds.Height, key.Height);
        Assert.Equal(bounds.Location, key.Location);
        Assert.Equal(bounds.X, key.X);
        Assert.Equal(bounds.Y, key.Y);
        Assert.Equal(bounds.Top, key.Top);
        Assert.Equal(bounds.Bottom, key.Bottom);
        Assert.Equal(bounds.Left, key.Left);
        Assert.Equal(bounds.Right, key.Right);

        Rectangle center = new Rectangle(6, 7, 8, 9);
        key = new(frame, bounds, center);

        Assert.True(key.IsNinePatch);
        Assert.Equal(center, key.CenterBounds);
        Assert.Equal(center.Size, key.CenterSize);
        Assert.Equal(center.Width, key.CenterWidth);
        Assert.Equal(center.Height, key.CenterHeight);
        Assert.Equal(center.Location, key.CenterLocation);
        Assert.Equal(center.X, key.CenterX);
        Assert.Equal(center.Y, key.CenterY);
        Assert.Equal(center.Top, key.CenterTop);
        Assert.Equal(center.Bottom, key.CenterBottom);
        Assert.Equal(center.Left, key.CenterLeft);
        Assert.Equal(center.Right, key.CenterRight);

        Point pivot = new(10, 11);
        key = new(frame, bounds, null, pivot);

        Assert.True(key.HasPivot);
        Assert.Equal(pivot, key.Pivot);
        Assert.Equal(pivot.X, key.PivotX);
        Assert.Equal(pivot.Y, key.PivotY);
    }
}