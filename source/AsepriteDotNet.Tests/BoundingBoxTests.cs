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
namespace AsepriteDotNet.Tests;

public sealed class BoundingBoxTests
{
    [Fact]
    public void BoundingBox_EqualTest()
    {
        BoundingBox a = new BoundingBox(1, 2, 3, 4);
        BoundingBox b = new BoundingBox(1, 2, 3, 4);

        Assert.True(a == b);
        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.False(a == BoundingBox.Empty);
        Assert.False(a.Equals(BoundingBox.Empty));
        Assert.False(a.Equals((object)BoundingBox.Empty));
    }

    [Fact]
    public void BoundingBox_NotEqualTest()
    {
        Assert.True(BoundingBox.Empty != new BoundingBox(1, 2, 3, 4));
        Assert.False(BoundingBox.Empty != new BoundingBox(0, 0, 0, 0));
    }
}