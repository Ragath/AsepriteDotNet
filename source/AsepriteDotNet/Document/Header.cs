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
using System.Drawing;

namespace AsepriteDotNet.Document;

public class Header
{
    private Size _size = new(1, 1);
    private int _transparentIndex = 0;

    public int Frames { get; set; }
    public Size Size
    {
        get => _size;
        set
        {
            if(value.Width < 1 || value.Height < 1)
            {
                throw new InvalidOperationException($"{nameof(Size)} width and height must be greater than zero");
            }

            _size = value;
        }
    }

    public ColorDepth ColorDepth { get; set; }
    public int TransparentIndex
    {
        get => ColorDepth == ColorDepth.Indexed ? _transparentIndex : 0;
        set => _transparentIndex = value;
    }

    public int NumberOfColors { get; set; }


    public Header() { }
}