/* -----------------------------------------------------------------------------
Copyright 2022 Christopher Whitley

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
----------------------------------------------------------------------------- */
using AsepriteDotNet.Core.Color;

namespace AsepriteDotNet.Core.AseTypes;

public record AsePalette(int TransparentIndex)
{
    /// <summary>
    ///     The collection of <see cref="Rgba32"/> color elements that represent
    ///     this <see cref="AsePalette"/>.
    /// </summary>
    public Rgba32[] Colors { get; private set; } = Array.Empty<Rgba32>();

    /// <summary>
    ///     The total number of <see cref="Rgba32"/> color elements in this
    ///     <see cref="AsePalette"/>.
    /// </summary>
    public int Count => Colors.Length;

    /// <summary>
    ///     Returns the <see cref="Rgba32"/> color element at the specified
    ///     <paramref name="index"/> from this <see cref="AsePalette"/>.
    /// </summary>
    /// <param name="index">
    ///     The index of the <see cref="Rgba32"/> color element within this
    ///     <see cref="AsePalette"/> to return.
    /// </param>
    /// <returns>
    ///     The <see cref="Rgba32"/> color element at the specified 
    ///     <paramref name="index"/> from this <see cref="AsePalette"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if this specified <paramref name="index"/> is less than zero
    ///     or is greater than or equal to the <see cref="Count"/>.
    /// </exception>
    public Rgba32 this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Colors[index];
        }
        set
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            Colors[index] = value;
        }
    }

    internal void Resize(int newSize)
    {
        Rgba32[] newColors = new Rgba32[newSize];
        Array.Copy(Colors, newColors, Count);
        Colors = newColors;
    }
}