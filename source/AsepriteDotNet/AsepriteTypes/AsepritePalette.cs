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
using System.Collections;

using AsepriteDotNet.Color;

namespace AsepriteDotNet.AsepriteTypes;


/// <summary>
///     Represents the palette of colors in an Aseprite image.
/// </summary>
public sealed class AsepritePalette : IEnumerable<Rgba32>
{
    private Rgba32[] _colors = Array.Empty<Rgba32>();

    /// <summary>
    ///     Gets the <see cref="Rgba32"/> element at the specified
    ///     <paramref name="index"/> from this <see cref="AsepritePalette"/>.
    /// </summary>
    /// <param name="index">
    ///     The index of the <see cref="Rgba32"/> element.
    /// </param>
    /// <returns>
    ///     The <see cref="Rgba32"/> element at the specified 
    ///     <paramref name="index"/> from this <see cref="AsepritePalette"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="index"/> is less than zero or 
    ///     <paramref name="index"/> is equal to or greater than
    ///     <see cref="Count"/>.
    /// </exception>
    public Rgba32 this[int index]
    {
        get
        {
            if (index < 0 || index >= _colors.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _colors[index];
        }

        internal set
        {
            if (index < 0 || index >= _colors.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _colors[index] = value;
        }
    }

    /// <summary>
    ///     Gets the index of the <see cref="Rgba32"/> element in this
    ///     <see cref="AsepritePalette"/> that represents a transparent pixel.
    /// </summary>
    /// <remarks>
    ///     This value is only valid when the Aseprite image used a color depth
    ///     mode of <see cref="ColorDepth.Indexed"/>.
    /// </remarks>
    public int TransparentIndex { get; }

    /// <summary>
    ///     Get the total number of <see cref="Rgba32"/> elements in this
    ///     <see cref="AsepritePalette"/>.
    /// </summary>
    public int Count => _colors.Length;

    internal AsepritePalette(int transparentIndex) => TransparentIndex = transparentIndex;

    internal void Resize(int newSize)
    {
        Rgba32[] newColors = new Rgba32[newSize];
        Array.Copy(_colors, newColors, _colors.Length);
        _colors = newColors;
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the <see cref="Rgba32"/> 
    ///     elements in this <see cref="AsepritePalette"/> instance.
    /// </summary>
    /// <returns>
    ///     An enumerator that iterates through the <see cref="Rgba32"/> elements 
    ///     in this <see cref="AsepritePalette"/> instance.
    /// </returns>
    public IEnumerator<Rgba32> GetEnumerator()
    {
        for (int i = 0; i < _colors.Length; i++)
        {
            yield return _colors[i];
        }
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the <see cref="Rgba32"/> 
    ///     elements in this <see cref="AsepritePalette"/> instance.
    /// </summary>
    /// <returns>
    ///     An enumerator that iterates through the <see cref="Rgba32"/> elements
    ///     in this <see cref="AsepritePalette"/> instance.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}