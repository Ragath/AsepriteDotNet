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
using AsepriteDotNet.Core.Primitives;

namespace AsepriteDotNet.Core.AseTypes;

/// <summary>
///     Represents a single frame tilemap cel in an Aseprite image.
/// </summary>
/// <param name="Size">
///     The width and height extents, in tiles, of this
///     <see cref="AseTilemapCel"/>.
/// </param>
/// <param name="TileSize">
///     The width and height extents, in pixels, of each <see cref="AseTile"/>
///     element in this <see cref="AseTilemapCel"/>.
/// </param>
/// <param name="Tiles">
///     The collection of <see cref="AseTile"/> elements in this
///     <see cref="AseTilemapCel"/>.
/// </param>
/// <param name="Layer">
///     The <see cref="AseLayer"/> that this <see cref="AseTilemapCel"/> is on.
/// </param>
/// <param name="Position">
///     The x- and y-coordinate position of the top-left corner of this
///     <see cref="AseTilemapCel"/> relative to the bounds of the 
///     <see cref="AseFrame"/> it is contained within.
/// </param>
/// <param name="Opacity">
///     The opacity level of this <see cref="AseTilemapCel"/>.
/// </param>
/// <param name="UserData">
///     The custom <see cref="UserData"/> that was set for this 
///     <see cref="AseTilemapCel"/> within Aseprite.
/// </param>
public record AseTilemapCel(Size Size, Size TileSize, List<AseTile> Tiles, AseLayer Layer, Point Position, int Opacity, AseUserData? UserData = default)
    : AseCel(Layer, Position, Opacity, UserData)
{
    /// <summary>
    ///     The width, in tiles, of this <see cref="AseTilemapCel"/>.
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    ///     The height, in tiles, of this <see cref="AseTilemapCel"/>.
    /// </summary>
    public int Height => Size.Height;

    /// <summary>
    ///     The width, in pixels, of each <see cref="AseTile"/> element in this
    ///     <see cref="AseTilemapCel"/>.
    /// </summary>
    public int TileWidth => TileSize.Width;

    /// <summary>
    ///     The height, in pixels, of each <see cref="AseTile"/> element in this
    ///     <see cref="AseTilemapCel"/>.
    /// </summary>
    public int TileHeight => TileSize.Height;

    /// <summary>
    ///     The total number of <see cref="AseTile"/> elements in this
    ///     <see cref="AseTilemapCel"/>.
    /// </summary>
    public int TileCount => Tiles.Count;

    /// <summary>
    ///     Returns the <see cref="AseTile"/> element at the specified 
    ///     <paramref name="index"/> from this <see cref="AseTilemapCel"/>.
    /// </summary>
    /// <param name="index">
    ///     The index of the <see cref="AseTile"/> element within this
    ///     <see cref="AseTilemapCel"/> to return.
    /// </param>
    /// <returns>
    ///     The <see cref="AseTile"/> element at the specified 
    ///     <paramref name="index"/> from this <see cref="AseTilemapCel"/>/
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the specified <paramref name="index"/> is less than zero
    ///     or is greater than or equal to <see cref="TileCount"/>.
    /// </exception>
    public AseTile this[int index]
    {
        get
        {
            if (index < 0 || index >= TileCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Tiles[index];
        }
    }
}