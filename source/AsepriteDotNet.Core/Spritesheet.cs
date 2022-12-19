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
using System.Text.Json.Serialization;

using AsepriteDotNet.Core.Color;
using AsepriteDotNet.Core.IO;
using AsepriteDotNet.Core.Primitives;
using AsepriteDotNet.Core.Serialization;

namespace AsepriteDotNet.Core;

/// <summary>
///     Represents the exported spritesheet data from an 
///     <see cref="AsepriteFile"/>, including the pixel image, frame, tag,
///     and slice data.
/// </summary>
/// <param name="Size">
///     The width and height extents, in pixels, of this 
///     <see cref="Spritesheet"/>.
/// </param>
/// <param name="Pixels">
///     An <see cref="Array"/> of <see cref="Rgba32"/> color elements that
///     represent the pixel data for this <see cref="Spritesheet"/>.  Order
///     of pixel data is from top-to-bottom, read left-to-right.
/// </param>
/// <param name="Frames">
///     The collection of all <see cref="Frame"/> elements in this
///     <see cref="Spritesheet"/>.
/// </param>
[JsonConverter(typeof(SpritesheetConverter))]
public record Spritesheet(Size Size, Rgba32[] Pixels, List<Frame> Frames)
{
    /// <summary>
    ///     The width, in pixels, of this <see cref="Spritesheet"/>/
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    ///     The height, in pixels, of this <see cref="Spritesheet"/>.
    /// </summary>
    public int Height => Size.Height;

    /// <summary>
    ///     The total number of <see cref="Rgba32"/> pixel color elements in
    ///     this <see cref="Spritesheet"/>.
    /// </summary>
    public int PixelCount => Pixels.Length;

    /// <summary>
    ///     The total number of <see cref="Frame"/> elements in this
    ///     <see cref="Spritesheet"/>.
    /// </summary>
    public int FrameCount => Frames.Count;

    /// <summary>
    ///     Returns the <see cref="Frame"/> element at the specified
    ///     <paramref name="index"/> from this <see cref="Spritesheet"/>.
    /// </summary>
    /// <param name="index">
    ///     The index of the <see cref="Frame"/> element within this
    ///     <see cref="Spritesheet"/> to return.
    /// </param>
    /// <returns>
    ///     The <see cref="Frame"/> element at the specified 
    ///     <paramref name="index"/> from this <see cref="Spritesheet"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the specified <paramref name="index"/> is less than zero
    ///     or is greater than or equal to <see cref="FrameCount"/>.
    /// </exception>
    public Frame this[int index]
    {
        get
        {
            if (index < 0 || index >= FrameCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Frames[index];
        }
    }

    /// <summary>
    ///     Exports this <see cref="Spritesheet"/> to disk as a PNG (.png) file.
    /// </summary>
    /// <param name="path">
    ///     The absolute path and filename, including extension, of the PNG
    ///     (.png) file that will be created.  If the file already exists, it
    ///     will be overwritten.
    /// </param>
    public void ExportAsPng(string path) => PngWriter.SaveTo(path, Size, Pixels);

}