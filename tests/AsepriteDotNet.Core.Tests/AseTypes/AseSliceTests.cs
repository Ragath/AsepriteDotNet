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

public class AseSliceTests
{

    [Fact]
    public void AseSlice_ConstructorTest()
    {
        bool isNinePatch = false;
        bool hasPivot = true;
        string name = "Slice";
        List<AseSliceKey> keys = new();

        AseSlice slice = new(isNinePatch, hasPivot, name, keys);
        Assert.Equal(isNinePatch, slice.IsNinePatch);
        Assert.Equal(hasPivot, slice.HasPivot);
        Assert.Equal(name, slice.Name);
        Assert.Empty(slice.Keys);
        Assert.Equal(0, slice.KeyCount);
        Assert.Null(slice.UserData);

        string text = "Hello World";
        Rgba32 color = Rgba32.FromRGBA(1, 2, 3, 4);
        AseUserData userdata = new(text, color);

        slice = new(isNinePatch, hasPivot, name, keys, userdata);
        Assert.NotNull(slice.UserData);
        Assert.Equal(color, slice.UserData.Color);
        Assert.Equal(text, slice.UserData.Text);
    }

    [Fact]
    public void AseSlice_IndexerTest()
    {
        AseSliceKey key = new(0, Rectangle.Empty);
        List<AseSliceKey> keys = new() { key };
        AseSlice slice = new(false, false, string.Empty, keys);

        Assert.Equal(keys[0], slice[0]);
    }
}