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
using AsepriteDotNet.Primitives;

namespace AsepriteDotNet;

/// <summary>
///     Represents a single tile within a <see cref="Tilesheet"/> or a 
///     <see cref="Tilemap"/>.
/// </summary>
/// <param name="ID">
///     The ID of this <see cref="Tile"/> when used in a <see cref="Tilesheet"/> 
///     or the ID of the <see cref="Tile"/> to use when used in a 
///     <see cref="Tilemap"/>.
/// </param>
/// <param name="Source">
///     Defines the area within a <see cref="Tilesheet"/> this represented by
///     this <see cref="Tile"/> or the area within a <see cref="Tilemap"/> that
///     is represented by this <see cref="Tile"/>.
/// </param>
public sealed record Tile(int ID, Rectangle Source)
{
    /// <summary>
    ///     The width and height extent of this <see cref="Tile"/>.
    /// </summary>
    public Size Size => Source.Size;

    /// <summary>
    ///     The x- and y-coordinate location of the top-left corner of this
    ///     <see cref="Tile"/>.
    /// </summary>
    public Point Location => Source.Location;

    /// <summary>
    ///     The y-coordinate location of the top-left corner of this
    ///     <see cref="Tile"/>.
    /// </summary>
    public int Top => Source.Top;

    /// <summary>
    ///     The y-coordinate location of the bottom-right corner of this
    ///     <see cref="Tile"/>.
    /// </summary>
    public int Bottom => Source.Bottom;

    /// <summary>
    ///     The x-coordinate location of the top-left corner of this
    ///     <see cref="Tile"/>.
    /// </summary>
    public int Left => Source.Left;

    /// <summary>
    ///     The x-coordinate location of the bottom-right corner of this
    ///     <see cref="Tile"/>.
    /// </summary>
    public int Right => Source.Right;

    /// <summary>
    ///     The x-coordinate location of the top-left corner of this
    ///     <see cref="Tile"/>.
    /// </summary>
    public int X => Source.X;

    /// <summary>
    ///     The y-coordinate location of the top-left corner of this
    ///     <see cref="Tile"/>.
    /// </summary>
    public int Y => Source.Y;

    /// <summary>
    ///     The width of this <see cref="Tile"/>.
    /// </summary>
    public int Width => Source.Width;

    /// <summary>
    ///     The height of this <see cref="Tile"/>.
    /// </summary>
    public int Height => Source.Height;
}