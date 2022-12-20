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
using System.Diagnostics;

using AsepriteDotNet.Core.Compression;
using AsepriteDotNet.Core.Color;
using AsepriteDotNet.Core.Primitives;
using AsepriteDotNet.Core.AseTypes;
using System.IO.Compression;

namespace AsepriteDotNet.Core.IO;

/// <summary>
///     Utility class for reading an Aseprite file from disk.
/// </summary>
public static class AsepriteFileReader
{
    private const ushort ASE_HEADER_MAGIC = 0xA5E0;                 //  File Header Magic Number
    private const int ASE_HEADER_SIZE = 128;                        //  File Header Length, In Bytes
    private const uint ASE_HEADER_FLAG_LAYER_OPACITY_VALID = 1;     //  Header Flag (Is Layer Opacity Valid)

    private const ushort ASE_FRAME_MAGIC = 0xF1FA;                  //  Frame Magic Number

    private const ushort ASE_CHUNK_OLD_PALETTE1 = 0x0004;           //  Old Palette Chunk
    private const ushort ASE_CHUNK_OLD_PALETTE2 = 0x0011;           //  Old Palette Chunk
    private const ushort ASE_CHUNK_LAYER = 0x2004;                  //  Layer Chunk
    private const ushort ASE_CHUNK_CEL = 0x2005;                    //  Cel Chunk
    private const ushort ASE_CHUNK_CEL_EXTRA = 0x2006;              //  Cel Extra Chunk
    private const ushort ASE_CHUNK_COLOR_PROFILE = 0x2007;          //  Color Profile Chunk
    private const ushort ASE_CHUNK_EXTERNAL_FILES = 0x2008;         //  External Files Chunk
    private const ushort ASE_CHUNK_MASK = 0x2016;                   //  Mask Chunk (deprecated)
    private const ushort ASE_CHUNK_PATH = 0x2017;                   //  Path Chunk (never used)
    private const ushort ASE_CHUNK_TAGS = 0x2018;                   //  Tags Chunk
    private const ushort ASE_CHUNK_PALETTE = 0x2019;                //  Palette Chunk
    private const ushort ASE_CHUNK_USER_DATA = 0x2020;              //  User Data Chunk
    private const ushort ASE_CHUNK_SLICE = 0x2022;                  //  Slice Chunk
    private const ushort ASE_CHUNK_TILESET = 0x2023;                //  Tileset Chunk

    private const ushort ASE_LAYER_TYPE_NORMAL = 0;                 //  Layer Type Normal (Image) Layer
    private const ushort ASE_LAYER_TYPE_GROUP = 1;                  //  Layer Type Group
    private const ushort ASE_LAYER_TYPE_TILEMAP = 2;                //  Layer Type Tilemap

    private const ushort ASE_LAYER_FLAG_VISIBLE = 1;                //  Layer Flag (Is Visible)
    private const ushort ASE_LAYER_FLAG_EDITABLE = 2;               //  Layer Flag (Is Editable)
    private const ushort ASE_LAYER_FLAG_LOCKED = 4;                 //  Layer Flag  (Movement Locked)
    private const ushort ASE_LAYER_FLAG_BACKGROUND = 8;             //  Layer Flag (Is Background Layer)
    private const ushort ASE_LAYER_FLAG_PREFERS_LINKED = 16;        //  Layer Flag (Prefers Linked Cels)
    private const ushort ASE_LAYER_FLAG_COLLAPSED = 32;             //  Layer Flag (Displayed Collapsed)
    private const ushort ASE_LAYER_FLAG_REFERENCE = 64;             //  Layer Flag (Is Reference Layer)

    private const ushort ASE_CEL_TYPE_RAW_IMAGE = 0;                //  Cel Type (Raw Image)
    private const ushort ASE_CEL_TYPE_LINKED = 1;                   //  Cel Type (Linked)
    private const ushort ASE_CEL_TYPE_COMPRESSED_IMAGE = 2;         //  Cel Type (Compressed Image)
    private const ushort ASE_CEL_TYPE_COMPRESSED_TILEMAP = 3;       //  Cel Type (Compressed Tilemap)

    private const uint ASE_CEL_EXTRA_FLAG_PRECISE_BOUNDS_SET = 1;   //  Cel Extra Flag (Precise Bounds Set)

    private const ushort ASE_PALETTE_FLAG_HAS_NAME = 1;             //  Palette Flag (Color Has Name)

    private const uint ASE_USER_DATA_FLAG_HAS_TEXT = 1;             //  User Data Flag (Has Text)
    private const uint ASE_USER_DATA_FLAG_HAS_COLOR = 2;            //  User Data Flag (Has Color)

    private const uint ASE_SLICE_FLAGS_IS_NINE_PATCH = 1;           //  Slice Flag (Is 9-Patch Slice)
    private const uint ASE_SLICE_FLAGS_HAS_PIVOT = 2;               //  Slice Flag (Has Pivot Information)

    private const uint ASE_TILESET_FLAG_EXTERNAL_FILE = 1;          //  Tileset Flag (Includes Link To External File)
    private const uint ASE_TILESET_FLAG_EMBEDDED = 2;               //  Tileset Flag (Includes Tiles Inside File)

    private const byte TILE_ID_SHIFT = 0;                           //  Tile ID Bitmask Shift
    private const uint TILE_ID_MASK = 0x1fffffff;                   //  Tile ID Bitmask
    private const uint TILE_FLIP_X_MASK = 0x20000000;               //  Tile Flip X Bitmask
    private const uint TILE_FLIP_Y_MASK = 0x40000000;               //  Tile Flip Y Bitmask
    private const uint TILE_90CW_ROTATION_MASK = 0x80000000;        //  Tile 90CW Rotation Bitmask

    internal static AsepriteFile ReadFile(string path)
    {
        using Stream stream = File.OpenRead(path);
        return ReadStream(stream);
    }

    private static AsepriteFile ReadStream(Stream stream)
    {
        //  -------------------------------------------------------------------
        //  Read the file header
        //  -------------------------------------------------------------------
        _ = stream.ReadDword();                 //  File size (don't need, ignored)
        ushort headerMagic = stream.ReadWord(); //  Header magic (0xA5E0)

        if (headerMagic != ASE_HEADER_MAGIC)
        {
            stream.Dispose();
            throw new InvalidOperationException($"Invalid header magic number (0x{headerMagic:X4}). This does not appear to be a valid Aseprite file");
        }

        ushort frameCount = stream.ReadWord();  //  Total number of frames
        ushort frameWidth = stream.ReadWord();  //  Frame width, in pixels
        ushort frameHeight = stream.ReadWord(); //  Frame height, in pixels

        if (frameWidth < 1 || frameHeight < 1)
        {
            stream.Dispose();
            throw new InvalidOperationException($"Invalid canvas size {frameWidth}x{frameHeight}.");
        }

        Size frameSize = new(frameWidth, frameHeight);

        ushort colorDepth = stream.ReadWord();  //  Color depth (bits per pixel)

        if (colorDepth != 32 && colorDepth != 16 && colorDepth != 8)
        {
            stream.Dispose();
            throw new InvalidOperationException($"Invalid color depth: {colorDepth}");
        }

        uint headerFlags = stream.ReadDword();   //  Header flags

        bool isLayerOpacityValid = HasFlag(headerFlags, ASE_HEADER_FLAG_LAYER_OPACITY_VALID);

        _ = stream.ReadWord();                          //  Speed (ms between frame) (deprecated, ignored)
        _ = stream.ReadDword();                         //  Set to 0 (ignored)
        _ = stream.ReadDword();                         //  Set to 0 (ignored)
        byte transparentIndex = stream.ReadByteEx();    //  Index of transparent color in palette

        if (colorDepth != 8)
        {
            //  Transparent index can only be non-zero on Indexed color depth mode.
            transparentIndex = 0;
        }

        _ = stream.ReadBytes(3);                //  Ignore these bytes               
        ushort colorCount = stream.ReadWord();  //  Number of colors (0 means 256 for old sprites)

        if (colorCount == 0)
        {
            colorCount = 256;
        }

        //  Don't need rest of header, but for documentation, these values are
        //  skipped:
        //  pixel width             (byte)
        //  pixel height            (byte)
        //  Grid x-position         (short)
        //  Grid y-position         (short)
        //  Grid width              (ushort)
        //  Grid heigh              (ushort)
        //  Future use (set to 0)   (byte[84])
        stream.Position = ASE_HEADER_SIZE;

        AsePalette palette = new(transparentIndex);
        List<AseTileset> tilesets = new();
        List<AseLayer> layers = new();
        List<AseFrame> frames = new();
        List<AseTag> tags = new();
        List<AseSlice> slices = new();

        //  Reference ot the last group layer that is read so that subsequent
        //  child layers can be added to it.
        AseGroupLayer? lastGroupLayer = default;

        //  Read frame-by-frame until all frames are read.
        for (int fNum = 0; fNum < frameCount; fNum++)
        {
            List<AseCel> cels = new();

            //  ---------------------------------------------------------------
            //  Read Frame Header
            //  ---------------------------------------------------------------
            _ = stream.ReadDword();                 //  Frame size in bytes (don't need, ignored)
            ushort frameMagic = stream.ReadWord();  //  Frame magic number (0xF1FA)

            if (frameMagic != ASE_FRAME_MAGIC)
            {
                stream.Dispose();
                throw new InvalidOperationException($"Invalid frame magic number (0x{frameMagic:X4}) in frame {fNum}.");
            }

            int chunkCount = stream.ReadWord();        //  Old field that specifies number of chunks
            ushort duration = stream.ReadWord();    //  Frame duration (ms)
            _ = stream.ReadBytes(2);                //  For future (set to 0) (ignored)
            uint moreChunks = stream.ReadDword();   //  New field that specifies number of chunks

            //  Determine actual chunk count
            if (chunkCount == 0xFFFF && chunkCount < moreChunks)
            {
                chunkCount = (int)moreChunks;
            }

            //  Read chunk-by-chunk until all chunks are read
            for (int cNum = 0; cNum < chunkCount; cNum++)
            {
                long chunkStart = stream.Position;

                //  -----------------------------------------------------------
                //  Read chunk header
                //  -----------------------------------------------------------
                uint chunkSize = stream.ReadDword();    //  Chunk size, in bytes
                ushort chunkType = stream.ReadWord();   //  Chunk type

                long chunKEnd = chunkStart + chunkSize;

                if (chunkType == ASE_CHUNK_LAYER)
                {
                    //  -------------------------------------------------------
                    //  Read Layer chunk
                    //  -------------------------------------------------------
                    ushort layerFlags = stream.ReadWord();      //  Layer flags
                    ushort layerType = stream.ReadWord();       //  Layer Type
                    ushort childLevel = stream.ReadWord();      //  Child level relative to last layer read
                    _ = stream.ReadWord();                      //  Default layer width (ignored)
                    _ = stream.ReadWord();                      //  Default layer height (ignored)
                    ushort blendMode = stream.ReadWord();       //  Blend mode
                    byte layerOpacity = stream.ReadByteEx();    //  Layer opacity
                    _ = stream.ReadBytes(3);                    //  For future (set to 0) (ignored)
                    string layerName = stream.ReadString();     //  Layer name


                    if (!isLayerOpacityValid)
                    {
                        layerOpacity = 255;
                    }

                    bool isVisible = HasFlag(layerFlags, ASE_LAYER_FLAG_VISIBLE);
                    bool isBackground = HasFlag(layerFlags, ASE_LAYER_FLAG_BACKGROUND);
                    bool isReference = HasFlag(layerFlags, ASE_LAYER_FLAG_REFERENCE);
                    BlendMode mode = (BlendMode)blendMode;

                    AseLayer layer = new(isVisible, isBackground, isReference, childLevel, mode, layerOpacity, layerName);

                    if (layerType == ASE_LAYER_TYPE_NORMAL)
                    {
                        layer = new AseImageLayer(isVisible, isBackground, isReference, childLevel, mode, layerOpacity, layerName);
                    }
                    else if (layerType == ASE_LAYER_TYPE_GROUP)
                    {
                        layer = new AseGroupLayer(isVisible, isBackground, isReference, childLevel, mode, layerOpacity, layerName);
                    }
                    else if (layerType == ASE_LAYER_TYPE_TILEMAP)
                    {
                        uint tilesetIndex = stream.ReadDword(); //  Index of tileset used by tilemap layer
                        AseTileset tileset = tilesets[(int)tilesetIndex];
                        layer = new AseTilemapLayer(tileset, isVisible, isBackground, isReference, childLevel, mode, layerOpacity, layerName);
                    }
                    else
                    {
                        stream.Dispose();
                        throw new InvalidOperationException($"Unknown layer type '{layerType}'");
                    }

                    if (TryGetUserData(stream, out string? text, out Rgba32? color))
                    {
                        layer = layer with { UserData = new(text, color) };
                        cNum++;
                    }

                    if (childLevel != 0 && lastGroupLayer is not null)
                    {
                        lastGroupLayer.Children.Add(layer);
                    }

                    if (layer is AseGroupLayer groupLayer)
                    {
                        lastGroupLayer = groupLayer;
                    }

                    layers.Add(layer);
                }
                else if (chunkType == ASE_CHUNK_CEL)
                {
                    //  -------------------------------------------------------
                    //  Read Cel chunk
                    //  -------------------------------------------------------
                    ushort layerIndex = stream.ReadWord();  //  Index of layer the cel is on
                    short xPosition = stream.ReadShort();   //  Cel x-position relative to frame bounds
                    short yPosition = stream.ReadShort();   //  Cel y-position relative to frame bounds
                    byte celOpacity = stream.ReadByteEx();  //  Cel opacity level
                    ushort celType = stream.ReadWord();     //  Type of cel
                    _ = stream.ReadBytes(7);                //  For future (set to 0) (ignored)

                    Point celPosition = new(xPosition, yPosition);
                    AseLayer celLayer = layers[layerIndex];
                    AseCel cel;

                    if (celType == ASE_CEL_TYPE_RAW_IMAGE || celType == ASE_CEL_TYPE_COMPRESSED_IMAGE)
                    {
                        ushort celWidth = stream.ReadWord();    //  Width of cel, in pixels
                        ushort celHeight = stream.ReadWord();   //  Height of cel, in pixels
                        byte[] data = stream.ReadTo(chunKEnd);      //  Cel pixel data

                        if (celType == ASE_CEL_TYPE_COMPRESSED_IMAGE)
                        {
                            data = Decompress(data);
                        }

                        Size celSize = new(celWidth, celHeight);
                        Rgba32[] celPixels = PixelsToColor(data, colorDepth, palette);

                        cel = new AseImageCel(celSize, celPixels, celLayer, celPosition, celOpacity);
                    }
                    else if (celType == ASE_CEL_TYPE_LINKED)
                    {
                        ushort frameIndex = stream.ReadWord();  //  Frame position to link with

                        AseCel other = frames[frameIndex].Cels[cels.Count];
                        cel = other;
                    }
                    else if (celType == ASE_CEL_TYPE_COMPRESSED_TILEMAP)
                    {
                        ushort celWidth = stream.ReadWord();        //  Cel width, in tiles
                        ushort celHeight = stream.ReadWord();       //  Cel height, in tiles
                        ushort bitsPerTile = stream.ReadWord();     //  Bit per tile (always 32 at the moment)
                        uint tileIdBitmask = stream.ReadDword();    //  Bitmask for Tile ID (e.g. 0x1fffffff for 32-bit)
                        uint xFlipBitmask = stream.ReadDword();     //  Bitmask for X Flip
                        uint yFlipBitmask = stream.ReadDword();     //  Bitmask for Y Flip
                        uint rotationBitmask = stream.ReadDword();  //  Bitmask for 90CW Rotation
                        _ = stream.ReadBytes(10);                   //  Reserved (ignored)
                        byte[] data = stream.ReadTo(chunKEnd);      //  Compressed tile data

                        data = Decompress(data);
                        Size celSize = new(celWidth, celHeight);

                        //  Per Aseprite file spec, the "bits" per tile is, at
                        //  the moment, always 32-bits.  This means it's 4-bytes
                        //  per tile (32 / 8 = 4).  Meaning that each tile value
                        //  is a uint (DWORD)
                        int bytesPerTile = 4;
                        int tileCount = data.Length / bytesPerTile;
                        List<AseTile> tiles = new(tileCount);

                        for (int i = 0, b = 0; i < tileCount; i++, b += bytesPerTile)
                        {
                            byte[] dword = data[b..(b + bytesPerTile)];
                            uint value = BitConverter.ToUInt32(dword);
                            uint tileId = (value & TILE_ID_MASK) >> TILE_ID_SHIFT;
                            uint xFlip = (value & TILE_FLIP_X_MASK);
                            uint yFlip = (value & TILE_FLIP_Y_MASK);
                            uint rotate = (value & TILE_90CW_ROTATION_MASK);

                            AseTile tile = new(tileId, xFlip, yFlip, rotate);
                            tiles.Add(tile);
                        }

                        Debug.Assert(tiles.Count == data.Length / bytesPerTile, $"Tile Count '{tiles.Count}' does not equal '{data.Length / bytesPerTile}'");

                        Size tileSize = ((AseTilemapLayer)celLayer).Tileset.TileSize;

                        cel = new AseTilemapCel(celSize, tileSize, tiles, celLayer, celPosition, celOpacity);
                    }
                    else
                    {
                        stream.Dispose();
                        throw new InvalidOperationException($"Unknown cel type '{celType}'");
                    }

                    if (TryGetUserData(stream, out string? text, out Rgba32? color))
                    {
                        cel = cel with { UserData = new(text, color) };
                    }

                    cels.Add(cel);
                }
                else if (chunkType == ASE_CHUNK_TAGS)
                {
                    //  -------------------------------------------------------
                    //  Read Tag chunk
                    //  -------------------------------------------------------
                    ushort tagCount = stream.ReadWord();    //  Total number of tags
                    _ = stream.ReadBytes(8);                //  For future (set to zero) (ignored)

                    for (int tNum = 0; tNum < tagCount; tNum++)
                    {
                        ushort fromFrame = stream.ReadWord();       //  From frame
                        ushort toFrame = stream.ReadWord();         //  To frame
                        byte direction = stream.ReadByteEx();       //  Loop animation direction
                        _ = stream.ReadBytes(8);                    //  For future (set to 0) (ignored)
                        byte r = stream.ReadByteEx();               //  RGB Red value (deprecated in 1.3)
                        byte g = stream.ReadByteEx();               //  RGB Green value (deprecated in 1.3)
                        byte b = stream.ReadByteEx();               //  RGB Blue value (deprecated in 1.3)
                        _ = stream.ReadByteEx();                    //  Extra byte (zero) (ignored)
                        string tagName = stream.ReadString();       //  Tag name

                        LoopDirection loopDirection = (LoopDirection)direction;
                        Rgba32 tagColor = Rgba32.FromRGBA(r, g, b, 255);

                        AseTag tag = new(fromFrame, toFrame, loopDirection, tagColor, tagName);

                        tags.Add(tag);
                    }

                    //  Tags are a special case, user data for tags 
                    //  comes all together (one next to the other) after 
                    //  the tags chunk, in the same order:
                    //
                    //  * TAGS CHUNK (TAG1, TAG2, ..., TAGn)
                    //  * USER DATA CHUNK FOR TAG1
                    //  * USER DATA CHUNK FOR TAG2
                    //  * ...
                    //  * USER DATA CHUNK FOR TAGn
                    //
                    //  So here we expect that the next user data chunk 
                    //  will correspond to the next tag in the tags 
                    //  collection
                    int tagIterator = 0;
                    while (TryGetUserData(stream, out string? uText, out Rgba32? uColor))
                    {
                        tags[tagIterator] = tags[tagIterator] with { UserData = new(uText, uColor) };
                        tagIterator++;
                        cNum++;
                    }
                }
                else if (chunkType == ASE_CHUNK_PALETTE)
                {
                    //  -------------------------------------------------------
                    //  Read Palette chunk
                    //  -------------------------------------------------------
                    uint newSize = stream.ReadDword();  //  New palette size (total number of entries)
                    uint from = stream.ReadDword();     //  First color index to change
                    uint to = stream.ReadDword();       //  Last color index to change
                    _ = stream.ReadBytes(8);            //  For future (set to zero) (ignored)

                    //  Resize current palette if needed
                    if (newSize > 0)
                    {
                        palette.Resize((int)newSize);
                    }

                    for (uint i = from; i <= to; i++)
                    {
                        ushort flags = stream.ReadWord();   //  Entry flags
                        byte r = stream.ReadByteEx();       //  RGBA Red value (0 - 255)
                        byte g = stream.ReadByteEx();       //  RGBA Green value (0 - 255)
                        byte b = stream.ReadByteEx();       //  RGBA Blue value (0 - 255)
                        byte a = stream.ReadByteEx();       //  RGBA Alpha value (0 - 255)

                        if (HasFlag(flags, ASE_PALETTE_FLAG_HAS_NAME))
                        {
                            _ = stream.ReadString();        //  Color name (ignored)
                        }

                        palette[(int)i] = Rgba32.FromRGBA(r, g, b, a);
                    }
                }
                else if (chunkType == ASE_CHUNK_SLICE)
                {
                    //  -------------------------------------------------------
                    //  Read Slice chunk
                    //  -------------------------------------------------------
                    uint keyCount = stream.ReadDword();     //  Number of slice "keys"
                    uint sliceFlags = stream.ReadDword();   //  Slice flags
                    _ = stream.ReadDword();                 //  Reserved (ignored)
                    string sliceName = stream.ReadString(); //  Slice name

                    bool isNinePatch = HasFlag(sliceFlags, ASE_SLICE_FLAGS_IS_NINE_PATCH);
                    bool hasPivot = HasFlag(sliceFlags, ASE_SLICE_FLAGS_HAS_PIVOT);

                    List<AseSliceKey> keys = new();

                    for (uint i = 0; i < keyCount; i++)
                    {
                        uint keyStart = stream.ReadDword();     //  Frame index this key is valid for starting on
                        int keyXOrigin = stream.ReadLong();     //  Slice X origin      
                        int keyYOrigin = stream.ReadLong();     //  Slice Y origin
                        uint keyWidth = stream.ReadDword();     //  Slice width
                        uint keyHeight = stream.ReadDword();    //  Slice height

                        Rectangle bounds = new(keyXOrigin, keyYOrigin, (int)keyWidth, (int)keyHeight);
                        Rectangle? center = default;
                        Point? pivot = default;

                        if (isNinePatch)
                        {
                            int centerX = stream.ReadLong();        //  Center X position
                            int centerY = stream.ReadLong();        //  Center Y Position
                            uint centerWidth = stream.ReadDword();  //  Center width
                            uint centerHeight = stream.ReadDword(); //  Center height

                            center = new(centerX, centerY, (int)centerWidth, (int)centerHeight);
                        }

                        if (hasPivot)
                        {
                            int pivotX = stream.ReadLong();     //  Pivot X Position
                            int pivotY = stream.ReadLong();     //  Pivot Y Position

                            pivot = new(pivotX, pivotY);
                        }

                        AseSliceKey key = new((int)keyStart, bounds, center, pivot);
                        keys.Add(key);
                    }

                    AseSlice slice = new(isNinePatch, hasPivot, sliceName, keys);

                    if (TryGetUserData(stream, out string? uText, out Rgba32? uColor))
                    {
                        slice = slice with { UserData = new(uText, uColor) };
                        cNum++;
                    }

                    slices.Add(slice);
                }
                else if (chunkType == ASE_CHUNK_TILESET)
                {
                    //  -------------------------------------------------------
                    //  Read Tileset chunk
                    //  -------------------------------------------------------
                    uint tilesetId = stream.ReadDword();        //  Tileset ID
                    uint tilesetFlags = stream.ReadDword();     //  Tileset Flags
                    uint tileCount = stream.ReadDword();        //  Number of Tiles
                    ushort tileWidth = stream.ReadWord();       //  Tile width
                    ushort tileHeight = stream.ReadWord();      //  Tile height
                    _ = stream.ReadShort();                     //  Base Index (ignored, UI only)
                    _ = stream.ReadBytes(14);                   //  Reserved (ignored)
                    string tilesetName = stream.ReadString();   //  Name of tileset

                    if (HasFlag(tilesetFlags, ASE_TILESET_FLAG_EXTERNAL_FILE))
                    {
                        stream.Dispose();
                        throw new InvalidOperationException($"Tileset '{tilesetName}' includes tileset in external file. This is not supported at this time");
                    }

                    if (HasFlag(tilesetFlags, ASE_TILESET_FLAG_EMBEDDED))
                    {
                        uint dataLength = stream.ReadDword();               //  Compressed data length
                        byte[] data = stream.ReadBytes((int)dataLength);    //  Compressed Tileset image

                        data = Decompress(data);
                        Rgba32[] pixels = PixelsToColor(data, colorDepth, palette);

                        Size tileSize = new(tileWidth, tileHeight);

                        AseTileset tileset = new((int)tilesetId, (int)tileCount, tileSize, tilesetName, pixels);
                        tilesets.Add(tileset);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Tileset '{tilesetName}' does not include tileset image in file");
                    }
                }
                else if (chunkType == ASE_CHUNK_OLD_PALETTE1 ||                     //  Only valid for pre v1.2, not supported
                                         chunkType == ASE_CHUNK_OLD_PALETTE2 ||     //  Only valid for pre v1.2, not supported
                                         chunkType == ASE_CHUNK_CEL_EXTRA ||        //  Only used by UI I think, not needed
                                         chunkType == ASE_CHUNK_COLOR_PROFILE ||    //  Not needed
                                         chunkType == ASE_CHUNK_EXTERNAL_FILES ||   //  Not implemented in 1.3 beta yet
                                         chunkType == ASE_CHUNK_MASK ||             //  Deprecated
                                         chunkType == ASE_CHUNK_PATH)               //  Never used
                {
                    stream.Position = chunKEnd;
                }
            }

            AseFrame frame = new(frameSize, duration, cels);
            frames.Add(frame);
        }

        return new AsepriteFile(frameSize, palette, frames, layers, tags, slices, tilesets);

    }

    //  Checks if the given value has the specified flag set
    private static bool HasFlag(uint value, uint flag) => (value & flag) != 0;

    //  Deflates a compressed Zlib buffer of data from an Aseprite file.
    private static byte[] Decompress(byte[] buffer)
    {
        using MemoryStream compressedStream = new(buffer);

        //  First 2 bytes are the zlib header information, skip past them.
        _ = compressedStream.ReadByte();
        _ = compressedStream.ReadByte();

        using MemoryStream decompressedStream = new();
        using DeflateStream deflateStream = new(compressedStream, CompressionMode.Decompress);
        deflateStream.CopyTo(decompressedStream);
        return decompressedStream.ToArray();
    }

    //  Begins reading the next chunk to see if it is a User Data chunk.  If it
    //  is, it reads the User Data chunk and sets the values read in the two
    //  out parameters, then returns true.
    //
    //  If it is not a User Data chunk, it resets the stream position back to
    //  where it was when this method was called, then return false.
    private static bool TryGetUserData(Stream stream, out string? text, out Rgba32? color)
    {
        text = default;
        color = default;

        //  Ensure we're not at the end of stream after reading the last chunk
        if (stream.Position != stream.Length)
        {
            _ = stream.ReadDword();                         //  Chunk Length
            if (stream.ReadWord() == ASE_CHUNK_USER_DATA)   //  Chunk Type (is it User Data?)
            {
                uint userDataFlags = stream.ReadDword();    //  User Data Flags

                if (HasFlag(userDataFlags, ASE_USER_DATA_FLAG_HAS_TEXT))
                {
                    text = stream.ReadString();             //  User Data Text
                }

                if (HasFlag(userDataFlags, ASE_USER_DATA_FLAG_HAS_COLOR))
                {
                    byte r = stream.ReadByteEx();           //  User Data RGBA Red value (0 - 255)
                    byte g = stream.ReadByteEx();           //  User Data RGBA Green value (0 - 255)
                    byte b = stream.ReadByteEx();           //  User Data RGBA Blue value (0 - 255)
                    byte a = stream.ReadByteEx();           //  User Data RGBA Alpha value (0 - 255)

                    color = Rgba32.FromRGBA(r, g, b, a);
                }

                return true;
            }

            //  Reset stream position 6-bytes
            //  (4-bytes for chunk length and 2-bytes for chunk type)
            stream.Position -= 6;
        }

        return false;
    }

    //  Given a array of pixel data and the bits per pixel, converts the byte
    //  data into an array of Rgba32 color values
    //
    //  Palette nad transparent index values needed incase the bits per pixel
    //  is 8 which means Indexed color depth mode.
    private static Rgba32[] PixelsToColor(byte[] pixels, ushort bitsPerPixel, AsePalette palette)
    {
        return bitsPerPixel switch
        {
            8 => IndexedPixelsToColor(pixels, palette),
            16 => GrayscalePixelsToColor(pixels),
            32 => RGBAPixelsToColor(pixels),
            _ => throw new InvalidOperationException("Unknown Color Depth")
        };
    }

    //  Translates RGBA Color depth mode (32-bits per pixel) into Rgba32 color
    //  values.
    private static Rgba32[] RGBAPixelsToColor(byte[] pixels)
    {
        int bytesPerPixel = (int)ColorDepth.RGBA / 8;
        Rgba32[] results = new Rgba32[pixels.Length / bytesPerPixel];

        for (int i = 0, b = 0; i < results.Length; i++, b += bytesPerPixel)
        {
            byte red = pixels[b];
            byte green = pixels[b + 1];
            byte blue = pixels[b + 2];
            byte alpha = pixels[b + 3];
            results[i] = Rgba32.FromRGBA(red, green, blue, alpha);
        }

        return results;
    }

    //  Translates Grayscale Color Depth mode (16-bits per pixel) into Rgba32
    //  color values.
    internal static Rgba32[] GrayscalePixelsToColor(byte[] pixels)
    {
        int bytesPerPixel = (int)ColorDepth.Grayscale / 8;
        Rgba32[] results = new Rgba32[pixels.Length / bytesPerPixel];

        for (int i = 0, b = 0; i < results.Length; i++, b += bytesPerPixel)
        {
            byte red = pixels[b];
            byte green = pixels[b];
            byte blue = pixels[b];
            byte alpha = pixels[b + 1];
            results[i] = Rgba32.FromRGBA(red, green, blue, alpha);
        }

        return results;
    }

    //  Translates Indexed Color Depth mode (8-bits per pixeL) into Rgba32 color
    //  values.
    internal static Rgba32[] IndexedPixelsToColor(byte[] pixels, AsePalette palette)
    {
        int bytesPerPixel = (int)ColorDepth.Indexed / 8;
        Rgba32[] results = new Rgba32[pixels.Length / bytesPerPixel];

        for (int i = 0; i < pixels.Length; i++)
        {
            int index = pixels[i];

            if (index == palette.TransparentIndex)
            {
                results[i] = Rgba32.Transparent;
            }
            else
            {
                results[i] = palette[index];
            }
        }

        return results;
    }
}