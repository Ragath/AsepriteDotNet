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
using System.Collections.ObjectModel;

using AsepriteDotNet.AsepriteTypes;
using AsepriteDotNet.Color;
using AsepriteDotNet.ImageInternal;
using AsepriteDotNet.IO;
using AsepriteDotNet.Primitives;

namespace AsepriteDotNet;

/// <summary>
///     Represents the contents of an Aseprite file.
/// </summary>
public sealed class AsepriteFile
{
    private List<AsepriteFrame> _frames = new();
    private List<AsepriteLayer> _layers = new();
    private List<AsepriteTag> _tags = new();
    private List<AsepriteSlice> _slices = new();
    private List<AsepriteTileset> _tilesets = new();
    private List<string> _warnings = new();

    /// <summary>
    ///     Gets the width and height, in pixels, of the sprite in this
    ///     <see cref="AsepriteFile"/>.
    /// </summary>
    public Size Size { get; }

    /// <summary>
    ///     Gets the <see cref="ColorDepth"/> (bits per pixel) used by this
    ///     <see cref="AsepriteFile"/>.
    /// </summary>
    public ColorDepth ColorDepth { get; }

    /// <summary>
    ///     Gets a read-only collection of all <see cref="AsepriteFrame"/> elements for
    ///     this <see cref="AsepriteFile"/>.
    /// </summary>
    public ReadOnlyCollection<AsepriteFrame> Frames { get; }

    /// <summary>
    ///     Gets a read-only collection of all <see cref="AsepriteLayer"/> elements for
    ///     this <see cref="AsepriteFile"/>.
    /// </summary>
    public ReadOnlyCollection<AsepriteLayer> Layers { get; }

    /// <summary>
    ///     Gets a read-only collection of all <see cref="AsepriteTag"/> elements for
    ///     this <see cref="AsepriteFile"/>.
    /// </summary>
    public ReadOnlyCollection<AsepriteTag> Tags { get; }

    /// <summary>
    ///     Gets a read-only collection of all <see cref="AsepriteSlice"/> elements for
    ///     this <see cref="AsepriteFile"/>.
    /// </summary>
    public ReadOnlyCollection<AsepriteSlice> Slices { get; }

    /// <summary>
    ///     Gets a read-only collection of all <see cref="AsepriteTileset"/> elements
    ///     for this <see cref="AsepriteFile"/>.
    /// </summary>
    public ReadOnlyCollection<AsepriteTileset> Tilesets { get; }

    /// <summary>
    ///     Gets a read-only collection of all warnings that were produced
    ///     while importing this <see cref="AsepriteFile"/>.
    /// </summary>
    public ReadOnlyCollection<string> Warnings { get; }

    /// <summary>
    ///     Gets the <see cref="Palette"/> for this 
    ///     <see cref="AsepriteFile"/>
    /// </summary>
    public AsepritePalette Palette { get; }

    internal AsepriteFile(AsepritePalette palette, Size size, ColorDepth colorDepth)
    {
        Size = size;
        ColorDepth = colorDepth;
        Palette = palette;
        Frames = _frames.AsReadOnly();
        Layers = _layers.AsReadOnly();
        Tags = _tags.AsReadOnly();
        Slices = _slices.AsReadOnly();
        Tilesets = _tilesets.AsReadOnly();
        Warnings = _warnings.AsReadOnly();
    }

    internal void Add(AsepriteFrame frame) => _frames.Add(frame);
    internal void Add(AsepriteLayer layer) => _layers.Add(layer);
    internal void Add(AsepriteTag tag) => _tags.Add(tag);
    internal void Add(AsepriteSlice slice) => _slices.Add(slice);
    internal void Add(AsepriteTileset tileset) => _tilesets.Add(tileset);
    internal void AddWarning(string message) => _warnings.Add(message);

    /// <summary>
    ///     Loads the Aseprite file at the specified
    ///     <paramref name="filePath"/>.  The result is a new 
    ///     <see cref="AsepriteFile"/> class instance initialized with the 
    ///     data read from the Aseprite file.
    /// </summary>
    /// <param name="filePath">
    ///     The absolute file path to the Aseprite file.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="AsepriteFile"/> class initialized
    ///     with the data read from the Aseprite file.
    /// </returns>
    /// <exception cref="AsepriteFileLoadException">
    ///     Thrown if an exception occurs during the loading of the Aseprite
    ///     file. Refer to the inner exception for the exact error.
    /// </exception>
    public static AsepriteFile Load(string filePath)
    {
        try
        {
            return AsepriteFileReader.ReadFile(filePath);
        }
        catch (Exception ex)
        {
            throw new AsepriteFileLoadException($"An error occurred while loading the Aseprite file. Please see inner exception for exact error.", ex);
        }
    }

    /// <summary>
    ///     Generates a new <see cref="AsepriteSheet"/> class instance from the
    ///     data within this <see cref="AsepriteFile"/>.
    /// </summary>
    /// <param name="spritesheetOptions">
    ///     The options to adhere to when generating the spritesheet from this
    ///     <see cref="AsepriteFile"/>.
    /// </param>
    /// <param name="tilesheetOptions">
    ///     The options to adhere to when generating the tilesheets from this
    ///     <see cref="AsepriteFile"/>.
    /// </param>
    /// <returns>
    ///     The <see cref="AsepriteSheet"/> that is created by this method.
    /// </returns>
    public AsepriteSheet ToAsepriteSheet(bool onlyVisibleLayers = true,
                                         bool mergeDuplicates = true,
                                         int borderPadding = 0,
                                         int spacing = 0,
                                         int innerPadding = 0)
    {
        Spritesheet spritesheet = ToSpritesheet(onlyVisibleLayers, mergeDuplicates, borderPadding, spacing, innerPadding);
        List<Tilesheet> tilesheets = new();

        for (int i = 0; i < Tilesets.Count; i++)
        {
            Tilesheet tilesheet = Tilesets[i].ToTilesheet(mergeDuplicates, borderPadding, spacing, innerPadding);
            tilesheets.Add(tilesheet);
        }

        return new AsepriteSheet(spritesheet, tilesheets);
    }

    /// <summary>
    ///     Generates a new <see cref="Spritesheet"/> class instance from the
    ///     frame, slice, and tag data within this <see cref="AsepriteFile"/>.
    /// </summary>
    /// <param name="options">
    ///     The options to adhere to when generating the spritesheet for the
    ///     <see cref="AsepriteSheet"/>.
    /// </param>
    /// <returns>
    ///     The <see cref="Spritesheet"/> that is created by this method.
    /// </returns>
    public Spritesheet ToSpritesheet(bool onlyVisibleLayers = true, bool mergeDuplicates = true, int borderPadding = 0, int spacing = 0, int innerPadding = 0)
    {
        List<SpritesheetFrame> sheetFrames = new();
        List<SpritesheetAnimation> sheetAnimations = new();

        //  Process frames and the pixels

        Dictionary<int, Rgba32[]> frameColorLookup = new Dictionary<int, Rgba32[]>();
        Dictionary<int, int> frameDuplicateMap = new Dictionary<int, int>();

        for (int frameNum = 0; frameNum < Frames.Count; frameNum++)
        {
            frameColorLookup.Add(frameNum, Frames[frameNum].FlattenFrame(onlyVisibleLayers).Pixels);
        }

        int totalFrames = frameColorLookup.Count;

        if (mergeDuplicates)
        {
            for (int i = 0; i < frameColorLookup.Count; i++)
            {
                for (int d = 0; d < i; d++)
                {
                    if (frameColorLookup[i].SequenceEqual(frameColorLookup[d]))
                    {
                        frameDuplicateMap.Add(i, d);
                        break;
                    }
                }
            }

            //  Since we are merging duplicates, we need to subtract the
            //  number of duplicates from the total frame count
            totalFrames -= frameDuplicateMap.Count;
        }

        //  Determine the number of columns and rows needed to pack the frames
        //  into the spritesheet
        double sqrt = Math.Sqrt(totalFrames);
        int columns = (int)Math.Floor(sqrt);
        if (Math.Abs(sqrt % 1) >= double.Epsilon)
        {
            columns++;
        }

        int rows = totalFrames / columns;
        if (totalFrames % columns != 0)
        {
            rows++;
        }

        //  Determine the final width and height of the spritesheet based on the
        //  number of columns and rows and adjusting for padding and spacing
        int width = (columns * Size.Width) +
                    (borderPadding * 2) +
                    (spacing * (columns - 1)) +
                    (innerPadding * 2 * columns);

        int height = (rows * Size.Height) +
                     (borderPadding * 2) +
                     (spacing * (rows - 1)) +
                     (innerPadding * 2 * rows);

        Size sheetSize = new(width, height);

        Rgba32[] sheetPixels = new Rgba32[width * height];

        Dictionary<int, SpritesheetFrame> originalToDuplicateFrameLookup = new();

        int fOffset = 0;

        for (int frameNum = 0; frameNum < Frames.Count; frameNum++)
        {
            if (!mergeDuplicates || !frameDuplicateMap.ContainsKey(frameNum))
            {
                //  Calculate the x and y position of the frame's top-left
                //  pixel relative to the top-left of the final spritesheet
                int frameCol = (frameNum - fOffset) % columns;
                int frameRow = (frameNum - fOffset) / columns;

                //  Inject the pixel color data from the frame into the
                //  final spritesheet color data array
                Rgba32[] pixels = frameColorLookup[frameNum];

                for (int pixelNum = 0; pixelNum < pixels.Length; pixelNum++)
                {
                    int x = (pixelNum % Size.Width) + (frameCol * Size.Width);
                    int y = (pixelNum / Size.Width) + (frameRow * Size.Height);

                    //  Adjust for padding/spacing
                    x += borderPadding;
                    y += borderPadding;

                    if (spacing > 0)
                    {
                        if (frameCol > 0)
                        {
                            x += spacing * frameCol;
                        }

                        if (frameRow > 0)
                        {
                            y += spacing * frameRow;
                        }
                    }
                    Dictionary<string, int> a = new();
                    List<string> b = a.Keys.ToList();

                    if (innerPadding > 0)
                    {
                        x += innerPadding * (frameCol + 1);
                        y += innerPadding * (frameRow + 1);

                        if (frameCol > 0)
                        {
                            x += innerPadding * frameCol;
                        }

                        if (frameRow > 0)
                        {
                            y += innerPadding * frameRow;
                        }
                    }

                    int index = y * width + x;
                    sheetPixels[index] = pixels[pixelNum];
                }

                //  Now create the frame data
                Rectangle sourceRectangle = new(0, 0, Size.Width, Size.Height);
                sourceRectangle.X += borderPadding;
                sourceRectangle.Y += borderPadding;

                if (spacing > 0)
                {
                    if (frameCol > 0)
                    {
                        sourceRectangle.X += spacing * frameCol;
                    }

                    if (frameRow > 0)
                    {
                        sourceRectangle.Y += spacing * frameRow;
                    }
                }

                if (innerPadding > 0)
                {
                    sourceRectangle.X += innerPadding * (frameCol + 1);
                    sourceRectangle.Y += innerPadding * (frameRow + 1);

                    if (frameCol > 0)
                    {
                        sourceRectangle.X += innerPadding * frameCol;
                    }
                    if (frameRow > 0)
                    {
                        sourceRectangle.Y += innerPadding * frameRow;
                    }
                }

                SpritesheetFrame frame = new(sourceRectangle, Frames[frameNum].Duration);

                sheetFrames.Add(frame);
                originalToDuplicateFrameLookup.Add(frameNum, frame);
            }
            else
            {
                //  We are merging duplicates and it was detected that the
                //  current frame to process is a duplicate.  So we still
                //  need to add the spritesheet frame, but we need to make
                //  user the data is the same as the frame it's a duplicate
                //  of
                SpritesheetFrame original = originalToDuplicateFrameLookup[frameDuplicateMap[frameNum]];
                sheetFrames.Add(original.CreateCopy());
                fOffset++;
            }
        }

        //  Process Animations
        for (int tagNum = 0; tagNum < Tags.Count; tagNum++)
        {
            AsepriteTag tag = Tags[tagNum];
            string name = tag.Name;
            LoopDirection direction = tag.LoopDirection;
            List<SpritesheetFrame> aFrames = new(sheetFrames.GetRange(tag.From, tag.To - tag.From + 1));
            SpritesheetAnimation animation = new(aFrames, name, direction);
            sheetAnimations.Add(animation);
        }


        //  Process Slices

        //  Slice keys in Aseprite are defined with a frame value that
        //  indicates what frame the key starts on, but doesn't give
        //  what frame it ends on or is transformed on.  So we'll have to
        //  interpolate the slices per frame
        for (int sliceNum = 0; sliceNum < Slices.Count; sliceNum++)
        {
            AsepriteSlice slice = Slices[sliceNum];

            AsepriteSliceKey? lastKey = default;

            for (int keyNum = 0; keyNum < slice.Count; keyNum++)
            {
                AsepriteSliceKey key = slice[keyNum];

                string name = slice.Name;
                Rgba32 color = slice.UserData.Color ?? Rgba32.FromRGBA(0, 0, 255, 255);
                Rectangle bounds = key.Bounds;
                Rectangle? center = key.CenterBounds;
                Point? pivot = key.Pivot;
                SpritesheetSlice sheetSlice = new(bounds, center, pivot, name, color);

                if (lastKey is not null && lastKey.Frame < key.Frame)
                {
                    for (int offset = 1; offset < key.Frame - lastKey.Frame; offset++)
                    {
                        string interpolatedName = slice.Name;
                        Rgba32 interpolatedColor = slice.UserData.Color ?? Rgba32.FromRGBA(0, 0, 255, 255);
                        Rectangle interpolatedBounds = lastKey.Bounds;
                        Rectangle? interpolatedCenter = lastKey.CenterBounds;
                        Point? interpolatedPivot = lastKey.Pivot;

                        SpritesheetSlice interpolated = new(interpolatedBounds, interpolatedCenter, interpolatedPivot, interpolatedName, interpolatedColor);

                        sheetFrames[lastKey.Frame + offset].AddSlice(interpolated);
                    }
                }

                sheetFrames[key.Frame].AddSlice(sheetSlice);
                lastKey = key;
            }

            if (lastKey?.Frame < sheetFrames.Count - 1)
            {
                for (int offset = 1; offset < sheetFrames.Count - lastKey.Frame; offset++)
                {
                    string interpolatedName = slice.Name;
                    Rgba32 interpolatedColor = slice.UserData.Color ?? Rgba32.FromRGBA(0, 0, 255, 255);
                    Rectangle interpolatedBounds = lastKey.Bounds;
                    Rectangle? interpolatedCenter = lastKey.CenterBounds;
                    Point? interpolatedPivot = lastKey.Pivot;

                    SpritesheetSlice interpolated = new(interpolatedBounds, interpolatedCenter, interpolatedPivot, interpolatedName, interpolatedColor);

                    sheetFrames[lastKey.Frame + offset].AddSlice(interpolated);
                }
            }
        }

        return new Spritesheet(sheetSize, sheetFrames, sheetAnimations, sheetPixels);
    }

    public Image ExportSpritesheet(bool onlyVisibleLayers = true,
                                   bool mergeDuplicates = true,
                                   int borderPadding = 0,
                                   int spacing = 0,
                                   int innerPadding = 0)

    {
        //  Create spritesheet image
        List<Image> frames = new();

        for (int i = 0; i < Frames.Count; i++)
        {
            AsepriteFrame frame = Frames[i];
            frames.Add(frame.FlattenFrame(onlyVisibleLayers));
        }

        return Image.Pack(Size, frames, true, borderPadding, spacing, innerPadding);
    }

    public List<Animation> ExportAnimationData()
    {
        List<Animation> animations = new();

        //  Create the animation data
        for (int tNum = 0; tNum < Tags.Count; tNum++)
        {
            AsepriteTag tag = Tags[tNum];
            string name = tag.Name;
            LoopDirection direction = tag.LoopDirection;

            List<AnimationKey> keys = new();

            for (int start = tag.From; start <= tag.To; start++)
            {
                int fIndex = start;
                int duration = Frames[fIndex].Duration;

                keys.Add(new(fIndex, duration));
            }

            animations.Add(new(name, direction, keys));
        }

        return animations;
    }

    //  This is wrong. We're packing, then double packing.
    //
    //
    // public Image ExportTilesheets(bool onlyVisibleLayers = true,
    //                              bool mergeDuplicates = true,
    //                              int borderPadding = 0,
    //                              int spacing = 0,
    //                              int innerPadding = 0)

    // {
    //     //  Create spritesheet image
    //     List<Image> tiles = new();

    //     for (int tNum = 0; tNum < Tilesets.Count; tNum++)
    //     {
    //         AsepriteTileset tileSet = Tilesets[tNum];
    //         tiles.Add(tileSet.Export(mergeDuplicates, borderPadding, spacing, innerPadding));
    //     }

    //     return Image.Pack(Size, tiles, true, borderPadding, spacing, innerPadding);
    // }
}