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
using AsepriteDotNet.Core.Primitives;

namespace AsepriteDotNet.Tests;

public sealed class RectangleTests
{
    [Fact]
    public void Rectangle_EmptyTest()
    {
        Rectangle empty = new Rectangle(0, 0, 0, 0);
        Assert.Equal(empty, Rectangle.Empty);
    }

    [Fact]
    public void Rectangle_DefaultConstructorTest()
    {
        Assert.Equal(Rectangle.Empty, new Rectangle());
    }

    [Theory]
    [InlineData(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue)]
    [InlineData(int.MaxValue, 10, int.MinValue, 10)]
    [InlineData(10, int.MaxValue, 10, int.MinValue)]
    [InlineData(0, 0, 0, 0)]
    public void Rectangle_NonDefaultConstructorTests(int x, int y, int w, int h)
    {
        Rectangle rect = new(x, y, w, h);
        Assert.Equal(x, rect.X);
        Assert.Equal(y, rect.Y);
        Assert.Equal(w, rect.Width);
        Assert.Equal(h, rect.Height);
    }

    [Theory]
    [InlineData(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue)]
    [InlineData(int.MaxValue, 10, int.MinValue, 10)]
    [InlineData(10, int.MaxValue, 10, int.MinValue)]
    [InlineData(0, 0, 0, 0)]
    public void Rectangle_DimensionsTest(int x, int y, int w, int h)
    {
        Rectangle rect = new(x, y, w, h);
        Assert.Equal(new Point(x, y), rect.Location);
        Assert.Equal(new Size(w, h), rect.Size);
        Assert.Equal(y, rect.Top);
        Assert.Equal(unchecked(y + h), rect.Bottom);
        Assert.Equal(x, rect.Left);
        Assert.Equal(unchecked(x + w), rect.Right);
    }

    [Fact]
    public void Rectangle_IsEmpty_DefaultTests()
    {
        Assert.True(Rectangle.Empty.IsEmpty);
        Assert.True(default(Rectangle).IsEmpty);
        Assert.True(new Rectangle().IsEmpty);
        Assert.True(new Rectangle(0, 0, 0, 0).IsEmpty);
    }

    [Theory]
    [InlineData(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue, int.MaxValue, 0)]
    [InlineData(int.MinValue, int.MinValue, 0, int.MaxValue)]
    [InlineData(int.MinValue, 0, int.MaxValue, int.MaxValue)]
    [InlineData(0, int.MinValue, int.MaxValue, int.MaxValue)]
    [InlineData(10, 20, 30, 40)]
    public void Rectangle_IsEmpty_NotEmptyTests(int x, int y, int w, int h)
    {
        Assert.False(new Rectangle(x, y, w, h).IsEmpty);
    }

    [Fact]
    public void Rect_EqualTest()
    {
        Rectangle a = new Rectangle(1, 2, 3, 4);
        Rectangle b = new Rectangle(1, 2, 3, 4);

        Assert.True(a == b);
        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.False(a == Rectangle.Empty);
        Assert.False(a.Equals(Rectangle.Empty));
        Assert.False(a.Equals((object)Rectangle.Empty));
    }

    [Fact]
    public void Rect_NotEqualTest()
    {
        Assert.True(Rectangle.Empty != new Rectangle(1, 2, 3, 4));
        Assert.False(Rectangle.Empty != new Rectangle(0, 0, 0, 0));
    }

    [Fact]
    public void Rectangle_GetHashCodeTest()
    {
        Rectangle rect = new(10, 20, 30, 40);
        Assert.Equal(rect.GetHashCode(), new Rectangle(10, 20, 30, 40).GetHashCode());
        Assert.NotEqual(rect.GetHashCode(), new Rectangle(20, 10, 40, 30).GetHashCode());
        Assert.NotEqual(rect.GetHashCode(), new Rectangle(10, 10, 10, 10).GetHashCode());
    }

    [Fact]
    public void Rectangle_ToStringTest()
    {
        Rectangle rect = new(10, 20, 30, 40);
        Assert.Equal($"{{X={rect.X}, Y={rect.Y}, Width={rect.Width}, Height={rect.Height}}}", rect.ToString());
    }
}