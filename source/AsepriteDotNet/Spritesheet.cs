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
using AsepriteDotNet.Primitives;

namespace AsepriteDotNet;

public record Spritesheet(Size Size, Rgba32[] Pixels, List<Frame> Frames, TagCollection Tags, SliceCollection Slices)
{
    /// <summary>
    ///     The width of this <see cref="Spritesheet"/>.
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    ///     The height of this <see cref="Spritesheet"/>.
    /// </summary>
    public int Height => Size.Height;

    /// <summary>
    ///     The number of <see cref="Frame"/> elements in this
    ///     <see cref="Spritesheet"/>.
    /// </summary>
    public int FrameCount => Frames.Count;

    /// <summary>
    ///     The number of <see cref="Tag"/> elements in this
    ///     <see cref="Spritesheet"/>.
    /// </summary>
    public int TagCount => Tags.Count;

    /// <summary>
    ///     The number of <see cref="Slice"/> elements in this 
    ///     <see cref="Spritesheet"/>.
    /// </summary>
    public int SliceCount => Slices.Count;

    /// <summary>
    ///     Returns the <see cref="Frame"/> element at the specified 
    ///     <paramref name="frameIndex"/>.
    /// </summary>
    /// <param name="frameIndex">
    ///     The index of the <see cref="Frame"/> element to return.
    /// </param>
    /// <returns>
    ///     The <see cref="Frame"/> element at the specified 
    ///     <paramref name="frameIndex"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the specified <paramref name="frameIndex"/> is less than
    ///     zero or equal to or greater than <see cref="FrameCount"/>.
    /// </exception>
    public Frame this[int frameIndex]
    {
        get
        {
            if(frameIndex < 0 || frameIndex >= FrameCount)
            {
                throw new ArgumentOutOfRangeException(nameof(frameIndex));
            }

            return Frames[frameIndex];
        }
    }
}