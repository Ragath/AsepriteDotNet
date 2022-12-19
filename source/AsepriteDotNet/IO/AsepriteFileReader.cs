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
using AsepriteDotNet.Color;
using AsepriteDotNet.Primitives;

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

    private record AseFrame(Size Size, int Duration, List<AseCel> Cels);

    private record AseCel(AseLayer Layer,
                          Point Position,
                          int Opacity,
                          Size Size,
                          Rgba32[]? Pixels = default,
                          AseTile[]? Tiles = default,
                          AseUserData? Userdata = default);

    private record AseLayer(bool IsVisible,
                            bool IsBackground,
                            bool IsReference,
                            int ChildLevel,
                            BlendMode BlendMode,
                            int Opacity,
                            string Name,
                            AseTileset? Tileset = default,
                            AseUserData? UserData = default);

    private record AsePalette(Rgba32[] Colors, int TransparentIndex);

    private record AseSlice(string Name,
                            bool IsNinePatch,
                            bool HasPivot,
                            List<AseSliceKey> Keys,
                            AseUserData? UserData = default);

    private record AseSliceKey(int Frame,
                               Rectangle Bounds,
                               Rectangle? CenterBounds = default,
                               Point? Pivot = default);

    private record AseTag(int From,
                          int To,
                          LoopDirection Direction,
                          Rgba32 Color,
                          string Name,
                          AseUserData? UserData = default);

    private record AseTile(uint ID,
                           uint XFlip,
                           uint YFlip,
                           uint Rotate);

    private record AseTileset(int ID,
                              int Count,
                              Size TileSize,
                              string Name,
                              Rgba32[] Pixels);

    private record AseUserData(string? Text, Rgba32? Color);

    private record PackResult(Size ImageSize, Rgba32[] Pixels, List<Rectangle> Frames);

    internal static Aseprite ReadFile(string path, FileReadOptions options)
    {
        using Stream stream = File.OpenRead(path);
        return ReadStream(stream, options);
    }

    private static Aseprite ReadStream(Stream stream, FileReadOptions options)
    {
        //  -------------------------------------------------------------------
        //  Read the file header
        //  -------------------------------------------------------------------
        _ = stream.ReadDword();             //  File size (don't need, ignored)
        ushort hMagic = stream.ReadWord();  //  Header magic (0xA5E0)

        if (hMagic != ASE_HEADER_MAGIC)
        {
            stream.Dispose();
            throw new InvalidOperationException($"Invalid header magic number (0x{hMagic:X4}). This does not appear to be a valid Aseprite file");
        }

        ushort frameCount = stream.ReadWord();  //  Total number of frames
        ushort frameWidth = stream.ReadWord();  //  Frame width, in pixels
        ushort frameHeight = stream.ReadWord(); //  Frame height, in pixels

        if (frameWidth < 1 || frameHeight < 1)
        {
            stream.Dispose();
            throw new InvalidOperationException($"Invalid canvas size {frameWidth}x{frameHeight}.");
        }

        Size fSize = new(frameWidth, frameHeight);

        ushort colorDepth = stream.ReadWord();  //  Color depth (bits per pixel)

        if (colorDepth != 32 && colorDepth != 16 && colorDepth != 8)
        {
            stream.Dispose();
            throw new InvalidOperationException($"Invalid color depth: {colorDepth}");
        }

        uint hFlags = stream.ReadDword();   //  Header flags

        bool isLayerOpacityValid = HasFlag(hFlags, ASE_HEADER_FLAG_LAYER_OPACITY_VALID);

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

        Rgba32[] palette = Array.Empty<Rgba32>();
        List<AseTileset> tilesets = new();
        List<AseLayer> layers = new();
        List<AseFrame> frames = new();
        List<AseTag> tags = new();
        List<AseSlice> slices = new();

        //  Read frame-by-frame until all frames are read.
        for (int fNum = 0; fNum < frameCount; fNum++)
        {
            List<AseCel> cels = new();

            //  ---------------------------------------------------------------
            //  Read Frame Header
            //  ---------------------------------------------------------------
            _ = stream.ReadDword();             //  Frame size in bytes (don't need, ignored)
            ushort fMagic = stream.ReadWord();  //  Frame magic number (0xF1FA)

            if (fMagic != ASE_FRAME_MAGIC)
            {
                stream.Dispose();
                throw new InvalidOperationException($"Invalid frame magic number (0x{fMagic:X4}) in frame {fNum}.");
            }

            int nChunks = stream.ReadWord();        //  Old field that specifies number of chunks
            ushort duration = stream.ReadWord();    //  Frame duration (ms)
            _ = stream.ReadBytes(2);                //  For future (set to 0) (ignored)
            uint moreChunks = stream.ReadDword();   //  New field that specifies number of chunks

            //  Determine actual chunk count
            if (nChunks == 0xFFFF && nChunks < moreChunks)
            {
                nChunks = (int)moreChunks;
            }

            //  Read chunk-by-chunk until all chunks are read
            for (int cNum = 0; cNum < nChunks; cNum++)
            {
                long cStart = stream.Position;

                //  -----------------------------------------------------------
                //  Read chunk header
                //  -----------------------------------------------------------
                uint chunkSize = stream.ReadDword();    //  Chunk size, in bytes
                ushort chunkType = stream.ReadWord();   //  Chunk type

                long cEnd = cStart + chunkSize;

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

                    if (layerType == ASE_LAYER_TYPE_TILEMAP)
                    {
                        uint tilesetIndex = stream.ReadDword(); //  Index of tileset used by tilemap layer
                        layer = layer with { Tileset = tilesets[(int)tilesetIndex] };
                    }

                    if (TryGetUserData(stream, out string? text, out Rgba32? color))
                    {
                        layer = layer with { UserData = new(text, color) };
                        cNum++;
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

                    Point position = new(xPosition, yPosition);
                    AseLayer layer = layers[layerIndex];
                    AseCel cel;

                    if (celType == ASE_CEL_TYPE_RAW_IMAGE || celType == ASE_CEL_TYPE_COMPRESSED_IMAGE)
                    {
                        ushort celWidth = stream.ReadWord();    //  Width of cel, in pixels
                        ushort celHeight = stream.ReadWord();   //  Height of cel, in pixels
                        byte[] data = stream.ReadTo(cEnd);      //  Cel pixel data

                        if (celType == ASE_CEL_TYPE_COMPRESSED_IMAGE)
                        {
                            data = Zlib.Deflate(data);
                        }

                        Size size = new(celWidth, celHeight);
                        Rgba32[] pixels = PixelsToColor(data, colorDepth, palette, transparentIndex);

                        cel = new(layer, position, celOpacity, size, pixels);
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
                        byte[] data = stream.ReadTo(cEnd);          //  Compressed tile data

                        data = Zlib.Deflate(data);
                        Size size = new(celWidth, celHeight);

                        //  Per Aseprite file spec, the "bits" per tile is, at
                        //  the moment, always 32-bits.  This means it's 4-bytes
                        //  per tile (32 / 8 = 4).  Meaning that each tile value
                        //  is a uint (DWORD)
                        int bytesPerTile = 4;
                        AseTile[] tiles = new AseTile[data.Length / bytesPerTile];

                        for (int i = 0, b = 0; i < tiles.Length; i++, b += bytesPerTile)
                        {
                            byte[] dword = data[b..(b + bytesPerTile)];
                            uint value = BitConverter.ToUInt32(dword);
                            uint tileId = (value & TILE_ID_MASK) >> TILE_ID_SHIFT;
                            uint xFlip = (value & TILE_FLIP_X_MASK);
                            uint yFlip = (value & TILE_FLIP_Y_MASK);
                            uint rotate = (value & TILE_90CW_ROTATION_MASK);

                            AseTile tile = new(tileId, xFlip, yFlip, rotate);
                            tiles[i] = tile;
                        }

                        cel = new(layer, position, celOpacity, size, null, tiles);
                    }
                    else
                    {
                        stream.Dispose();
                        throw new InvalidOperationException($"Unknown cel type '{celType}'");
                    }

                    if (TryGetUserData(stream, out string? text, out Rgba32? color))
                    {
                        cel = cel with { Userdata = new(text, color) };
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
                        Rgba32 tColor = Rgba32.FromRGBA(r, g, b, 255);

                        AseTag tag = new(fromFrame, toFrame, loopDirection, tColor, tagName);

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
                        Rgba32[] newPalette = new Rgba32[newSize];
                        Array.Copy(palette, newPalette, palette.Length);
                        palette = newPalette;
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

                    AseSlice slice = new(sliceName, isNinePatch, hasPivot, keys);

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

                        data = Zlib.Deflate(data);
                        Rgba32[] pixels = PixelsToColor(data, colorDepth, palette, transparentIndex);

                        Size tileSize = new(tileWidth, tileHeight);

                        AseTileset tileset = new((int)tilesetId, (int)tileCount, tileSize, tilesetName, pixels);
                        tilesets.Add(tileset);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Tileset '{tilesetName}' does not include tileset image in file");
                    }
                }
                else if (chunkType == ASE_CHUNK_OLD_PALETTE1 ||     //  Only valid for pre v1.2, not supported
                         chunkType == ASE_CHUNK_OLD_PALETTE2 ||     //  Only valid for pre v1.2, not supported
                         chunkType == ASE_CHUNK_CEL_EXTRA ||        //  Only used by UI I think, not needed
                         chunkType == ASE_CHUNK_COLOR_PROFILE ||    //  Not needed
                         chunkType == ASE_CHUNK_EXTERNAL_FILES ||   //  Not implemented in 1.3 beta yet
                         chunkType == ASE_CHUNK_MASK ||             //  Deprecated
                         chunkType == ASE_CHUNK_PATH)               //  Never used
                {
                    stream.Position = cEnd;
                }
            }

            AseFrame frame = new(fSize, duration, cels);
            frames.Add(frame);
        }

        PackResult spritesheetResult = GenerateSpritesheetImage(frames, fSize, options);
        Spritesheet spritesheet = new(spritesheetResult.ImageSize, spritesheetResult.Pixels, )


        Image spritesheetImage = GenerateSpritesheetImage(frames, options);
        List<Slice> sliceData = GenerateSliceData(slices);
        List<Animation> animations = GenerateAnimationData(frames, tags);
        List<Tilesheet> tilesheets = GenerateTilesetImages(tilesets, options);

        Spritesheet spritesheet = new(spritesheetImage, animations, sliceData);

        return new Aseprite(spritesheet, tilesheets);
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

    //  Generates a packed image from the individual frames.  Each frame is
    //  flattened by blending the cels on the top layers down to the bottom
    //  layer for each frame using each layer's blend mode, then packed
    //  using a square packing method into a final image.
    private static PackResult GenerateSpritesheetImage(List<AseFrame> frames, Size frameSize, FileReadOptions options)
    {
        List<Rgba32[]> flattenedFrames = FlattenFrames(frames, options);
        

        return Pack(frameSize, flattenedFrames, options);







































        // List<Image> images = new();

        // //  Flatten each frame
        // for (int i = 0; i < frames.Count; i++)
        // {
        //     AseFrame frame = frames[i];

        //     Rgba32[] pixels = new Rgba32[frame.Size.Width * frame.Size.Height];

        //     for (int celNum = 0; celNum < frame.Cels.Count; celNum++)
        //     {
        //         AseCel cel = frame.Cels[celNum];

        //         if (options.OnlyVisibleLayers && !cel.Layer.IsVisible)
        //         {
        //             continue;
        //         }

        //         if (cel.Pixels is not null)
        //         {
        //             byte opacity = Rgba32.MUL_UN8(cel.Opacity, cel.Layer.Opacity);

        //             for (int pixelNum = 0; pixelNum < cel.Pixels.Length; pixelNum++)
        //             {
        //                 int x = (pixelNum % cel.Size.Width) + cel.Position.X;
        //                 int y = (pixelNum / cel.Size.Width) + cel.Position.Y;
        //                 int index = y * frame.Size.Width + x;

        //                 //  Sometimes a cell can have a negative x and/or y value. This
        //                 //  is caused by selecting an area within aseprite and then 
        //                 //  moving a portion of the selected pixels outside the canvas. 
        //                 //  We don't care about these pixels so if the index is outside
        //                 //  the range of the array to store them in then we'll just
        //                 //  ignore them.
        //                 if (index < 0 || index >= pixels.Length) { continue; }

        //                 Rgba32 backdrop = pixels[index];
        //                 Rgba32 source = cel.Pixels[pixelNum];
        //                 pixels[index] = Rgba32.Blend(cel.Layer.BlendMode, backdrop, source, opacity);
        //             }
        //         }

        //         images.Add(new(frame.Size, pixels));
        //     }
        // }

        // return Image.Pack(frames[0].Size, images, options.MergeDuplicates, options.BorderPadding, options.Spacing, options.InnerPadding);
    }

    private static List<Rgba32[]> FlattenFrames(List<AseFrame> frames, FileReadOptions options)
    {
        List<Rgba32[]> flattenedFrames = new();

        //  Flatten each frame
        for (int frameNum = 0; frameNum < frames.Count; frameNum++)
        {
            AseFrame frame = frames[frameNum];

            Rgba32[] framePixels = new Rgba32[frame.Size.Width * frame.Size.Height];

            for (int celNum = 0; celNum < frame.Cels.Count; celNum++)
            {
                AseCel cel = frame.Cels[celNum];

                if (options.OnlyVisibleLayers && !cel.Layer.IsVisible)
                {
                    continue;
                }

                if (cel.Pixels is not null)
                {
                    byte opacity = Rgba32.MUL_UN8(cel.Opacity, cel.Layer.Opacity);

                    for (int pixelNum = 0; pixelNum < cel.Pixels.Length; pixelNum++)
                    {
                        int x = (pixelNum % cel.Size.Width) + cel.Position.X;
                        int y = (pixelNum / cel.Size.Width) + cel.Position.Y;
                        int index = y * frame.Size.Width + x;

                        //  Sometimes a cell can have a negative x and/or y 
                        //  value. This is caused by selecting an area within 
                        //  aseprite and then moving a portion of the selected
                        //  pixels outside the canvas.  We don't care about 
                        //  these pixels so if the index is outside the range of
                        //  the array to store them in then we'll just ignore 
                        //  them.
                        if (index < 0 || index >= framePixels.Length) { continue; }

                        Rgba32 backdrop = framePixels[index];
                        Rgba32 source = cel.Pixels[pixelNum];
                        framePixels[index] = Rgba32.Blend(cel.Layer.BlendMode, backdrop, source, opacity);
                    }
                }

                flattenedFrames.Add(framePixels);
            }
        }

        return flattenedFrames;
    }

    //  Generates the animation data from the tag data.
    private static List<Animation> GenerateAnimationData(List<AseFrame> frames, List<AseTag> tags)
    {
        List<Animation> animations = new();

        for (int tNum = 0; tNum < tags.Count; tNum++)
        {
            AseTag tag = tags[tNum];

            List<AnimationKey> keys = new();

            for (int start = tag.From; start <= tag.To; start++)
            {
                keys.Add(new(start, frames[start].Duration));
            }

            animations.Add(new(tag.Name, tag.Direction, keys));
        }

        return animations;
    }

    //  Generates the slice data for the sprite sheet.  Slice keys in Aseprite
    //  are defined with a frame value that indicates what frame the key starts
    //  on, but doesn't give what frame it ends on or is transformed on.  So
    //  we'll have to interpolate the slices per frame.
    private static List<Slice> GenerateSliceData(List<AseSlice> aseSlices)
    {
        List<Slice> slices = new();

        for (int sliceNum = 0; sliceNum < aseSlices.Count; sliceNum++)
        {
            AseSlice aseSlice = aseSlices[sliceNum];

            AseSliceKey? lastKey = default;

            for (int keyNum = 0; keyNum < aseSlice.Keys.Count; keyNum++)
            {
                AseSliceKey aseKey = aseSlice.Keys[keyNum];

                int frame = aseKey.Frame;
                string name = aseSlice.Name;
                Rgba32 color = aseSlice.UserData?.Color ?? Rgba32.FromRGBA(0, 0, 255, 255);
                Rectangle bounds = aseKey.Bounds;
                Rectangle? center = aseKey.CenterBounds;
                Point? pivot = aseKey.Pivot;

                if (lastKey?.Frame < aseKey.Frame)
                {
                    for (int offset = 1; offset < aseKey.Frame - lastKey.Frame; offset++)
                    {
                        Slice interpolatedSlice = new(name, frame + offset, bounds, color, center, pivot);
                        slices.Add(interpolatedSlice);
                    }
                }

                Slice slice = new(name, frame, bounds, color, center, pivot);
                slices.Add(slice);
                lastKey = aseKey;
            }
        }

        return slices;
    }

    //  The image/pixel data for a Tileset in Aseprite is packed into a vertical
    //  strip for some reason.  So first, we'll split the tiles out of the
    //  vertical packed arrangement and then repack them using a square packing
    //  method.
    private static List<Tilesheet> GenerateTilesetImages(List<AseTileset> aseTilesets, FileReadOptions options)
    {
        List<Tilesheet> tilesheets = new();

        for (int tilesetNum = 0; tilesetNum < aseTilesets.Count; tilesetNum++)
        {
            AseTileset aseTileset = aseTilesets[tilesetNum];

            //  Extract each individual tile from the vertical packed data.
            List<Image> tiles = new();
            int len = aseTileset.TileSize.Width * aseTileset.TileSize.Height;
            for (int i = 0; i < aseTileset.Count; i++)
            {
                Rgba32[] pixels = aseTileset.Pixels[(i * len)..((i * len) + len)];
                tiles.Add(new Image(aseTileset.TileSize, pixels));
            }

            //  Repack the tiles using the square packing method
            Image image = Image.Pack(aseTileset.TileSize, tiles, options.MergeDuplicates, options.BorderPadding, options.Spacing, options.InnerPadding);

            tilesheets.Add(new(aseTileset.Name, image));
        }

        return tilesheets;
    }


    private static PackResult Pack(Size frameSize, List<Rgba32[]> frames, FileReadOptions options)
    {
        int nFrames = frames.Count;
        List<Rectangle> sourceRects = new();
        Dictionary<int, Rectangle> originalToDuplicateLookup = new();
        Dictionary<int, int> frameDuplicateMap = new();

        if (options.MergeDuplicates)
        {
            for (int i = 0; i < frames.Count; i++)
            {
                for (int d = 0; d < i; d++)
                {
                    if (frames[i] == frames[d])
                    {
                        frameDuplicateMap.Add(i, d);
                        nFrames--;
                        break;
                    }
                }
            }
        }

        //  Determine the number of columns and ros needed to pack the frames
        //  into the image
        double sqrt = Math.Sqrt(nFrames);
        int columns = (int)Math.Floor(sqrt);
        if (Math.Abs(sqrt % 1) >= double.Epsilon)
        {
            columns++;
        }

        int rows = nFrames / columns;
        if (nFrames % columns != 0)
        {
            rows++;
        }

        //  Determine the final width and height of hte image based on the
        //  number of columns and rows, adjusting for padding and spacing
        int width = (columns * frameSize.Width) +
                    (options.BorderPadding * 2) +
                    (options.Spacing * (columns - 1)) +
                    (options.InnerPadding * 2 * columns);

        int height = (rows * frameSize.Height) +
                     (options.BorderPadding * 2) +
                     (options.Spacing * (columns - 1)) +
                     (options.InnerPadding * 2 * rows);

        Size imageSize = new(width, height);

        Rgba32[] imagePixels = new Rgba32[width * height];

        int fOffset = 0;    //  Offset for when we detect merged frames

        for (int fNum = 0; fNum < frames.Count; fNum++)
        {
            if (!options.MergeDuplicates || !frameDuplicateMap.ContainsKey(fNum))
            {
                //  Calculate the x- and y-coordinate position of the frame's
                //  top-left pixel relative to the top-left of the final image.
                int fCol = (fNum - fOffset) % columns;
                int fRow = (fNum - fOffset) / columns;

                //  Inject the pixel color data from the frame into the final
                //  image color array
                Rgba32[] fPixels = frames[fNum];

                for (int pNum = 0; pNum < fPixels.Length; pNum++)
                {
                    int x = (pNum % frameSize.Width) + (fCol * frameSize.Width);
                    int y = (pNum / frameSize.Width) + (fRow * frameSize.Height);

                    //  Adjust x- and y-coordinate for any padding and/or
                    //  spacing
                    x += options.BorderPadding +
                         (options.Spacing * fCol) +
                         (options.InnerPadding * (fCol + 1 + fCol));

                    y += options.BorderPadding +
                         (options.Spacing * fRow) +
                         (options.InnerPadding * (fRow + 1 + fRow));

                    int index = y * width + x;
                    imagePixels[index] = fPixels[pNum];
                }

                //  Now create the frame region
                Rectangle sourceRect = new(fCol * frameSize.Width, fRow * frameSize.Height, frameSize.Width, frameSize.Height);

                sourceRect.X += options.BorderPadding +
                                (options.Spacing * fCol) +
                                (options.InnerPadding * (fCol + 1 + fCol));

                sourceRect.Y += options.BorderPadding +
                                (options.Spacing * fRow) +
                                (options.InnerPadding * (fRow + 1 + fRow));

                sourceRects.Add(sourceRect);
                originalToDuplicateLookup.Add(fNum, sourceRect);
            }
            else
            {
                //  We ar emerging duplicates and it was detected that the
                //  current frame to process is a duplicate.  So we still 
                //  need to add the source rect.
                Rectangle original = originalToDuplicateLookup[frameDuplicateMap[fNum]];
                sourceRects.Add(original);
                fOffset++;

            }
        }

        return new PackResult(imageSize, imagePixels, sourceRects);
    }

    //  Checks if the given value has the specified flag set
    private static bool HasFlag(uint value, uint flag) => (value & flag) != 0;

    //  Given a array of pixel data and the bits per pixel, converts the byte
    //  data into an array of Rgba32 color values
    //
    //  Palette nad transparent index values needed incase the bits per pixel
    //  is 8 which means Indexed color depth mode.
    private static Rgba32[] PixelsToColor(byte[] pixels, ushort bitsPerPixel, Rgba32[] palette, int transIndex)
    {
        return bitsPerPixel switch
        {
            8 => IndexedPixelsToColor(pixels, palette, transIndex),
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
    internal static Rgba32[] IndexedPixelsToColor(byte[] pixels, Rgba32[] palette, int transIndex)
    {
        int bytesPerPixel = (int)ColorDepth.Indexed / 8;
        Rgba32[] results = new Rgba32[pixels.Length / bytesPerPixel];

        for (int i = 0; i < pixels.Length; i++)
        {
            int index = pixels[i];

            if (index == transIndex)
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







// internal static AsepriteFile ReadFile(Stream stream)
// {
//     //  Reference to the last group layer that is read so that subsequent
//     //  child layers can be added to it.
//     AsepriteGroupLayer? lastGroupLayer = default;

//     //  Read the Aseprite file header
//     _ = stream.ReadDword();             //  File size (ignored, don't need)
//     ushort hMagic = stream.ReadWord();  //  Header magic number

//     if (hMagic != ASE_HEADER_MAGIC)
//     {
//         stream.Dispose();
//         throw new InvalidOperationException($"Invalid header magic number (0x{hMagic:X4}). This does not appear to be a valid Aseprite file");
//     }

//     ushort nFrames = stream.ReadWord(); //  Total number of frames
//     ushort width = stream.ReadWord();   //  Width, in pixels
//     ushort height = stream.ReadWord();  //  Height, in pixels

//     if (width < 1 || height < 1)
//     {
//         stream.Dispose();
//         throw new InvalidOperationException($"Invalid canvas size {width}x{height}.");
//     }

//     ushort depth = stream.ReadWord();   //  Color depth (bits per pixel)

//     if (!Enum.IsDefined<ColorDepth>((ColorDepth)depth))
//     {
//         stream.Dispose();
//         throw new InvalidOperationException($"Invalid color depth: {depth}");
//     }

//     uint hFlags = stream.ReadDword();   //  Header flags

//     bool isLayerOpacityValid = HasFlag(hFlags, ASE_HEADER_FLAG_LAYER_OPACITY_VALID);

//     _ = stream.ReadWord();                          //  Speed (ms between frames) (deprecated)
//     _ = stream.ReadDword();                         //  Set to zero (ignored)
//     _ = stream.ReadDword();                         //  Set to zero (ignored)
//     byte transparentIndex = stream.ReadByteEx();    //  Index of transparent color in palette



//     AsepritePalette palette = new(transparentIndex);
//     AsepriteFile doc = new(palette, new Size(width, height), (ColorDepth)depth);

//     if (!isLayerOpacityValid)
//     {
//         doc.AddWarning("Layer opacity valid flag is not set. All layer opacity will default to 255");
//     }

//     if (transparentIndex > 0 && doc.ColorDepth != ColorDepth.Indexed)
//     {
//         //  Transparent color index is only valid in indexed depth
//         transparentIndex = 0;
//         doc.AddWarning("Transparent index only valid for Indexed Color Depth. Defaulting to 0");
//     }


//     _ = stream.ReadBytes(3);            //  Ignore these bytes
//     ushort nColors = stream.ReadWord(); //  Number of colors

//     //  Remainder of header is not needed, skipping to end of header
//     stream.Seek(ASE_HEADER_SIZE, SeekOrigin.Begin);

//     //  Read frame-by-frame until all frames are read.
//     for (int frameNum = 0; frameNum < nFrames; frameNum++)
//     {
//         List<AsepriteCel> cels = new();

//         //  Reference to the last chunk that can have user data so we can
//         //  apply a User Data chunk to it when one is read.
//         IAsepriteUserData? lastWithUserData = default;

//         //  Tracks the iteration of the tags when reading user data for
//         //  tags chunk.
//         int tagIterator = 0;

//         //  Read the frame header
//         _ = stream.ReadDword();             //  Bytes in frame (don't need, ignored)
//         ushort fMagic = stream.ReadWord();  //  Frame magic number

//         if (fMagic != ASE_FRAME_MAGIC)
//         {
//             stream.Dispose();
//             throw new InvalidOperationException($"Invalid frame magic number (0x{fMagic:X4}) in frame {frameNum}.");
//         }

//         int nChunks = stream.ReadWord();        //  Old field which specified chunk count
//         ushort duration = stream.ReadWord();    //  Frame duration, in millisecond
//         _ = stream.ReadBytes(2);                //  For future (set to zero)
//         uint moreChunks = stream.ReadDword();   //  New field which specifies chunk count

//         //  Determine which chunk count to use
//         if (nChunks == 0xFFFF && nChunks < moreChunks)
//         {
//             nChunks = (int)moreChunks;
//         }

//         //  Read chunk-by-chunk until all chunks in this frame are read.
//         for (int chunkNum = 0; chunkNum < nChunks; chunkNum++)
//         {
//             long chunkStart = stream.Position;
//             uint chunkLength = stream.ReadDword();  //  Size of chunk, in bytes
//             ushort chunkType = stream.ReadWord();   //  The type of chunk
//             long chunkEnd = chunkStart + chunkLength;

//             if (chunkType == ASE_CHUNK_LAYER)
//             {
//                 ushort layerFlags = stream.ReadWord();  //  Layer flags
//                 ushort layerType = stream.ReadWord();   //  Layer type
//                 ushort level = stream.ReadWord();       //  Layer child level
//                 _ = stream.ReadWord();                  //  Default layer width (ignored)
//                 _ = stream.ReadWord();                  //  Default layer height (ignored)
//                 ushort blend = stream.ReadWord();       //  Blend mode
//                 byte opacity = stream.ReadByteEx();     //  Layer opacity
//                 _ = stream.ReadBytes(3);                //  For future (set to zero)
//                 string name = stream.ReadString();      //  Layer name

//                 if (!isLayerOpacityValid)
//                 {
//                     opacity = 255;
//                 }

//                 //  Validate blend mode
//                 if (!Enum.IsDefined<BlendMode>((BlendMode)blend))
//                 {
//                     stream.Dispose();
//                     throw new InvalidOperationException($"Unknown blend mode '{blend}' found in layer '{name}'");
//                 }

//                 bool isVisible = HasFlag(layerFlags, ASE_LAYER_FLAG_VISIBLE);
//                 bool isBackground = HasFlag(layerFlags, ASE_LAYER_FLAG_BACKGROUND);
//                 bool isReference = HasFlag(layerFlags, ASE_LAYER_FLAG_REFERENCE);
//                 BlendMode mode = (BlendMode)blend;

//                 AsepriteLayer layer;

//                 if (layerType == ASE_LAYER_TYPE_NORMAL)
//                 {
//                     layer = new AsepriteImageLayer(isVisible, isBackground, isReference, level, mode, opacity, name);
//                 }
//                 else if (layerType == ASE_LAYER_TYPE_GROUP)
//                 {
//                     layer = new AsepriteGroupLayer(isVisible, isBackground, isReference, level, mode, opacity, name);
//                 }
//                 else if (layerType == ASE_LAYER_TYPE_TILEMAP)
//                 {
//                     uint index = stream.ReadDword();    //  Tileset index
//                     AsepriteTileset tileset = doc.Tilesets[(int)index];

//                     layer = new AsepriteTilemapLayer(tileset, isVisible, isBackground, isReference, level, mode, opacity, name);
//                 }
//                 else
//                 {
//                     stream.Dispose();
//                     throw new InvalidOperationException($"Unknown layer type '{layerType}'");
//                 }

//                 if (level != 0 && lastGroupLayer is not null)
//                 {
//                     lastGroupLayer.AddChild(layer);
//                 }

//                 if (layer is AsepriteGroupLayer gLayer)
//                 {
//                     lastGroupLayer = gLayer;
//                 }

//                 lastWithUserData = layer;
//                 doc.Add(layer);

//             }
//             else if (chunkType == ASE_CHUNK_CEL)
//             {
//                 ushort index = stream.ReadWord();       //  Layer index
//                 short x = stream.ReadShort();           //  X position
//                 short y = stream.ReadShort();           //  Y position
//                 byte opacity = stream.ReadByteEx();     //  Opacity level
//                 ushort type = stream.ReadWord();        //  Cel type
//                 _ = stream.ReadBytes(7);                //  For future (set to zero)

//                 AsepriteCel cel;
//                 Point position = new Point(x, y);
//                 AsepriteLayer celLayer = doc.Layers[index];

//                 if (type == ASE_CEL_TYPE_RAW_IMAGE)
//                 {
//                     ushort w = stream.ReadWord();               //  Width, in pixels
//                     ushort h = stream.ReadWord();               //  Height, in pixels
//                     byte[] pixelData = stream.ReadTo(chunkEnd); //  Raw pixel data

//                     Rgba32[] pixels = PixelsToColor(pixelData, doc.ColorDepth, doc.Palette);
//                     Size size = new Size(w, h);
//                     cel = new AsepriteImageCel(size, pixels, celLayer, position, opacity);
//                 }
//                 else if (type == ASE_CEL_TYPE_LINKED)
//                 {
//                     ushort frameIndex = stream.ReadWord();  //  Frame position to link with

//                     AsepriteCel otherCel = doc.Frames[frameIndex].Cels[cels.Count];
//                     cel = new AsepriteLinkedCel(otherCel, celLayer, position, opacity);
//                 }
//                 else if (type == ASE_CEL_TYPE_COMPRESSED_IMAGE)
//                 {
//                     ushort w = stream.ReadWord();                   //  Width, in pixels
//                     ushort h = stream.ReadWord();                   //  Height, in pixels
//                     byte[] compressed = stream.ReadTo(chunkEnd);    //  Raw pixel data compressed with Zlib
//                     byte[] pixelData = Zlib.Deflate(compressed);
//                     Rgba32[] pixels = PixelsToColor(pixelData, doc.ColorDepth, doc.Palette);

//                     Size size = new Size(w, h);
//                     cel = new AsepriteImageCel(size, pixels, celLayer, position, opacity);
//                 }
//                 else if (type == ASE_CEL_TYPE_COMPRESSED_TILEMAP)
//                 {
//                     ushort w = stream.ReadWord();                           //  Width, in number of tiles
//                     ushort h = stream.ReadWord();                           //  Height, in number of tiles
//                     ushort bpt = stream.ReadWord();                         //  Bits per tile
//                     uint id = stream.ReadDword();                           //  Bitmask for Tile ID
//                     uint xFlipBitmask = stream.ReadDword();                 //  Bitmask for X Flip
//                     uint yFlipBitmask = stream.ReadDword();                 //  Bitmask for Y Flip
//                     uint rotationBitmask = stream.ReadDword();              //  Bitmask for 90CW rotation
//                     _ = stream.ReadBytes(10);                               //  Reserved
//                     byte[] compressed = stream.ReadTo(chunkEnd);            //  Raw tile data compressed with Zlib

//                     byte[] tileData = Zlib.Deflate(compressed);

//                     Size size = new Size(w, h);

//                     //  Per Aseprite file spec, the "bits" per tile is, at
//                     //  the moment, always 32-bits.  This means it's 4-bytes
//                     //  per tile (32 / 8 = 4).  Meaning that each tile value
//                     //  is a uint (DWORD)
//                     int bytesPerTile = 4;
//                     AsepriteTile[] tiles = new AsepriteTile[tileData.Length / bytesPerTile];

//                     for (int i = 0, b = 0; i < tiles.Length; i++, b += bytesPerTile)
//                     {
//                         byte[] dword = tileData[b..(b + bytesPerTile)];
//                         uint value = BitConverter.ToUInt32(dword);
//                         uint tileId = (value & TILE_ID_MASK) >> TILE_ID_SHIFT;
//                         uint xFlip = (value & TILE_FLIP_X_MASK);
//                         uint yFlip = (value & TILE_FLIP_Y_MASK);
//                         uint rotate = (value & TILE_90CW_ROTATION_MASK);

//                         AsepriteTile tile = new(tileId, xFlip, yFlip, rotate);
//                         tiles[i] = tile;
//                     }

//                     cel = new AsepriteTilemapCel(size, bpt, id, xFlipBitmask, yFlipBitmask, rotationBitmask, tiles, celLayer, position, opacity);
//                 }
//                 else
//                 {
//                     stream.Dispose();
//                     throw new InvalidOperationException($"Unknown cel type '{type}'");
//                 }

//                 lastWithUserData = cel;
//                 cels.Add(cel);
//             }
//             else if (chunkType == ASE_CHUNK_TAGS)
//             {
//                 ushort nTags = stream.ReadWord();   //  Number of tags
//                 _ = stream.ReadBytes(8);            //  For future (set to zero)

//                 for (int i = 0; i < nTags; i++)
//                 {
//                     ushort from = stream.ReadWord();        //  From frame
//                     ushort to = stream.ReadWord();          //  To frame
//                     byte direction = stream.ReadByteEx();   //  Loop Direction

//                     //  Validate direction value
//                     if (!Enum.IsDefined<LoopDirection>((LoopDirection)direction))
//                     {
//                         stream.Dispose();
//                         throw new InvalidOperationException($"Unknown loop direction '{direction}'");
//                     }

//                     _ = stream.ReadBytes(8);            //  For future (set to zero)
//                     byte r = stream.ReadByteEx();       //  Red RGB value of tag color
//                     byte g = stream.ReadByteEx();       //  Green RGB value of tag color
//                     byte b = stream.ReadByteEx();       //  Blue RGB value of tag color
//                     _ = stream.ReadByteEx();            //  Extra byte (zero)
//                     string name = stream.ReadString();  //  Tag name

//                     LoopDirection loopDirection = (LoopDirection)direction;
//                     Rgba32 tagColor = Rgba32.FromRGBA(r, g, b, 255);

//                     AsepriteTag tag = new(from, to, loopDirection, tagColor, name);

//                     doc.Add(tag);
//                 }

//                 tagIterator = 0;
//                 lastWithUserData = doc.Tags.FirstOrDefault();
//             }
//             else if (chunkType == ASE_CHUNK_PALETTE)
//             {
//                 uint newSize = stream.ReadDword();  //  New palette size (total number of entries)
//                 uint from = stream.ReadDword();     //  First color index to change
//                 uint to = stream.ReadDword();       //  Last color index to change
//                 _ = stream.ReadBytes(8);            //  For future (set to zero)

//                 if (newSize > 0)
//                 {
//                     doc.Palette.Resize((int)newSize);
//                 }

//                 for (uint i = from; i <= to; i++)
//                 {
//                     ushort flags = stream.ReadWord();
//                     byte r = stream.ReadByteEx();
//                     byte g = stream.ReadByteEx();
//                     byte b = stream.ReadByteEx();
//                     byte a = stream.ReadByteEx();

//                     if (HasFlag(flags, ASE_PALETTE_FLAG_HAS_NAME))
//                     {
//                         _ = stream.ReadString();    //  Color name (ignored)
//                     }
//                     doc.Palette[(int)i] = Rgba32.FromRGBA(r, g, b, a);
//                 }
//             }
//             else if (chunkType == ASE_CHUNK_USER_DATA)
//             {
//                 uint flags = stream.ReadDword();    //  Flags

//                 string? text = default;
//                 if (HasFlag(flags, ASE_USER_DATA_FLAG_HAS_TEXT))
//                 {
//                     text = stream.ReadString();     //  User Data text
//                 }

//                 Rgba32? color = default;
//                 if (HasFlag(flags, ASE_USER_DATA_FLAG_HAS_COLOR))
//                 {
//                     byte r = stream.ReadByteEx();     //  Color Red (0 - 255)
//                     byte g = stream.ReadByteEx();     //  Color Green (0 - 255)
//                     byte b = stream.ReadByteEx();     //  Color Blue (0 - 255)
//                     byte a = stream.ReadByteEx();     //  Color Alpha (0 - 255)

//                     color = Rgba32.FromRGBA(r, g, b, a);
//                 }

//                 Debug.Assert(lastWithUserData is not null);

//                 if (lastWithUserData is not null)
//                 {
//                     lastWithUserData.UserData.Text = text;
//                     lastWithUserData.UserData.Color = color;

//                     if (lastWithUserData is AsepriteTag)
//                     {

//                         //  Tags are a special case, user data for tags 
//                         //  comes all together (one next to the other) after 
//                         //  the tags chunk, in the same order:
//                         //
//                         //  * TAGS CHUNK (TAG1, TAG2, ..., TAGn)
//                         //  * USER DATA CHUNK FOR TAG1
//                         //  * USER DATA CHUNK FOR TAG2
//                         //  * ...
//                         //  * USER DATA CHUNK FOR TAGn
//                         //
//                         //  So here we expect that the next user data chunk 
//                         //  will correspond to the next tag in the tags 
//                         //  collection
//                         tagIterator++;

//                         if (tagIterator < doc.Tags.Count)
//                         {
//                             lastWithUserData = doc.Tags[tagIterator];
//                         }
//                         else
//                         {
//                             lastWithUserData = null;
//                         }
//                     }

//                 }
//             }
//             else if (chunkType == ASE_CHUNK_SLICE)
//             {
//                 uint nKeys = stream.ReadDword();    //  Number of "slice keys"
//                 uint flags = stream.ReadDword();    //  Flags
//                 _ = stream.ReadDword();             //  Reserved
//                 string name = stream.ReadString();  //  Name

//                 bool isNinePatch = HasFlag(flags, ASE_SLICE_FLAGS_IS_NINE_PATCH);
//                 bool hasPivot = HasFlag(flags, ASE_SLICE_FLAGS_HAS_PIVOT);

//                 AsepriteSlice slice = new(isNinePatch, hasPivot, name);


//                 for (uint i = 0; i < nKeys; i++)
//                 {
//                     uint startFrame = stream.ReadDword();   //  Frame number this slice is valid starting from
//                     int x = stream.ReadLong();              //  Slice X origin coordinate in the sprite
//                     int y = stream.ReadLong();              //  Slice Y origin coordinate in the sprite
//                     uint w = stream.ReadDword();            //  Slice Width (can be 0 if slice is hidden)
//                     uint h = stream.ReadDword();            //  Slice Height (can be 0 if slice is hidden)

//                     Rectangle bounds = new Rectangle(x, y, (int)w, (int)h);
//                     Rectangle? center = default;
//                     Point? pivot = default;

//                     if (slice.IsNinePatch)
//                     {
//                         int cx = stream.ReadLong();     //  Center X position (relative to slice bounds)
//                         int cy = stream.ReadLong();     //  Center Y position (relative to slice bounds)
//                         uint cw = stream.ReadDword();   //  Center width
//                         uint ch = stream.ReadDword();   //  Center height

//                         center = new Rectangle(cx, cy, (int)cw, (int)ch);
//                     }

//                     if (slice.HasPivot)
//                     {
//                         int px = stream.ReadLong(); //  Pivot X position (relative to the slice origin)
//                         int py = stream.ReadLong(); //  Pivot Y position (relative to the slice origin)

//                         pivot = new Point(px, py);
//                     }

//                     AsepriteSliceKey key = new(slice, (int)startFrame, bounds, center, pivot);
//                 }

//                 doc.Add(slice);
//                 lastWithUserData = slice;
//             }
//             else if (chunkType == ASE_CHUNK_TILESET)
//             {
//                 uint id = stream.ReadDword();       //  Tileset ID
//                 uint flags = stream.ReadDword();    //  Tileset flags
//                 uint count = stream.ReadDword();    //  Number of tiles
//                 ushort w = stream.ReadWord();       //  Tile width
//                 ushort h = stream.ReadWord();       //  Tile height
//                 _ = stream.ReadShort();             //  Base index (ignoring, only used in Aseprite UI)
//                 _ = stream.ReadBytes(14);           //  Reserved
//                 string name = stream.ReadString();  //  Name of tileset


//                 if (HasFlag(flags, ASE_TILESET_FLAG_EXTERNAL_FILE))
//                 {
//                     stream.Dispose();
//                     throw new InvalidOperationException($"Tileset '{name}' includes tileset in external file. This is not supported at this time");
//                 }

//                 if (HasFlag(flags, ASE_TILESET_FLAG_EMBEDDED))
//                 {
//                     uint len = stream.ReadDword();                  //  Compressed data length
//                     byte[] compressed = stream.ReadBytes((int)len); //  Compressed tileset image

//                     byte[] pixelData = Zlib.Deflate(compressed);
//                     Rgba32[] pixels = PixelsToColor(pixelData, doc.ColorDepth, doc.Palette);

//                     Size tileSize = new Size(w, h);

//                     AsepriteTileset tileset = new((int)id, (int)count, tileSize, name, pixels);

//                     doc.Add(tileset);
//                 }
//                 else
//                 {
//                     throw new InvalidOperationException($"Tileset '{name}' does not include tileset image in file");
//                 }
//             }
//             else if (chunkType == ASE_CHUNK_OLD_PALETTE1)
//             {
//                 doc.AddWarning($"Old Palette Chunk (0x{chunkType:X4}) ignored");
//                 stream.Seek(chunkEnd, SeekOrigin.Begin);
//             }
//             else if (chunkType == ASE_CHUNK_OLD_PALETTE2)
//             {
//                 doc.AddWarning($"Old Palette Chunk (0x{chunkType:X4}) ignored");
//                 stream.Seek(chunkEnd, SeekOrigin.Begin);
//             }
//             else if (chunkType == ASE_CHUNK_CEL_EXTRA)
//             {
//                 doc.AddWarning($"Cel Extra Chunk (0x{chunkType:x4}) ignored");
//                 stream.Seek(chunkEnd, SeekOrigin.Begin);
//             }
//             else if (chunkType == ASE_CHUNK_COLOR_PROFILE)
//             {
//                 doc.AddWarning($"Color Profile Chunk (0x{chunkType:X4}) ignored");
//                 stream.Seek(chunkEnd, SeekOrigin.Begin);
//             }
//             else if (chunkType == ASE_CHUNK_EXTERNAL_FILES)
//             {
//                 doc.AddWarning($"External Files Chunk (0x{chunkType:X4}) ignored");
//                 stream.Seek(chunkEnd, SeekOrigin.Begin);
//             }
//             else if (chunkType == ASE_CHUNK_MASK)
//             {
//                 doc.AddWarning($"Mask Chunk (0x{chunkType:X4}) ignored");
//                 stream.Seek(chunkEnd, SeekOrigin.Begin);
//             }
//             else if (chunkType == ASE_CHUNK_PATH)
//             {
//                 doc.AddWarning($"Path Chunk (0x{chunkType:X4}) ignored");
//                 stream.Seek(chunkEnd, SeekOrigin.Begin);
//             }

//             Debug.Assert(stream.Position == chunkEnd);
//         }

//         AsepriteFrame frame = new(duration, cels, doc.Size);
//         doc.Add(frame);
//     }

//     if (doc.Palette.Count != nColors)
//     {
//         doc.AddWarning($"Number of colors in header ({nColors}) does not match final palette count ({doc.Palette.Count})");
//     }

//     return doc;
// }
