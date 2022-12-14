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
using System.Collections;
using System.Collections.ObjectModel;

using AsepriteDotNet.IO.Image;

namespace AsepriteDotNet.AsepriteTypes;

/// <summary>
///     Represents a frame in an Aseprite image.
/// </summary>
public sealed class AsepriteFrame : IEnumerable<AsepriteCel>
{
    /// <summary>
    ///     Gets the width and height of this <see cref="AsepriteFrame"/>.
    /// </summary>
    public Dimension Size { get; }

    /// <summary>
    ///     Gets the duration, in milliseconds, of this <see cref="AsepriteFrame"/> when
    ///     used as part of an animation.
    /// </summary>
    public int Duration { get; }

    /// <summary>
    ///     Gets the <see cref="AsepriteCel"/> in this frame at the specified index.
    /// </summary>
    /// <param name="index">
    ///     The index of the <see cref="AsepriteCel"/> to retrieve.
    /// </param>
    /// <returns>
    ///     The <see cref="AsepriteCel"/> at the specified index in this 
    ///     <see cref="AsepriteFrame"/>.
    /// </returns>
    public AsepriteCel this[int index]
    {
        get => Cels[index];
    }

    /// <summary>
    ///     Gets a read-only collection of all <see cref="AsepriteCel"/> instances in
    ///     this <see cref="AsepriteFrame"/>.
    /// </summary>
    public ReadOnlyCollection<AsepriteCel> Cels { get; }

    internal AsepriteFrame(int duration, List<AsepriteCel> cels, Dimension size)
    {
        Duration = duration;
        Cels = cels.AsReadOnly();
        Size = size;
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the <see cref="AsepriteCel"/>
    ///     elements in this <see cref="AsepriteFrame"/>.
    /// </summary>
    /// <returns>
    ///     An enumerator that iterates through the <see cref="AsepriteCel"/> elements
    ///     in this <see cref="AsepriteFrame"/>.
    /// </returns>
    public IEnumerator<AsepriteCel> GetEnumerator() => Cels.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Cels.GetEnumerator();

    /// <summary>
    ///     Flattens this <see cref="AsepriteFrame"/> by blending each <see cref="AsepriteCel"/>
    ///     staring with the top most <see cref="AsepriteCel"/> and blending down.  Te
    ///     result is an <see cref="Array"/> of <see cref="Rgba32"/> elements
    ///     representing the final flattened image of this <see cref="AsepriteFrame"/>.
    /// </summary>
    /// <param name="onlyVisibleLayers">
    ///     Whether only the <see cref="AsepriteCel"/> elements that are on a
    ///     <see cref="AsepriteLayer"/> that is visible should be included.
    /// </param>
    /// <returns>
    ///     A new <see cref="Array"/> of <see cref="Rgba32"/> elements that
    ///     represent the flattened image of this <see cref="AsepriteFrame"/>.
    /// </returns>
    public Rgba32[] FlattenFrame(bool onlyVisibleLayers = true)
    {
        Rgba32[] result = new Rgba32[Size.Width * Size.Height];

        for (int celNum = 0; celNum < Cels.Count; celNum++)
        {
            AsepriteCel cel = Cels[celNum];

            if (onlyVisibleLayers && !cel.Layer.IsVisible)
            {
                continue;
            }

            AsepriteImageCel imageCel;

            if (cel is AsepriteImageCel)
            {
                imageCel = (AsepriteImageCel)cel;
            }
            else if (cel is AsepriteLinkedCel linkedCel && linkedCel.Cel is AsepriteImageCel)
            {
                imageCel = (AsepriteImageCel)linkedCel.Cel;
            }
            else
            {

                continue;
            }

            byte opacity = Rgba32.MUL_UN8(imageCel.Opacity, imageCel.Layer.Opacity);

            for (int pixelNum = 0; pixelNum < imageCel.Pixels.Length; pixelNum++)
            {
                int x = (pixelNum % imageCel.Size.Width) + imageCel.Position.X;
                int y = (pixelNum / imageCel.Size.Width) + imageCel.Position.Y;
                int index = y * Size.Width + x;

                //  Sometimes a cell can have a negative x and/or y value. This
                //  is caused by selecting an area within aseprite and then 
                //  moving a portion of the selected pixels outside the canvas. 
                //  We don't care about these pixels so if the index is outside
                //  the range of the array to store them in then we'll just
                //  ignore them.
                if (index < 0 || index >= result.Length) { continue; }

                Rgba32 backdrop = result[index];
                Rgba32 source = imageCel.Pixels[pixelNum];
                result[index] = Rgba32.Blend(imageCel.Layer.BlendMode, backdrop, source, opacity);
            }
        }

        return result;
    }

    /// <summary>
    ///     Writes the pixel data for this <see cref="AsepriteFrame"/> to disk as a .png
    ///     file.
    /// </summary>
    /// <param name="path">
    ///     The absolute file path to save the generated .png file to.
    /// </param>
    /// <param name="onlyVisibleLayers">
    ///     Whether only the <see cref="AsepriteCel"/> elements that are on a 
    ///     <see cref="AsepriteLayer"/> that is visible should be included.
    /// </param>
    public void ToPng(string path, bool onlyVisibleLayers = true)
    {
        Rgba32[] frame = FlattenFrame(onlyVisibleLayers);
        PngWriter.SaveTo(path, Size, frame);
    }
}