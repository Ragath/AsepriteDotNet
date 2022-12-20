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

public class AseUserDataTests
{

    [Fact]
    public void AseUser_ConstructorTest()
    {
        string? text = default;
        Rgba32? color = default;

        AseUserData userData = new(text, color);

        Assert.False(userData.HasText);
        Assert.False(userData.HasColor);
        Assert.Equal(text, userData.Text);
        Assert.Equal(color, userData.Color);

        text = "User Data";
        userData = new(text, color);
        Assert.True(userData.HasText);
        Assert.Equal(text, userData.Text);

        color = Rgba32.FromRGBA(255, 0, 0, 255);
        userData = new(text, color);
        Assert.True(userData.HasColor);
        Assert.Equal(color, userData.Color);
    }
}