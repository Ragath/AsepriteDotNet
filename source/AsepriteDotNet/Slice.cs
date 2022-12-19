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
using System.Diagnostics.CodeAnalysis;

using AsepriteDotNet.Color;
using AsepriteDotNet.Primitives;

namespace AsepriteDotNet;

/// <summary>
///     Represents a rectangular area defined within a single
///     <see cref="Frame"/>.
/// </summary>
/// <param name="Name">
///     The name of this <see cref="Slice"/>.
/// </param>
/// <param name="FrameIndex">
///     The index of the frame this <see cref="Slice"/> is valid on.
/// </param>
/// <param name="Bounds">
///     The rectangular bounds of this <see cref="Slice"/>.
/// </param>
/// <param name="Color">
///     The color of this <see cref="Slice"/>.
/// </param>
/// <param name="CenterBounds">
///     The rectangular bounds of the center of this <see cref="Slice"/> if it
///     is a nine patch <see cref="Slice"/>; otherwise, <see langword="null"/>.
/// </param>
/// <param name="Pivot">
///     The x- and y-coordinate pivot point of this <see cref="Slice"/> if it
///     has pivot data; otherwise, <see langword="null"/>.
/// </param>
public sealed record Slice(string Name, int FrameIndex, Rectangle Bounds, Rgba32 Color, Rectangle? CenterBounds = default, Point? Pivot = default)
{
    /// <summary>
    ///     The width and height extents of this <see cref="Slice"/>.
    /// </summary>
    public Size Size => Bounds.Size;
    
    /// <summary>
    ///     The x- and y-coordinate location of the top-left corner of this
    ///     <see cref="Slice"/>.
    /// </summary>
    public Point Location => Bounds.Location;

    /// <summary>
    ///     The y-coordinate location of the top-left corner of this
    ///     <see cref="Slice"/>.
    /// </summary>
    public int Top => Bounds.Top;

    /// <summary>
    ///     The y-coordinate location of the bottom-right corner of this
    ///     <see cref="Slice"/>.
    /// </summary>
    public int Bottom => Bounds.Bottom;

    /// <summary>
    ///     The x-coordinate location of the top-left corner of this
    ///     <see cref="Slice"/>.
    /// </summary>
    public int Left => Bounds.Left;

    /// <summary>
    ///     The x-coordinate location of the bottom-right corner of this
    ///     <see cref="Slice"/>.
    /// </summary>
    public int Right => Bounds.Right;

    /// <summary>
    ///     The x-coordinate location of the top-left corner of this
    ///     <see cref="Slice"/>.
    /// </summary>
    public int X => Bounds.X;

    /// <summary>
    ///     The y-coordinate location of the top-left corner of this
    ///     <see cref="Slice"/>.
    /// </summary>
    public int Y => Bounds.Y;

    /// <summary>
    ///     The width of this <see cref="Slice"/>.
    /// </summary>
    public int Width => Bounds.Width;

    /// <summary>
    ///     The height of this <see cref="Slice"/>.
    /// </summary>
    public int Height => Bounds.Height;

    /// <summary>
    ///     Indicates whether this <see cref="Slice"/> is a nine patch
    ///     <see cref="Slice"/>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(CenterBounds))]
    [MemberNotNullWhen(true, nameof(CenterSize))]
    [MemberNotNullWhen(true, nameof(CenterLocation))]
    [MemberNotNullWhen(true, nameof(CenterTop))]
    [MemberNotNullWhen(true, nameof(CenterBottom))]
    [MemberNotNullWhen(true, nameof(CenterLeft))]
    [MemberNotNullWhen(true, nameof(CenterRight))]
    [MemberNotNullWhen(true, nameof(CenterX))]
    [MemberNotNullWhen(true, nameof(CenterY))]
    [MemberNotNullWhen(true, nameof(CenterWidth))]
    [MemberNotNullWhen(true, nameof(CenterHeight))]
    public bool IsNinePatch => CenterBounds is not null;

    /// <summary>
    ///     The width and height extent of the center bounds of this
    ///     <see cref="Slice"/> if it is a nine patch <see cref="Slice"/>;
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public Size? CenterSize => CenterBounds?.Size;

    /// <summary>
    ///     The x- and y-coordinate of the top-left corner of the center bounds
    ///     of this <see cref="Slice"/> if it is a nine patch 
    ///     <see cref="Slice"/>; otherwise, <see langword="null"/>.
    /// </summary>
    public Point? CenterLocation => CenterBounds?.Location;

    /// <summary>
    ///     The y-coordinate of the top-left corner of the center bounds of this
    ///     <see cref="Slice"/> if it is a nine patch <see cref="Slice"/>;
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterTop => CenterBounds?.Top;

    /// <summary>
    ///     The y-coordinate of the bottom-right corner of the center bounds of
    ///     this <see cref="Slice"/> if it is a nine patch <see cref="Slice"/>;
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterBottom => CenterBounds?.Bottom;

    /// <summary>
    ///     The x-coordinate of the top-left corner of the center bounds of this
    ///     <see cref="Slice"/> if it is a nine patch <see cref="Slice"/>;
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterLeft => CenterBounds?.Left;

    /// <summary>
    ///     The x-coordinate of the bottom-right corner of the center bounds of
    ///     this <see cref="Slice"/> if it is a nine patch <see cref="Slice"/>;
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterRight => CenterBounds?.Right;

    /// <summary>
    ///     The x-coordinate of the top-left corner of the center bounds of this
    ///     <see cref="Slice"/> if it is a nine patch <see cref="Slice"/>;
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterX => CenterBounds?.X;

    /// <summary>
    ///     The y-coordinate of the top-left corner of the center bounds of this
    ///     <see cref="Slice"/> if it is a nine patch <see cref="Slice"/>;
    ///     otherwise, <see langword="null"/>.
    /// </summary>
    public int? CenterY => CenterBounds?.Y;

    /// <summary>
    ///     The width of the center bounds of this <see cref="Slice"/> if it
    ///     is a nine patch <see cref="Slice"/>; otherwise,
    ///     <see langword="null"/>
    /// </summary>
    public int? CenterWidth => CenterBounds?.Width;

    /// <summary>
    ///     The height of the center bounds of this <see cref="Slice"/> if it
    ///     is a nine patch <see cref="Slice"/>; otherwise,
    ///     <see langword="null"/>
    /// </summary>
    public int? CenterHeight => CenterBounds?.Height;

    /// <summary>
    ///     Indicates whether this <see cref="Slice"/> has pivot data.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Pivot))]
    [MemberNotNullWhen(true, nameof(PivotX))]
    [MemberNotNullWhen(true, nameof(PivotY))]
    public bool HasPivot => Pivot is not null;

    /// <summary>
    ///     The x-coordinate location of the pivot point for this
    ///     <see cref="Slice"/> if it has pivot data; otherwise,
    ///     <see langword="null"/>. 
    /// </summary>
    public int? PivotX => Pivot?.X;

    /// <summary>
    ///     The y-coordinate location of the pivot point for this
    ///     <see cref="Slice"/> if it has pivot data; otherwise,
    ///     <see langword="null"/>.
    /// </summary>
    public int? PivotY => Pivot?.Y;
}
