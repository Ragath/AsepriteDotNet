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
///     Represents a single frame in an Aseprite image.
/// </summary>
/// <param name="Size">
///     The width and height extents, in pixels, of this <see cref="AseFrame"/>.
/// </param>
/// <param name="Duration">
///     The duration, in milliseconds, of this <see cref="AseFrame"/> when used
///     in an animation.
/// </param>
/// <param name="Cels">
///     The collection of <see cref="AseCel"/> elements that are in this
///     <see cref="AseFrame"/>.
/// </param>
public record AseFrame(Size Size, int Duration, List<AseCel> Cels)
{
    /// <summary>
    ///     The width, in pixels, of this <see cref="AseFrame"/>.
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    ///     The height, in pixels, of this <see cref="AseFrame"/>.
    /// </summary>
    public int Height => Size.Height;

    /// <summary>
    ///     The total number of <see cref="AseCel"/> elements within this
    ///     <see cref="AseFrame"/>.
    /// </summary>
    public int CelCount => Cels.Count;

    /// <summary>
    ///     Returns the <see cref="AseCel"/> element at the specified 
    ///     <paramref name="index"/> from this <see cref="AseFrame"/>.
    /// </summary>
    /// <param name="index">
    ///     The index of the <see cref="AseCel"/> element within this
    ///     <see cref="AseFrame"/> to return.
    /// </param>
    /// <returns>
    ///     The <see cref="AseCel"/> element at the specified 
    ///     <paramref name="index"/> from this <see cref="AseFrame"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the specified <paramref name="index"/> is less than zero
    ///     or is greater than or equal to the <see cref="CelCount"/>.
    /// </exception>
    public AseCel this[int index]
    {
        get
        {
            if (index < 0 || index >= CelCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Cels[index];
        }
    }

    /// <summary>
    ///     Returns a new <see cref="Array"/> of <see cref="Rgba32"/> color
    ///     elements that represents all <see cref="AseImageCel"/> elements
    ///     in this <see cref="AseFrame"/> flattened to produce a single
    ///     image.
    /// </summary>
    /// <remarks>
    ///     The <see cref="AseFrame"/> is flattened by taking each 
    ///     <see cref="AseImageCel"/> element in this <see cref="AseFrame"/>
    ///     starting with the one on the top-most <see cref="AseLayer"/> element
    ///     and blending it down with the <see cref="AseImageCel"/> element on 
    ///     the <see cref="AseLayer"/> below it using the 
    ///     <see cref="BlendMode"/> of the top <see cref="AseLayer"/> element,
    ///     until all <see cref="AseImageCel"/> elements are blended,producing 
    ///     the final image.
    /// </remarks>
    /// <param name="onlyVisibleLayers">
    ///     Indicates whether only <see cref="AseImageCel"/> elements that are
    ///     on <see cref="AseLayer"/> elements that are visible should be
    ///     included.
    /// </param>
    /// <returns>
    ///     A new <see cref="Array"/> of <see cref="Rgba32"/> elements that
    ///     represents the pixel data for the image produced from flattening
    ///     this <see cref="AseFrame"/>.  Order of pixel data is from
    ///     top-to-bottom, read left-to-right.
    /// </returns>
    public Rgba32[] FlattenFrame(bool onlyVisibleLayers = true)
    {
        Rgba32[] result = new Rgba32[Width * Height];

        for (int celNum = 0; celNum < CelCount; celNum++)
        {
            AseCel cel = Cels[celNum];

            //  Are we only processing cels on visible layers?
            if (onlyVisibleLayers && !cel.Layer.IsVisible) { continue; }

            //  Only process image cels
            if (cel is AseImageCel imageCel)
            {
                byte preMultipliedOpacity = Rgba32.MUL_UN8(imageCel.Opacity, imageCel.Layer.Opacity);

                for (int pixelNum = 0; pixelNum < imageCel.PixelCount; pixelNum++)
                {
                    int x = (pixelNum % imageCel.Width) + imageCel.X;
                    int y = (pixelNum / imageCel.Width) + imageCel.Y;
                    int index = y * Width + x;

                    //  Sometimes a cell can have a negative x and/or y 
                    //  value. This is caused by selecting an area within 
                    //  aseprite and then moving a portion of the selected
                    //  pixels outside the canvas.  We don't care about 
                    //  these pixels so if the index is outside the range of
                    //  the array to store them in then we'll just ignore 
                    //  them.
                    if (index < 0 || index >= result.Length) { continue; }

                    Rgba32 backdrop = result[index];
                    Rgba32 source = imageCel[pixelNum];
                    result[index] = Rgba32.Blend(cel.Layer.BlendMode, backdrop, source, preMultipliedOpacity);
                }
            }
        }

        return result;
    }

    /// <summary>
    ///     Exports this <see cref="AseFrame"/> to disk as a PNG (.png) file.
    /// </summary>
    /// <param name="path">
    ///     The absolute path and filename, including extension, of the PNG
    ///     (.png) file that will be created.  If the file already exists, it
    ///     will be overwritten.
    /// </param>
    /// <param name="onlyVisibleLayers">
    ///     Indicates whether only <see cref="AseImageCel"/> elements that are
    ///     on <see cref="AseLayer"/> elements that are visible should be
    ///     included.
    /// </param>
    public void ExportAsPng(string path, bool onlyVisibleLayers = true)
    {
        Rgba32[] pixels = FlattenFrame(onlyVisibleLayers);
        PngWriter.SaveTo(path, Size, pixels);
    }
}