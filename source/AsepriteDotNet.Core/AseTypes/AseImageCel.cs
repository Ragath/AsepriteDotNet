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
using AsepriteDotNet.Core.IO;
using AsepriteDotNet.Core.Primitives;

namespace AsepriteDotNet.Core.AseTypes;

/// <summary>
///     Represents a single frame image cel in an Aseprite image.
/// </summary>
/// <param name="Size">
///     The width and height extent, in pixels, of this <see cref="AseImageCel"/>
///     The <see cref="AseLayer"/> that this <see cref="AseImageCel"/> is on.
/// </param>
/// <param name="Pixels">
///     An <see cref="Array"/> of <see cref="Rgba32"/> elements that represent
///     the pixel data for this <see cref="AseImageCel"/>.  Pixel data is in
///     order from top-to-bottom, read left-to-right.
/// </param>
/// <param name="Layer">
///     The <see cref="AseLayer"/> that this <see cref="AseImageCel"/> is on.
/// </param>
/// <param name="Position">
///     The x- and y-coordinate position of the top-left corner of this
///     <see cref="AseImageCel"/> relative to the bounds of the 
///     <see cref="AseFrame"/> it is contained within.
/// </param>
/// <param name="Opacity">
///     The opacity level of this <see cref="AseImageCel"/>.
/// </param>
/// <param name="UserData">
///     The custom <see cref="UserData"/> that was set for this 
///     <see cref="AseImageCel"/> within Aseprite.
/// </param>
public record AseImageCel(Size Size, Rgba32[] Pixels, AseLayer Layer, Point Position, int Opacity, AseUserData? UserData = default)
    : AseCel(Layer, Position, Opacity, UserData)
{
    /// <summary>
    ///     The width, in pixels, of this <see cref="AseImageCel"/>.
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    ///     The height, in pixels, of this <see cref="AseImageCel"/>.
    /// </summary>
    public int Height => Size.Height;

    /// <summary>
    ///     The total number of <see cref="Rgba32"/> color elements representing
    ///     the pixels in this <see cref="AseImageCel"/>.
    /// </summary>
    public int PixelCount => Pixels.Length;

    /// <summary>
    ///     Returns the <see cref="Rgba32"/> color element at the specified
    ///     <paramref name="index"/> from this <see cref="AseImageCel"/>.
    /// </summary>
    /// <param name="index">
    ///     The index of the <see cref="Rgba32"/> color element within this
    ///     <see cref="AseImageCel"/> to return.
    /// </param>
    /// <returns>
    ///     The <see cref="Rgba32"/> color element at the specified
    ///     <paramref name="index"/> from this <see cref="AseImageCel"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the specified <paramref name="index"/> is less than zero
    ///     or is greater than or equal to the <see cref="PixelCount"/>.
    /// </exception>
    public Rgba32 this[int index]
    {
        get
        {
            if (index < 0 || index >= PixelCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Pixels[index];
        }
    }

    /// <summary>
    ///     Exports this <see cref="AseImageCel"/> to disk as a PNG (.png) file.
    /// </summary>
    /// <param name="path">
    ///     The absolute path and filename, including extension, of the PNG
    ///     (.png) file that will be created.  If the file already exists, it
    ///     will be overwritten.
    /// </param>
    public void ExportAsPng(string path) => PngWriter.SaveTo(path, Size, Pixels);
}