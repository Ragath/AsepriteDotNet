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
using System.Diagnostics.CodeAnalysis;

using AsepriteDotNet.Core.Primitives;

namespace AsepriteDotNet.Core.AseTypes;

/// <summary>
///     Represents a single instance key of an <see cref="AseSlice"/> in an
///     Aseprite image.
/// </summary>
/// <param name="Frame">
///     The index of the <see cref="AseFrame"/> this <see cref="AseSliceKey"/>
///     is valid starting on.
/// </param>
/// <param name="Bounds">
///     The bounds of this <see cref="AseSliceKey"/> relative to the bounds of
///     the <see cref="AseFrame"/> it is in.
/// </param>
/// <param name="CenterBounds">
///     The bounds of the center rectangle for this <see cref="AseSliceKey"/>
///     relative to the <see cref="Bounds"/>, if it is a nine patch; otherwise, 
///     <see langword="null"/>.
/// </param>
/// <param name="Pivot">
///     The x- and y-coordinate location of the pivot point for this
///     <see cref="AseSliceKey"/>, relative to the <see cref="Bounds"/>, if it
///     has pivot data; otherwise, <see langword="null"/>.
/// </param>
public record AseSliceKey(int Frame, Rectangle Bounds, Rectangle? CenterBounds, Point? Pivot)
{
    /// <summary>
    ///     Indicates whether this <see cref="AseSliceKey"/> is a nine patch.
    /// </summary>
    [MemberNotNullWhen(true, nameof(CenterBounds))]
    [MemberNotNullWhen(true, nameof(CenterSize))]
    [MemberNotNullWhen(true, nameof(CenterWidth))]
    [MemberNotNullWhen(true, nameof(CenterHeight))]
    [MemberNotNullWhen(true, nameof(CenterLocation))]
    [MemberNotNullWhen(true, nameof(CenterX))]
    [MemberNotNullWhen(true, nameof(CenterY))]
    [MemberNotNullWhen(true, nameof(CenterTop))]
    [MemberNotNullWhen(true, nameof(CenterBottom))]
    [MemberNotNullWhen(true, nameof(CenterLeft))]
    [MemberNotNullWhen(true, nameof(CenterRight))]
    public bool IsNinePatch => CenterBounds is not null;

    /// <summary>
    ///     Indicates whether this <see cref="AseSliceKey"/> has pivot data.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Pivot))]
    [MemberNotNullWhen(true, nameof(PivotX))]
    [MemberNotNullWhen(true, nameof(PivotY))]
    public bool HasPivot => Pivot is not null;

    /// <summary>
    ///     The width and height extents, in pixels, of this 
    ///     <see cref="AseSliceKey"/>.
    /// </summary>
    public Size Size => Bounds.Size;

    /// <summary>
    ///     The width, in pixels, of this <see cref="AseSliceKey"/>.
    /// </summary>
    public int Width => Bounds.Width;

    /// <summary>
    ///     The height, in pixels, of this <see cref="AseSliceKey"/>.
    /// </summary>
    public int Height => Bounds.Height;

    /// <summary>
    ///     The x- and y-coordinate location of the top-left corner of this
    ///     <see cref="AseSliceKey"/>, relative to the bounds of the
    ///     <see cref="AseFrame"/> it is in.
    /// </summary>
    public Point Location => Bounds.Location;

    /// <summary>
    ///     The x-coordinate location of the top-left corner of this
    ///     <see cref="AseSliceKey"/>, relative to the bounds of the
    ///     <see cref="AseFrame"/> it is in.
    /// </summary> 
    public int X => Bounds.X;

    /// <summary>
    ///     The y-coordinate location of the top-left corner of this
    ///     <see cref="AseSliceKey"/>, relative to the bounds of the
    ///     <see cref="AseFrame"/> it is in.
    /// </summary> 
    public int Y => Bounds.Y;


    /// <summary>
    ///     The y-coordinate location of the top-left corner of this
    ///     <see cref="AseSliceKey"/>, relative to the bounds of the
    ///     <see cref="AseFrame"/> it is in.
    /// </summary> 
    public int Top => Bounds.Top;

    /// <summary>
    ///     The y-coordinate location of the bottom-right corner of this
    ///     <see cref="AseSliceKey"/>, relative to the bounds of the
    ///     <see cref="AseFrame"/> it is in.
    /// </summary>
    public int Bottom => Bounds.Bottom;

    /// <summary>
    ///     The x-coordinate location of the top-left corner of this
    ///     <see cref="AseSliceKey"/>, relative to the bounds of the
    ///     <see cref="AseFrame"/> it is in.
    /// </summary>    
    public int Left => Bounds.Left;

    /// <summary>
    ///     The x-coordinate location of the bottom-right corner of this
    ///     <see cref="AseSliceKey"/>, relative to the bounds of the
    ///     <see cref="AseFrame"/> it is in.
    /// </summary>
    public int Right => Bounds.Right;

    /// <summary>
    ///     The width and height extents, in pixels, of the 
    ///     <see cref="CenterBounds"/> of this <see cref="AseSliceKey"/>, if it
    ///     is a nine patch slice; otherwise, <see langword="null"/>.
    /// </summary>
    public Size? CenterSize => CenterBounds?.Size;

    /// <summary>
    ///     The width, in pixels of the <see cref="CenterBounds"/> of this
    ///     <see cref="AseSliceKey"/>, if it is a nine patch slice; otherwise,
    ///     <see langword="null"/>.
    /// </summary>
    public int? CenterWidth => CenterBounds?.Width;

    /// <summary>
    ///     The height, in pixels of the <see cref="CenterBounds"/> of this
    ///     <see cref="AseSliceKey"/>, if it is a nine patch slice; otherwise,
    ///     <see langword="null"/>.
    /// </summary>
    public int? CenterHeight => CenterBounds?.Height;

    /// <summary>
    ///     The x- and y-coordinate location of the top-left corner of the
    ///     <see cref="CenterBounds"/> of this <see cref="AseSliceKey"/>, 
    ///     relative to the <see cref="Bounds"/>, if it is a nine patch slice; 
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public Point? CenterLocation => CenterBounds?.Location;

    /// <summary>
    ///     The x-coordinate location of the top-left corner of the
    ///     <see cref="CenterBounds"/> of this <see cref="AseSliceKey"/>, 
    ///     relative to the <see cref="Bounds"/>, if it is a nine patch slice; 
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterX => CenterBounds?.X;

    /// <summary>
    ///     The y-coordinate location of the top-left corner of the
    ///     <see cref="CenterBounds"/> of this <see cref="AseSliceKey"/>, 
    ///     relative to the <see cref="Bounds"/>, if it is a nine patch slice; 
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterY => CenterBounds?.Y;

    /// <summary>
    ///     The x-coordinate location of the top-left corner of the
    ///     <see cref="CenterBounds"/> of this <see cref="AseSliceKey"/>, 
    ///     relative to the <see cref="Bounds"/>, if it is a nine patch slice; 
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterTop => CenterBounds?.Top;

    /// <summary>
    ///     The y-coordinate location of the bottom-right corner of the
    ///     <see cref="CenterBounds"/> of this <see cref="AseSliceKey"/>, 
    ///     relative to the <see cref="Bounds"/>, if it is a nine patch slice; 
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterBottom => CenterBounds?.Bottom;

    /// <summary>
    ///     The x-coordinate location of the top-left corner of the
    ///     <see cref="CenterBounds"/> of this <see cref="AseSliceKey"/>, 
    ///     relative to the <see cref="Bounds"/>, if it is a nine patch slice; 
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterLeft => CenterBounds?.Left;

    /// <summary>
    ///     The x-coordinate location of the bottom-right corner of the
    ///     <see cref="CenterBounds"/> of this <see cref="AseSliceKey"/>, 
    ///     relative to the <see cref="Bounds"/>, if it is a nine patch slice; 
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterRight => CenterBounds?.Right;

    /// <summary>
    ///     The x-coordinate location of the pivot point for this
    ///     <see cref="AseSliceKey"/> relative to the <see cref="X"/> of this
    ///     <see cref="AseSliceKey"/>.
    /// </summary>
    public int? PivotX => Pivot?.X;

    /// <summary>
    ///     The y-coordinate location of the pivot point for this
    ///     <see cref="AseSliceKey"/> relative to the <see cref="Y"/> of this
    ///     <see cref="AseSliceKey"/>.
    /// </summary>
    public int? PivotY => Pivot?.Y;
}