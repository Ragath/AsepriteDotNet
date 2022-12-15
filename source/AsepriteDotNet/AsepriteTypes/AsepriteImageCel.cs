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
using AsepriteDotNet.Color;
using AsepriteDotNet.IO.Image;
using AsepriteDotNet.Primitives;

namespace AsepriteDotNet.AsepriteTypes;

/// <summary>
///     Represents a <see cref="AsepriteCel"/> that contains image data in an Aseprite
///     file.
/// </summary>
public sealed class AsepriteImageCel : AsepriteCel
{
    /// <summary>
    ///     Gets the width and height components of this <see cref="AsepriteImageCel"/>
    ///     as a <see cref="Size"/> value.
    /// </summary> 
    public Size Size { get; }

    /// <summary>
    ///     Gets an <see cref="Array"/> of <see cref="Rgba32"/> elements that
    ///     represents the raw pixel data for this <see cref="AsepriteImageCel"/>.
    /// </summary>
    /// <remarks>
    ///     Order of pixels is row by row, from top to bottom, for each scanline
    ///     read pixels from left to right.
    /// </remarks>
    public Rgba32[] Pixels { get; } = Array.Empty<Rgba32>();

    internal AsepriteImageCel(Size size, Rgba32[] pixels, AsepriteLayer layer, Point position, int opacity)
        : base(layer, position, opacity) => (Size, Pixels) = (size, pixels);

    /// <summary>
    ///     Writes the pixel data for this <see cref="AsepriteImageCel"/> to disk as a 
    ///     .png file.
    /// </summary>
    /// <param name="path">
    ///     The absolute file path to save the generated .png file to.
    /// </param>
    public void ToPng(string path) => PngWriter.SaveTo(path, Size, Pixels);
}