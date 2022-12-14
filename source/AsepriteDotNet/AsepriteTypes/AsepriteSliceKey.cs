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
using AsepriteDotNet.Common;

namespace AsepriteDotNet.AsepriteTypes;

/// <summary>
///     Represents a key of a slice in an Aseprite image.
/// </summary>
public class AsepriteSliceKey
{
    /// <summary>
    ///     Gets the <see cref="Slice"/> this <see cref="AsepriteSliceKey"/> belongs
    ///     too.
    /// </summary>
    public AsepriteSlice Slice { get; }

    /// <summary>
    ///     Gets the index of the <see cref="Frame"/> this 
    ///     <see cref="AsepriteSliceKey"/> is valid for stating from to the end of the
    ///     animation.
    /// </summary>
    public int Frame { get; }

    /// <summary>
    ///     Gets the bounds of this <see cref="AsepriteSliceKey"/>.
    /// </summary>
    public Rectangle Bounds { get; }

    /// <summary>
    ///     Gets the bounds of the 9-patch center rectangle if it is part of
    ///     a 9-patch <see cref="Slice"/>, <see langword="null"/> if not.
    /// </summary>
    public Rectangle? CenterBounds { get; }

    /// <summary>
    ///     Gets the xy-coordinate pivot point relative the origin of if the
    ///     <see cref="Slice"/> contains pivot information,
    ///     <see langword="null"/> if not.
    /// </summary>
    public Location? Pivot { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AsepriteSliceKey"/> class.
    /// </summary>
    internal AsepriteSliceKey(AsepriteSlice slice, int frame, Rectangle bounds, Rectangle? center, Location? pivot)
    {
        Slice = slice;
        slice.AddKey(this);
        Bounds = bounds;
        CenterBounds = center;
        Pivot = pivot;
    }
}