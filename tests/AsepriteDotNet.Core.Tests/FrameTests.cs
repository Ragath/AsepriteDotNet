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

public class FrameTests
{

    [Fact]
    public void Frame_ConstructorTest()
    {
        Rectangle bounds = new(10, 20, 30, 40);
        TimeSpan duration = TimeSpan.FromMilliseconds(50);

        Frame frame = new(bounds, duration);

        Assert.Equal(bounds, frame.Source);
        Assert.Equal(bounds.Size, frame.Size);
        Assert.Equal(bounds.Width, frame.Width);
        Assert.Equal(bounds.Height, frame.Height);
        Assert.Equal(bounds.Location, frame.Location);
        Assert.Equal(bounds.X, frame.X);
        Assert.Equal(bounds.Y, frame.Y);
        Assert.Equal(bounds.Top, frame.Top);
        Assert.Equal(bounds.Bottom, frame.Bottom);
        Assert.Equal(bounds.Left, frame.Left);
        Assert.Equal(bounds.Right, frame.Right);
        Assert.Equal(duration.TotalMilliseconds, frame.TotalMilliseconds);
        Assert.Equal(duration.TotalSeconds, frame.TotalSeconds);
    }
}