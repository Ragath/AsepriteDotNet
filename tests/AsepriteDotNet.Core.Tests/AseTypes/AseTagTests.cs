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

public class AseTagTests
{

    [Fact]
    public void AseTag_ConstructorTest()
    {
        int from = 1;
        int to = 2;
        LoopDirection direction = LoopDirection.Reverse;
        Rgba32 tagColor = Rgba32.FromRGBA(255, 0, 0, 255);
        string name = "Tag";

        AseTag tag = new(from, to, direction, tagColor, name);

        Assert.Equal(from, tag.From);
        Assert.Equal(to, tag.To);
        Assert.Equal(direction, tag.Direction);
        Assert.Equal(tagColor, tag.Color);
        Assert.Equal(name, tag.Name);
        Assert.Null(tag.UserData);

        string text = "Hello World";
        Rgba32 userDataColor = Rgba32.FromRGBA(1, 2, 3, 4);
        AseUserData userdata = new(text, userDataColor);

        tag = new(from, to, direction, tagColor, name, userdata);

        Assert.NotNull(tag.UserData);
        Assert.Equal(text, tag.UserData.Text);
        Assert.Equal(userDataColor, tag.UserData.Color);
    }
}