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
using AsepriteDotNet.ImageInternal;
using AsepriteDotNet.IO.Image;
using AsepriteDotNet.Primitives;

namespace AsepriteDotNet.AsepriteTypes;

/// <summary>
///     Represents a tileset in an Aseprite image.
/// </summary>
public class AsepriteTileset
{
    /// <summary>
    ///     Gets the ID of this <see cref="AsepriteTileset"/>.
    /// </summary>
    public int ID { get; }

    /// <summary>
    ///     Gets the total number of tiles in this ,<see cref="AsepriteTileset"/>.
    /// </summary>
    public int TileCount { get; }

    /// <summary>
    ///     Gets the width and height, in pixels of each tile in this
    ///     <see cref="AsepriteTileset"/>.
    /// </summary>
    public Size TileSize { get; }

    /// <summary>
    ///     Gets the name of this <see cref="AsepriteTileset"/>.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets or Sets an <see cref="Array"/> of <see cref="Rgba32"/> elements 
    ///     that represents the raw pixel data for this <see cref="AsepriteTileset"/>.
    /// </summary>
    public Rgba32[] Pixels { get; }

    internal AsepriteTileset(int id, int count, Size tileSize, string name, Rgba32[] pixels) =>
        (ID, TileCount, TileSize, Name, Pixels) = (id, count, tileSize, name, pixels);

    /// <summary>
    ///     Generates a new <see cref="Tilesheet"/> class instance from this
    ///     <see cref="AsepriteTileset"/>.
    /// </summary>
    /// <param name="options">
    ///     The options to adhere to when generating the tilesheet.
    /// </param>
    /// <returns>
    ///     The <see cref="Tilesheet"/> that is created by this method.
    /// </returns>
    public Tilesheet ToTilesheet(bool mergeDuplicates = true,
                                 int borderPadding = 0,
                                 int spacing = 0,
                                 int innerPadding = 0)
    {
        List<TilesheetTile> sheetTiles = new();
        List<Rgba32[]> tileColorLookup = SplitTiles();
        Dictionary<int, int> tileDuplicateMap = new();

        int totalTiles = tileColorLookup.Count;

        if (mergeDuplicates)
        {
            for (int i = 0; i < tileColorLookup.Count; i++)
            {
                for (int d = 0; d < i; d++)
                {
                    if (tileColorLookup[i].SequenceEqual(tileColorLookup[d]))
                    {
                        tileDuplicateMap.Add(i, d);
                        break;
                    }
                }
            }
            //  Since we are merging duplicates, we need to subtract the number of
            //  duplicates from the total tiles
            totalTiles -= tileDuplicateMap.Count;
        }

        //  Determine the number of columns and rows needed to pack the tiles
        //  into the tilesheet
        double sqrt = Math.Sqrt(totalTiles);
        int columns = (int)Math.Floor(sqrt);
        if (Math.Abs(sqrt % 1) >= double.Epsilon)
        {
            columns++;
        }

        int rows = totalTiles / columns;
        if (totalTiles % columns != 0)
        {
            rows++;
        }


        //  Determine the final width and height of the tile sheet based on the
        //  number of columns and rows and adjusting for padding and spacing
        int width = (columns * TileSize.Width) +
                    (borderPadding * 2) +
                    (spacing * (columns - 1)) +
                    (innerPadding * 2 * columns);

        int height = (rows * TileSize.Height) +
                     (borderPadding * 2) +
                     (spacing * (rows - 1)) +
                     (innerPadding * 2 * rows);

        Size sheetSize = new(width, height);

        Rgba32[] sheetPixels = new Rgba32[width * height];

        Dictionary<int, TilesheetTile> originalToDuplicateTileLookup = new();

        int tOffset = 0;

        for (int tileNum = 0; tileNum < TileCount; tileNum++)
        {
            if (!mergeDuplicates || !tileDuplicateMap.ContainsKey(tileNum))
            {
                //  Calculate the x and y position of the tile's top-left pixel
                //  relative to the top-left of the file tilesheet
                int tileCol = (tileNum - tOffset) % columns;
                int tileRow = (tileNum - tOffset) / columns;

                //  Inject the pixel color data from the tile into the final
                //  tilesheet color data array
                Rgba32[] tilePixels = tileColorLookup[tileNum];

                for (int pixelNum = 0; pixelNum < tilePixels.Length; pixelNum++)
                {
                    int x = (pixelNum % TileSize.Width) + (tileCol * TileSize.Width);
                    int y = (pixelNum / TileSize.Width) + (tileRow * TileSize.Height);

                    //  Adjust for padding/spacing
                    x += borderPadding;
                    y += borderPadding;

                    if (spacing > 0)
                    {
                        if (tileCol > 0)
                        {
                            x += spacing * tileCol;
                        }

                        if (tileRow > 0)
                        {
                            y += spacing * tileRow;
                        }
                    }

                    if (innerPadding > 0)
                    {
                        x += innerPadding * (tileCol + 1);
                        y += innerPadding * (tileRow + 1);

                        if (tileCol > 0)
                        {
                            x += innerPadding * tileCol;
                        }

                        if (tileRow > 0)
                        {
                            y += innerPadding * tileRow;
                        }
                    }

                    int index = y * width + x;
                    sheetPixels[index] = tilePixels[pixelNum];
                }

                //  Now create the tile source rectangle data
                Rectangle sourceRectangle = new(0, 0, TileSize.Width, TileSize.Height);
                sourceRectangle.X += borderPadding;
                sourceRectangle.Y += borderPadding;

                if (spacing > 0)
                {
                    if (tileCol > 0)
                    {
                        sourceRectangle.X += spacing * tileCol;
                    }

                    if (tileRow > 0)
                    {
                        sourceRectangle.Y += spacing * tileRow;
                    }
                }

                if (innerPadding > 0)
                {
                    sourceRectangle.X += innerPadding * (tileCol + 1);
                    sourceRectangle.Y += innerPadding * (tileRow + 1);

                    if (tileCol > 0)
                    {
                        sourceRectangle.X += innerPadding * tileCol;
                    }

                    if (tileRow > 0)
                    {
                        sourceRectangle.Y += innerPadding * tileRow;
                    }
                }

                TilesheetTile tile = new(sourceRectangle);

                sheetTiles.Add(tile);
                originalToDuplicateTileLookup.Add(tileNum, tile);
            }
            else
            {
                //  We are merging duplicates and it was detected that the
                //  current tile to process is a duplicate.  So we still need to
                //  add the tile, but we need to make sure the tile source
                //  rectangle is the same as the tile it's a duplicate of.
                TilesheetTile original = originalToDuplicateTileLookup[tileDuplicateMap[tileNum]];
                sheetTiles.Add(new TilesheetTile(original.SourceRectangle));
                tOffset++;
            }
        }

        return new Tilesheet(Name, sheetSize, sheetTiles, sheetPixels);
    }


    /// <summary>
    ///     Writes the pixel data for this <see cref="AsepriteImageCel"/> to disk as a 
    ///     .png file.
    /// </summary>
    /// <param name="path">
    ///     The absolute file path to save the generated .png file to.
    /// </param>
    /// <param name="method">
    ///     The packing method to use when creating the tileset image.
    /// </param>
    public void ToPng(string path)
    {


        List<Rgba32[]> tiles = SplitTiles();

        int columns, rows;
        int width, height;

        //  Determine the number of columns and rows needed to pack the tiles
        //  into the tilesheet
        double sqrt = Math.Sqrt(TileCount);
        columns = (int)Math.Floor(sqrt);
        if (Math.Abs(sqrt % 1) >= double.Epsilon)
        {
            columns++;
        }

        rows = TileCount / columns;
        if (TileCount % columns != 0)
        {
            rows++;
        }

        //  Determine the final width and height of the tile sheet based on the
        //  number of columns and rows and adjusting for padding and spacing
        width = columns * TileSize.Width;
        height = rows * TileSize.Height;

        Rgba32[] pixels = new Rgba32[width * height];

        for (int tileNum = 0; tileNum < TileCount; tileNum++)
        {
            int tileCol = tileNum % columns;
            int tileRow = tileNum / columns;
            Rgba32[] tilePixels = tiles[tileNum];

            for (int pixelNum = 0; pixelNum < tilePixels.Length; pixelNum++)
            {
                int x = (pixelNum % TileSize.Width) + (tileCol * TileSize.Width);
                int y = (pixelNum / TileSize.Width) + (tileRow * TileSize.Height);
                int index = y * width + x;
                pixels[index] = tilePixels[pixelNum];
            }
        }

        PngWriter.SaveTo(path, new Size(width, height), pixels);

    }

    internal List<Rgba32[]> SplitTiles()
    {
        List<Rgba32[]> tiles = new();

        int tileLen = TileSize.Width * TileSize.Height;

        for (int i = 0; i < TileCount; i++)
        {
            tiles.Add(Pixels[(i * tileLen)..((i * tileLen) + tileLen)]);
        }

        return tiles;
    }

    public Image Export(bool mergeDuplicates = true,
                        int borderPadding = 0,
                        int spacing = 0,
                        int innerPadding = 0)
    {
        List<Rgba32[]> tilesPixels = SplitTiles();
        List<Image> tiles = new();

        for (int tNum = 0; tNum < tilesPixels.Count; tNum++)
        {
            Image tImage = new(TileSize, tilesPixels[tNum], new List<Rectangle>() { new(0, 0, TileSize.Width, TileSize.Height) });
            tiles.Add(tImage);
        }

        return Image.Pack(TileSize, tiles, mergeDuplicates, borderPadding, spacing, innerPadding);
    }
}