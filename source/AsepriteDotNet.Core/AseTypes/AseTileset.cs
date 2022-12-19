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
///     Represents a tileset in an Aseprite image.
/// </summary>
/// <param name="ID">
///     The ID of this <see cref="AseTileset"/>.
/// </param>
/// <param name="TileCount">
///     The total number of tiles in this <see cref="AseTileset"/>.
/// </param>
/// <param name="TileSize">
///     The width and height extents of each tile in this 
///     <see cref="AseTileset"/>.
/// </param>
/// <param name="Name">
///     The name of this <see cref="AseTileset"/>.
/// </param>
/// <param name="Pixels">
///     An <see cref="Array"/> of <see cref="Rgba32"/> color elements that
///     represent the pixel data for this <see cref="AseTileset"/>.  Pixel data
///     is in order from top-to-bottom, read left-to-right.
/// </param>
public record AseTileset(int ID, int TileCount, Size TileSize, string Name, Rgba32[] Pixels)
{
    /// <summary>
    ///     The width of each tile in this <see cref="AseTileset"/>.
    /// </summary>
    public int TileWidth => TileSize.Width;

    /// <summary>
    ///     The height of each tile in this <see cref="AseTileset"/>.
    /// </summary>
    public int TileHeight => TileSize.Height;

    /// <summary>
    ///     Exports this <see cref="AseFrame"/> to disk as a PNG (.png) file.
    /// </summary>
    /// <param name="path">
    ///     The absolute path and filename, including extension, of the PNG
    ///     (.png) file that will be created.  If the file already exists, it
    ///     will be overwritten.
    /// </param>
    public void ExportAsPng(string path)
    {
        //  Tilesets are written to disk in Aseprite as a vertical strip.
        //  Need to decide if it should be converted to square packed
        Size size = new(TileWidth, TileHeight * TileCount);
        PngWriter.SaveTo(path, size, Pixels);
    }

}