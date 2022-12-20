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

public class AseLayerTests
{

    [Fact]
    public void AseLayer_ConstructorTest()
    {
        bool isVisible = true;
        bool isBackground = false;
        bool isReference = false;
        int childLevel = 1;
        BlendMode blend = BlendMode.Multiply;
        int opacity = 2;
        string name = "Layer";

        AseLayer layer = new(isVisible, isBackground, isReference, childLevel, blend, opacity, name);

        Assert.Equal(isVisible, layer.IsVisible);
        Assert.Equal(isBackground, layer.IsBackground);
        Assert.Equal(isReference, layer.IsReference);
        Assert.Equal(childLevel, layer.ChildLevel);
        Assert.Equal(blend, layer.BlendMode);
        Assert.Equal(opacity, layer.Opacity);
        Assert.Equal(name, layer.Name);
        Assert.Null(layer.UserData);

        string text = "Hello World";
        Rgba32 color = Rgba32.FromRGBA(1, 2, 3, 4);
        AseUserData userdata = new(text, color);

        layer = new(isVisible, isBackground, isReference, childLevel, blend, opacity, name, userdata);
        Assert.NotNull(layer.UserData);
        Assert.Equal(color, layer.UserData.Color);
        Assert.Equal(text, layer.UserData.Text);
    }
}