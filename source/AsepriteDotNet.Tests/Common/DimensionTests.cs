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

namespace AsepriteDotNet.Tests;

public sealed class DimensionTests
{
    [Fact]
    public void Dimension_AddTest()
    {
        Dimension left = new Dimension(1, 2);
        Dimension right = new Dimension(3, 4);

        Dimension expected = new Dimension(4, 6);

        Assert.Equal(expected, Dimension.Add(left, right));
        Assert.Equal(expected, left + right);
    }

    [Fact]
    public void Dimension_SubtractTest()
    {
        Dimension left = new Dimension(1, 2);
        Dimension right = new Dimension(3, 4);

        Dimension expected = new Dimension(-2, -2);

        Assert.Equal(expected, Dimension.Subtract(left, right));
        Assert.Equal(expected, left - right);
    }

    [Fact]
    public void Dimension_EqualTest()
    {
        Dimension a = new Dimension(1, 2);
        Dimension b = new Dimension(1, 2);

        Assert.True(a == b);
        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.False(a == Dimension.Empty);
        Assert.False(a.Equals(Dimension.Empty));
        Assert.False(a.Equals((object)Dimension.Empty));
    }

    [Fact]
    public void Dimension_NotEqualTest()
    {
        Assert.True(Dimension.Empty != new Dimension(1, 2));
        Assert.False(Dimension.Empty != new Dimension(0, 0));
    }
}