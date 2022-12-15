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
using AsepriteDotNet.Compression;
using AsepriteDotNet.AsepriteTypes;

namespace AsepriteDotNet.IO;

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

    /// <summary>
    ///     Reads the Aseprite file from the specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">
    ///     The absolute file path to the Aseprite file to read.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if invalid data is found within the Aseprite file while it
    ///     is being read. The exception message contains the details on what
    ///     value was invalid.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the specified <paramref name="path"/> is a zero-length
    ///     string, contains only white space, or contains one ore more
    ///     invalid characters. Use 
    ///     <see cref="System.IO.Path.GetInvalidPathChars"/> to query for 
    ///     invalid characters.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="PathTooLongException">
    ///     Thrown if the specified <paramref name="path"/>, file name, or
    ///     both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    ///     Thrown if the specified <paramref name="path"/> is invalid (for
    ///     example, it is on an unmapped drive).
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///     Thrown if the specified <paramref name="path"/> is a directory or
    ///     the caller does not have the required permissions.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///     Thrown if the file specified in the <paramref name="path"/> is not
    ///     found.
    /// </exception>
    /// <exception cref="NotSupportedException">
    ///     Thrown if the <paramref name="path"/> is in an invalid format.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown if an I/O error occurs while attempting to open the file.
    /// </exception>
    internal static AsepriteFile ReadFile(string path)
    {
        using Stream stream = File.OpenRead(path);
        return ReadFile(stream);
    }

    internal static AsepriteFile ReadFile(Stream stream)
    {
        //  Reference to the last group layer that is read so that subsequent
        //  child layers can be added to it.
        AsepriteGroupLayer? lastGroupLayer = default;

        //  Read the Aseprite file header
        _ = stream.ReadDword();             //  File size (ignored, don't need)
        ushort hMagic = stream.ReadWord();  //  Header magic number

        if (hMagic != ASE_HEADER_MAGIC)
        {
            stream.Dispose();
            throw new InvalidOperationException($"Invalid header magic number (0x{hMagic:X4}). This does not appear to be a valid Aseprite file");
        }

        ushort nFrames = stream.ReadWord(); //  Total number of frames
        ushort width = stream.ReadWord();   //  Width, in pixels
        ushort height = stream.ReadWord();  //  Height, in pixels

        if (width < 1 || height < 1)
        {
            stream.Dispose();
            throw new InvalidOperationException($"Invalid canvas size {width}x{height}.");
        }

        ushort depth = stream.ReadWord();   //  Color depth (bits per pixel)

        if (!Enum.IsDefined<ColorDepth>((ColorDepth)depth))
        {
            stream.Dispose();
            throw new InvalidOperationException($"Invalid color depth: {depth}");
        }

        uint hFlags = stream.ReadDword();   //  Header flags

        bool isLayerOpacityValid = HasFlag(hFlags, ASE_HEADER_FLAG_LAYER_OPACITY_VALID);

        _ = stream.ReadWord();                          //  Speed (ms between frames) (deprecated)
        _ = stream.ReadDword();                         //  Set to zero (ignored)
        _ = stream.ReadDword();                         //  Set to zero (ignored)
        byte transparentIndex = stream.ReadByteEx();    //  Index of transparent color in palette



        AsepritePalette palette = new(transparentIndex);
        AsepriteFile doc = new(palette, new Dimension(width, height), (ColorDepth)depth);

        if (!isLayerOpacityValid)
        {
            doc.AddWarning("Layer opacity valid flag is not set. All layer opacity will default to 255");
        }

        if (transparentIndex > 0 && doc.ColorDepth != ColorDepth.Indexed)
        {
            //  Transparent color index is only valid in indexed depth
            transparentIndex = 0;
            doc.AddWarning("Transparent index only valid for Indexed Color Depth. Defaulting to 0");
        }


        _ = stream.ReadBytes(3);            //  Ignore these bytes
        ushort nColors = stream.ReadWord(); //  Number of colors

        //  Remainder of header is not needed, skipping to end of header
        stream.Seek(ASE_HEADER_SIZE, SeekOrigin.Begin);

        //  Read frame-by-frame until all frames are read.
        for (int frameNum = 0; frameNum < nFrames; frameNum++)
        {
            List<AsepriteCel> cels = new();

            //  Reference to the last chunk that can have user data so we can
            //  apply a User Data chunk to it when one is read.
            IAsepriteUserData? lastWithUserData = default;

            //  Tracks the iteration of the tags when reading user data for
            //  tags chunk.
            int tagIterator = 0;

            //  Read the frame header
            _ = stream.ReadDword();             //  Bytes in frame (don't need, ignored)
            ushort fMagic = stream.ReadWord();  //  Frame magic number

            if (fMagic != ASE_FRAME_MAGIC)
            {
                stream.Dispose();
                throw new InvalidOperationException($"Invalid frame magic number (0x{fMagic:X4}) in frame {frameNum}.");
            }

            int nChunks = stream.ReadWord();        //  Old field which specified chunk count
            ushort duration = stream.ReadWord();    //  Frame duration, in millisecond
            _ = stream.ReadBytes(2);                //  For future (set to zero)
            uint moreChunks = stream.ReadDword();   //  New field which specifies chunk count

            //  Determine which chunk count to use
            if (nChunks == 0xFFFF && nChunks < moreChunks)
            {
                nChunks = (int)moreChunks;
            }

            //  Read chunk-by-chunk until all chunks in this frame are read.
            for (int chunkNum = 0; chunkNum < nChunks; chunkNum++)
            {
                long chunkStart = stream.Position;
                uint chunkLength = stream.ReadDword();  //  Size of chunk, in bytes
                ushort chunkType = stream.ReadWord();   //  The type of chunk
                long chunkEnd = chunkStart + chunkLength;

                if (chunkType == ASE_CHUNK_LAYER)
                {
                    ushort layerFlags = stream.ReadWord();  //  Layer flags
                    ushort layerType = stream.ReadWord();   //  Layer type
                    ushort level = stream.ReadWord();       //  Layer child level
                    _ = stream.ReadWord();                  //  Default layer width (ignored)
                    _ = stream.ReadWord();                  //  Default layer height (ignored)
                    ushort blend = stream.ReadWord();       //  Blend mode
                    byte opacity = stream.ReadByteEx();     //  Layer opacity
                    _ = stream.ReadBytes(3);                //  For future (set to zero)
                    string name = stream.ReadString();      //  Layer name

                    if (!isLayerOpacityValid)
                    {
                        opacity = 255;
                    }

                    //  Validate blend mode
                    if (!Enum.IsDefined<BlendMode>((BlendMode)blend))
                    {
                        stream.Dispose();
                        throw new InvalidOperationException($"Unknown blend mode '{blend}' found in layer '{name}'");
                    }

                    bool isVisible = HasFlag(layerFlags, ASE_LAYER_FLAG_VISIBLE);
                    bool isBackground = HasFlag(layerFlags, ASE_LAYER_FLAG_BACKGROUND);
                    bool isReference = HasFlag(layerFlags, ASE_LAYER_FLAG_REFERENCE);
                    BlendMode mode = (BlendMode)blend;

                    AsepriteLayer layer;

                    if (layerType == ASE_LAYER_TYPE_NORMAL)
                    {
                        layer = new AsepriteImageLayer(isVisible, isBackground, isReference, level, mode, opacity, name);
                    }
                    else if (layerType == ASE_LAYER_TYPE_GROUP)
                    {
                        layer = new AsepriteGroupLayer(isVisible, isBackground, isReference, level, mode, opacity, name);
                    }
                    else if (layerType == ASE_LAYER_TYPE_TILEMAP)
                    {
                        uint index = stream.ReadDword();    //  Tileset index
                        AsepriteTileset tileset = doc.Tilesets[(int)index];

                        layer = new AsepriteTilemapLayer(tileset, isVisible, isBackground, isReference, level, mode, opacity, name);
                    }
                    else
                    {
                        stream.Dispose();
                        throw new InvalidOperationException($"Unknown layer type '{layerType}'");
                    }

                    if (level != 0 && lastGroupLayer is not null)
                    {
                        lastGroupLayer.AddChild(layer);
                    }

                    if (layer is AsepriteGroupLayer gLayer)
                    {
                        lastGroupLayer = gLayer;
                    }

                    lastWithUserData = layer;
                    doc.Add(layer);

                }
                else if (chunkType == ASE_CHUNK_CEL)
                {
                    ushort index = stream.ReadWord();       //  Layer index
                    short x = stream.ReadShort();           //  X position
                    short y = stream.ReadShort();           //  Y position
                    byte opacity = stream.ReadByteEx();     //  Opacity level
                    ushort type = stream.ReadWord();        //  Cel type
                    _ = stream.ReadBytes(7);                //  For future (set to zero)

                    AsepriteCel cel;
                    Location position = new Location(x, y);
                    AsepriteLayer celLayer = doc.Layers[index];

                    if (type == ASE_CEL_TYPE_RAW_IMAGE)
                    {
                        ushort w = stream.ReadWord();               //  Width, in pixels
                        ushort h = stream.ReadWord();               //  Height, in pixels
                        byte[] pixelData = stream.ReadTo(chunkEnd); //  Raw pixel data

                        Rgba32[] pixels = PixelsToColor(pixelData, doc.ColorDepth, doc.Palette);
                        Dimension size = new Dimension(w, h);
                        cel = new AsepriteImageCel(size, pixels, celLayer, position, opacity);
                    }
                    else if (type == ASE_CEL_TYPE_LINKED)
                    {
                        ushort frameIndex = stream.ReadWord();  //  Frame position to link with

                        AsepriteCel otherCel = doc.Frames[frameIndex].Cels[cels.Count];
                        cel = new AsepriteLinkedCel(otherCel, celLayer, position, opacity);
                    }
                    else if (type == ASE_CEL_TYPE_COMPRESSED_IMAGE)
                    {
                        ushort w = stream.ReadWord();                   //  Width, in pixels
                        ushort h = stream.ReadWord();                   //  Height, in pixels
                        byte[] compressed = stream.ReadTo(chunkEnd);    //  Raw pixel data compressed with Zlib
                        byte[] pixelData = Zlib.Deflate(compressed);
                        Rgba32[] pixels = PixelsToColor(pixelData, doc.ColorDepth, doc.Palette);

                        Dimension size = new Dimension(w, h);
                        cel = new AsepriteImageCel(size, pixels, celLayer, position, opacity);
                    }
                    else if (type == ASE_CEL_TYPE_COMPRESSED_TILEMAP)
                    {
                        ushort w = stream.ReadWord();                           //  Width, in number of tiles
                        ushort h = stream.ReadWord();                           //  Height, in number of tiles
                        ushort bpt = stream.ReadWord();                         //  Bits per tile
                        uint id = stream.ReadDword();                           //  Bitmask for Tile ID
                        uint xFlipBitmask = stream.ReadDword();                 //  Bitmask for X Flip
                        uint yFlipBitmask = stream.ReadDword();                 //  Bitmask for Y Flip
                        uint rotationBitmask = stream.ReadDword();              //  Bitmask for 90CW rotation
                        _ = stream.ReadBytes(10);                               //  Reserved
                        byte[] compressed = stream.ReadTo(chunkEnd);            //  Raw tile data compressed with Zlib

                        byte[] tileData = Zlib.Deflate(compressed);

                        Dimension size = new Dimension(w, h);

                        //  Per Aseprite file spec, the "bits" per tile is, at
                        //  the moment, always 32-bits.  This means it's 4-bytes
                        //  per tile (32 / 8 = 4).  Meaning that each tile value
                        //  is a uint (DWORD)
                        int bytesPerTile = 4;
                        AsepriteTile[] tiles = new AsepriteTile[tileData.Length / bytesPerTile];

                        for (int i = 0, b = 0; i < tiles.Length; i++, b += bytesPerTile)
                        {
                            byte[] dword = tileData[b..(b + bytesPerTile)];
                            uint value = BitConverter.ToUInt32(dword);
                            uint tileId = (value & TILE_ID_MASK) >> TILE_ID_SHIFT;
                            uint xFlip = (value & TILE_FLIP_X_MASK);
                            uint yFlip = (value & TILE_FLIP_Y_MASK);
                            uint rotate = (value & TILE_90CW_ROTATION_MASK);

                            AsepriteTile tile = new(tileId, xFlip, yFlip, rotate);
                            tiles[i] = tile;
                        }

                        cel = new AsepriteTilemapCel(size, bpt, id, xFlipBitmask, yFlipBitmask, rotationBitmask, tiles, celLayer, position, opacity);
                    }
                    else
                    {
                        stream.Dispose();
                        throw new InvalidOperationException($"Unknown cel type '{type}'");
                    }

                    lastWithUserData = cel;
                    cels.Add(cel);
                }
                else if (chunkType == ASE_CHUNK_TAGS)
                {
                    ushort nTags = stream.ReadWord();   //  Number of tags
                    _ = stream.ReadBytes(8);            //  For future (set to zero)

                    for (int i = 0; i < nTags; i++)
                    {
                        ushort from = stream.ReadWord();        //  From frame
                        ushort to = stream.ReadWord();          //  To frame
                        byte direction = stream.ReadByteEx();   //  Loop Direction

                        //  Validate direction value
                        if (!Enum.IsDefined<LoopDirection>((LoopDirection)direction))
                        {
                            stream.Dispose();
                            throw new InvalidOperationException($"Unknown loop direction '{direction}'");
                        }

                        _ = stream.ReadBytes(8);            //  For future (set to zero)
                        byte r = stream.ReadByteEx();       //  Red RGB value of tag color
                        byte g = stream.ReadByteEx();       //  Green RGB value of tag color
                        byte b = stream.ReadByteEx();       //  Blue RGB value of tag color
                        _ = stream.ReadByteEx();            //  Extra byte (zero)
                        string name = stream.ReadString();  //  Tag name

                        LoopDirection loopDirection = (LoopDirection)direction;
                        Rgba32 tagColor = Rgba32.FromRGBA(r, g, b, 255);

                        AsepriteTag tag = new(from, to, loopDirection, tagColor, name);

                        doc.Add(tag);
                    }

                    tagIterator = 0;
                    lastWithUserData = doc.Tags.FirstOrDefault();
                }
                else if (chunkType == ASE_CHUNK_PALETTE)
                {
                    uint newSize = stream.ReadDword();  //  New palette size (total number of entries)
                    uint from = stream.ReadDword();     //  First color index to change
                    uint to = stream.ReadDword();       //  Last color index to change
                    _ = stream.ReadBytes(8);            //  For future (set to zero)

                    if (newSize > 0)
                    {
                        doc.Palette.Resize((int)newSize);
                    }

                    for (uint i = from; i <= to; i++)
                    {
                        ushort flags = stream.ReadWord();
                        byte r = stream.ReadByteEx();
                        byte g = stream.ReadByteEx();
                        byte b = stream.ReadByteEx();
                        byte a = stream.ReadByteEx();

                        if (HasFlag(flags, ASE_PALETTE_FLAG_HAS_NAME))
                        {
                            _ = stream.ReadString();    //  Color name (ignored)
                        }
                        doc.Palette[(int)i] = Rgba32.FromRGBA(r, g, b, a);
                    }
                }
                else if (chunkType == ASE_CHUNK_USER_DATA)
                {
                    uint flags = stream.ReadDword();    //  Flags

                    string? text = default;
                    if (HasFlag(flags, ASE_USER_DATA_FLAG_HAS_TEXT))
                    {
                        text = stream.ReadString();     //  User Data text
                    }

                    Rgba32? color = default;
                    if (HasFlag(flags, ASE_USER_DATA_FLAG_HAS_COLOR))
                    {
                        byte r = stream.ReadByteEx();     //  Color Red (0 - 255)
                        byte g = stream.ReadByteEx();     //  Color Green (0 - 255)
                        byte b = stream.ReadByteEx();     //  Color Blue (0 - 255)
                        byte a = stream.ReadByteEx();     //  Color Alpha (0 - 255)

                        color = Rgba32.FromRGBA(r, g, b, a);
                    }

                    Debug.Assert(lastWithUserData is not null);

                    if (lastWithUserData is not null)
                    {
                        lastWithUserData.UserData.Text = text;
                        lastWithUserData.UserData.Color = color;

                        if (lastWithUserData is AsepriteTag)
                        {

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
                            tagIterator++;

                            if (tagIterator < doc.Tags.Count)
                            {
                                lastWithUserData = doc.Tags[tagIterator];
                            }
                            else
                            {
                                lastWithUserData = null;
                            }
                        }

                    }
                }
                else if (chunkType == ASE_CHUNK_SLICE)
                {
                    uint nKeys = stream.ReadDword();    //  Number of "slice keys"
                    uint flags = stream.ReadDword();    //  Flags
                    _ = stream.ReadDword();             //  Reserved
                    string name = stream.ReadString();  //  Name

                    bool isNinePatch = HasFlag(flags, ASE_SLICE_FLAGS_IS_NINE_PATCH);
                    bool hasPivot = HasFlag(flags, ASE_SLICE_FLAGS_HAS_PIVOT);

                    AsepriteSlice slice = new(isNinePatch, hasPivot, name);


                    for (uint i = 0; i < nKeys; i++)
                    {
                        uint startFrame = stream.ReadDword();   //  Frame number this slice is valid starting from
                        int x = stream.ReadLong();              //  Slice X origin coordinate in the sprite
                        int y = stream.ReadLong();              //  Slice Y origin coordinate in the sprite
                        uint w = stream.ReadDword();            //  Slice Width (can be 0 if slice is hidden)
                        uint h = stream.ReadDword();            //  Slice Height (can be 0 if slice is hidden)

                        Rect bounds = new Rect(x, y, (int)w, (int)h);
                        Rect? center = default;
                        Location? pivot = default;

                        if (slice.IsNinePatch)
                        {
                            int cx = stream.ReadLong();     //  Center X position (relative to slice bounds)
                            int cy = stream.ReadLong();     //  Center Y position (relative to slice bounds)
                            uint cw = stream.ReadDword();   //  Center width
                            uint ch = stream.ReadDword();   //  Center height

                            center = new Rect(cx, cy, (int)cw, (int)ch);
                        }

                        if (slice.HasPivot)
                        {
                            int px = stream.ReadLong(); //  Pivot X position (relative to the slice origin)
                            int py = stream.ReadLong(); //  Pivot Y position (relative to the slice origin)

                            pivot = new Location(px, py);
                        }

                        AsepriteSliceKey key = new(slice, (int)startFrame, bounds, center, pivot);
                    }

                    doc.Add(slice);
                    lastWithUserData = slice;
                }
                else if (chunkType == ASE_CHUNK_TILESET)
                {
                    uint id = stream.ReadDword();       //  Tileset ID
                    uint flags = stream.ReadDword();    //  Tileset flags
                    uint count = stream.ReadDword();    //  Number of tiles
                    ushort w = stream.ReadWord();       //  Tile width
                    ushort h = stream.ReadWord();       //  Tile height
                    _ = stream.ReadShort();             //  Base index (ignoring, only used in Aseprite UI)
                    _ = stream.ReadBytes(14);           //  Reserved
                    string name = stream.ReadString();  //  Name of tileset


                    if (HasFlag(flags, ASE_TILESET_FLAG_EXTERNAL_FILE))
                    {
                        stream.Dispose();
                        throw new InvalidOperationException($"Tileset '{name}' includes tileset in external file. This is not supported at this time");
                    }

                    if (HasFlag(flags, ASE_TILESET_FLAG_EMBEDDED))
                    {
                        uint len = stream.ReadDword();                  //  Compressed data length
                        byte[] compressed = stream.ReadBytes((int)len); //  Compressed tileset image

                        byte[] pixelData = Zlib.Deflate(compressed);
                        Rgba32[] pixels = PixelsToColor(pixelData, doc.ColorDepth, doc.Palette);

                        Dimension tileSize = new Dimension(w, h);

                        AsepriteTileset tileset = new((int)id, (int)count, tileSize, name, pixels);

                        doc.Add(tileset);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Tileset '{name}' does not include tileset image in file");
                    }
                }
                else if (chunkType == ASE_CHUNK_OLD_PALETTE1)
                {
                    doc.AddWarning($"Old Palette Chunk (0x{chunkType:X4}) ignored");
                    stream.Seek(chunkEnd, SeekOrigin.Begin);
                }
                else if (chunkType == ASE_CHUNK_OLD_PALETTE2)
                {
                    doc.AddWarning($"Old Palette Chunk (0x{chunkType:X4}) ignored");
                    stream.Seek(chunkEnd, SeekOrigin.Begin);
                }
                else if (chunkType == ASE_CHUNK_CEL_EXTRA)
                {
                    doc.AddWarning($"Cel Extra Chunk (0x{chunkType:x4}) ignored");
                    stream.Seek(chunkEnd, SeekOrigin.Begin);
                }
                else if (chunkType == ASE_CHUNK_COLOR_PROFILE)
                {
                    doc.AddWarning($"Color Profile Chunk (0x{chunkType:X4}) ignored");
                    stream.Seek(chunkEnd, SeekOrigin.Begin);
                }
                else if (chunkType == ASE_CHUNK_EXTERNAL_FILES)
                {
                    doc.AddWarning($"External Files Chunk (0x{chunkType:X4}) ignored");
                    stream.Seek(chunkEnd, SeekOrigin.Begin);
                }
                else if (chunkType == ASE_CHUNK_MASK)
                {
                    doc.AddWarning($"Mask Chunk (0x{chunkType:X4}) ignored");
                    stream.Seek(chunkEnd, SeekOrigin.Begin);
                }
                else if (chunkType == ASE_CHUNK_PATH)
                {
                    doc.AddWarning($"Path Chunk (0x{chunkType:X4}) ignored");
                    stream.Seek(chunkEnd, SeekOrigin.Begin);
                }

                Debug.Assert(stream.Position == chunkEnd);
            }

            AsepriteFrame frame = new(duration, cels, doc.Size);
            doc.Add(frame);
        }

        if (doc.Palette.Count != nColors)
        {
            doc.AddWarning($"Number of colors in header ({nColors}) does not match final palette count ({doc.Palette.Count})");
        }

        return doc;
    }

    private static bool HasFlag(uint value, uint flag) => (value & flag) != 0;

    internal static Rgba32[] PixelsToColor(byte[] pixels, ColorDepth depth, AsepritePalette palette)
    {
        return depth switch
        {
            ColorDepth.Indexed => IndexedPixelsToColor(pixels, palette),
            ColorDepth.Grayscale => GrayscalePixelsToColor(pixels),
            ColorDepth.RGBA => RGBAPixelsToColor(pixels),
            _ => throw new InvalidOperationException("Unknown Color Depth")
        };
    }

    internal static Rgba32[] RGBAPixelsToColor(byte[] pixels)
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

    internal static Rgba32[] IndexedPixelsToColor(byte[] pixels, AsepritePalette palette)
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