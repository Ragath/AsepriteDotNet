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

/// <summary>
///     Represents a tilesheet.
/// </summary>
/// <param name="Id">
///     The ID of this <see cref="Tilesheet"/>.
/// </param>
/// <param name="Name">
///     The name of this <see cref="Tilesheet"/>.
/// </param>
/// <param name="Size">
///     The width and height extent of this <see cref="Tilesheet"/>.
/// </param>
/// <param name="TileSize">
///     The width and height extent of each <see cref="Tile"/> element in this
///     <see cref="Tilesheet"/>.
/// </param>
/// <param name="Pixels">
///     The pixel color data that represents the image of this 
///     <see cref="Tilesheet"/>.
/// </param>
/// <param name="Tiles">
///     The collection of <see cref="Tile"/> elements in this
///     <see cref="Tilesheet"/>.
/// </param>
public sealed record Tilesheet(int Id, string Name, Size Size, Size TileSize, Rgba32[] Pixels, List<Tile> Tiles)
{

    /// <summary>
    ///     Gets the width of this <see cref="Tilesheet"/>.
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    ///     Gets the height of this <see cref="Tilesheet"/>.
    /// </summary>
    public int Height => Size.Height;

    /// <summary>
    ///     Gets the width of a <see cref="Tile"/> element in this
    ///     <see cref="Tilesheet"/>.
    /// </summary>
    public int TileWidth => TileSize.Width;

    /// <summary>
    ///     Gets the height of a <see cref="Tile"/> element in this
    ///     <see cref="Tilesheet"/>.
    /// </summary>
    public int TileHeight => TileSize.Height;

    /// <summary>
    ///     Gets the total number of <see cref="Tile"/> elements in this
    ///     <see cref="Tilesheet"/>.
    /// </summary>
    public int TileCount => Tiles.Count;

    /// <summary>
    ///     Returns the <see cref="Tile"/> element with the specified 
    ///     <paramref name="tileID"/> from this <see cref="Tilesheet"/>.
    /// </summary>
    /// <param name="tileID">
    ///     The ID of the <see cref="Tile"/> element to return.
    /// </param>
    /// <returns>
    ///     The <see cref="Tile"/> element with the specified 
    ///     <paramref name="tileID"/> from this <see cref="Tilesheet"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the specified <paramref name="tileID"/> is less than zero
    ///     or is greater than or equal to the <see cref="TileCount"/>.
    /// </exception>
    public Tile this[int tileID]
    {
        get
        {
            if (tileID < 0 || tileID >= TileCount)
            {
                throw new ArgumentOutOfRangeException(nameof(tileID));
            }

            return Tiles[tileID];
        }
    }

    /// <summary>
    ///     Returns the pixel data of the <see cref="Tile"/> element with the
    ///     specified <paramref name="tileID"/> from this 
    ///     <see cref="Tilesheet"/>.
    /// </summary>
    /// <param name="tileID">
    ///     The ID of the <see cref="Tile"/> element.
    /// </param>
    /// <returns>
    ///     THe pixel data of the <see cref="Tile"/> element with the 
    ///     specified <paramref name="tileID"/> from this
    ///     <see cref="Tilesheet"/>.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public Rgba32[] GetTilePixels(int tileID)
    {
        throw new NotImplementedException();
    }
}