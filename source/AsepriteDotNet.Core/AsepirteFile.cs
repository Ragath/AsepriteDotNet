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
using System.Text.Json.Serialization;

using AsepriteDotNet.Core.AseTypes;
using AsepriteDotNet.Core.Color;
using AsepriteDotNet.Core.IO;
using AsepriteDotNet.Core.Primitives;

namespace AsepriteDotNet.Core;

/// <summary>
///     Represents an imported Aseprite file.
/// </summary>
/// <param name="FrameSize">
///     The width and height extents, in pixels, of each <see cref="AseFrame"/>
///     in this <see cref="AsepriteFile"/>.
/// </param>
/// <param name="Palette">
///     The <see cref="AsePalette"/> that contains the <see cref="Rgba32"/>
///     color elements that represents all colors used in this
///     <see cref="AsepriteFile"/>.
/// </param>
/// <param name="Frames">
///     The collection of all <see cref="AseFrame"/> elements in this
///     <see cref="AsepriteFile"/>.
/// </param>
/// <param name="Layers">
///     The collection of all <see cref="AseLayer"/> elements in this
///     <see cref="AsepriteFile"/>.
/// </param>
/// <param name="Tags">
///     The collection of all <see cref="AseTag"/> elements in this
///     <see cref="AsepriteFile"/>.
/// </param>
/// <param name="Slices">
///     The collection of all <see cref="AseSlice"/> elements in this
///     <see cref="AsepriteFile"/>.
/// </param>
/// <param name="Tilesets">
///     The collection of all <see cref="AseTileset"/> elements in this
///     <see cref="AsepriteFile"/>.
/// </param>
public record AsepriteFile(Size FrameSize, AsePalette Palette, List<AseFrame> Frames, List<AseLayer> Layers, List<AseTag> Tags, List<AseSlice> Slices, List<AseTileset> Tilesets)
{
    /// <summary>
    ///     The width, in pixels, of each <see cref="AseFrame"/> in this
    ///     <see cref="AsepriteFile"/>.
    /// </summary>
    public int FrameWidth => FrameSize.Width;

    /// <summary>
    ///     The height, in pixels, of each <see cref="AseFrame"/> in this
    ///     <see cref="AsepriteFile"/>.
    /// </summary>
    public int FrameHeight => FrameSize.Height;

    /// <summary>
    ///     The total number of <see cref="AseFrame"/> elements in this
    ///     <see cref="AsepriteFile"/>.
    /// </summary>
    public int FrameCount => Frames.Count;

    /// <summary>
    ///     The total number of <see cref="AseLayer"/> elements in this
    ///     <see cref="AsepriteFile"/>.
    /// </summary>
    public int LayerCount => Layers.Count;

    /// <summary>
    ///     The total number of <see cref="AseTag"/> elements in this
    ///     <see cref="AsepriteFile"/>.
    /// </summary>
    public int TagCount => Tags.Count;

    /// <summary>
    ///     The total number of <see cref="AseSlice"/> elements in this
    ///     <see cref="AsepriteFile"/>.
    /// </summary>
    public int SliceCount => Slices.Count;

    /// <summary>
    ///     The total number of <see cref="Rgba32"/> color elements in the
    ///     <see cref="AsePalette"/> of this <see cref="AsepriteFile"/>.
    /// </summary>
    public int PaletteCount => Palette.Count;

    public Spritesheet ExportSpritesheet(bool onlyVisibleLayers = true, bool mergeDuplicates = true, int borderPadding = 0, int spacing = 0, int innerPadding = 0)
    {
        //  Flatten all frames
        List<Rgba32[]> flattenedFrames = new();
        for (int frameNum = 0; frameNum < FrameCount; frameNum++)
        {
            flattenedFrames.Add(Frames[frameNum].FlattenFrame(onlyVisibleLayers));
        }

        //  Pack frames
        List<Frame> frames = new();
        int frameCount = flattenedFrames.Count;
        Dictionary<int, Frame> originalToDuplicateLookup = new();
        Dictionary<int, int> duplicateMap = new();

        if (mergeDuplicates)
        {
            for (int i = 0; i < flattenedFrames.Count; i++)
            {
                for (int d = 0; d < i; d++)
                {
                    if (flattenedFrames[i].SequenceEqual(flattenedFrames[d]))
                    {
                        duplicateMap.Add(i, d);
                        frameCount--;
                        break;
                    }
                }
            }
        }

        //  Determine the number of columns and rows needed to pack frames into
        //  the image
        double sqrt = Math.Sqrt(frameCount);
        int columns = (int)Math.Floor(sqrt);
        if (Math.Abs(sqrt % 1) >= double.Epsilon)
        {
            columns++;
        }

        int rows = frameCount / columns;
        if (frameCount % columns != 0)
        {
            rows++;
        }

        //  Determine the final width and height of the image based ont the
        //  number of columns and rows, adjusting for padding and spacing
        int width = (columns * FrameWidth) +
                    (borderPadding * 2) +
                    (spacing * (columns - 1)) +
                    (innerPadding * 2 * columns);

        int height = (rows * FrameHeight) +
                     (borderPadding * 2) +
                     (spacing * (columns - 1)) +
                     (innerPadding * 2 * rows);


        Size spritesheetSize = new(width, height);
        Rgba32[] spritesheetPixels = new Rgba32[width * height];

        //  Offset for when we detect merged frames
        int frameOffset = 0;

        for (int frameNum = 0; frameNum < flattenedFrames.Count; frameNum++)
        {
            if (!mergeDuplicates || !duplicateMap.ContainsKey(frameNum))
            {
                //  Calculate the x- and y-coordinate position fo the frame's
                //  top-left pixel relative to the top-left of the final
                //  image
                int frameColumn = (frameNum - frameOffset) % columns;
                int frameRow = (frameNum - frameOffset) / columns;

                //  Inject the pixel color data from the frame into the final
                //  image color array
                Rgba32[] framePixels = flattenedFrames[frameNum];

                for (int pixelNum = 0; pixelNum < framePixels.Length; pixelNum++)
                {
                    int x = (pixelNum % FrameWidth) + (frameColumn * FrameWidth);
                    int y = (pixelNum / FrameWidth) + (frameRow * FrameHeight);

                    //  Adjust x- and y-coordinate for any padding and/or
                    //  spacing.
                    x += borderPadding +
                         (spacing * frameColumn) +
                         (innerPadding * (frameColumn + 1 + frameColumn));

                    y += borderPadding +
                         (spacing * frameRow) +
                         (innerPadding * (frameRow + 1 + frameRow));

                    int index = y * width + x;
                    spritesheetPixels[index] = framePixels[pixelNum];
                }

                //  Now create the frame data
                Rectangle sourceRectangle = new(frameColumn * FrameWidth, frameRow * FrameHeight, FrameWidth, FrameHeight);
                sourceRectangle.X += borderPadding +
                                     (spacing * frameColumn) +
                                     (innerPadding * (frameColumn + 1 + frameColumn));
                sourceRectangle.Y += borderPadding +
                                     (spacing * frameRow) +
                                     (innerPadding * (frameRow + 1 + frameRow));

                Frame frame = new(sourceRectangle, TimeSpan.FromMilliseconds(Frames[frameNum].Duration));
                frames.Add(frame);
                originalToDuplicateLookup.Add(frameNum, frame);

            }
            else
            {
                //  We are merging duplicates and it was detected that the
                //  current frame to process is a duplicate.  So we still
                //  need to add the frame data
                Frame original = originalToDuplicateLookup[duplicateMap[frameNum]];
                Frame frame = original;
                frames.Add(frame);
                frameOffset++;
            }
        }

        List<Tag> tags = ExportTags();
        List<Slice> slices = ExportSlices();

        return new Spritesheet(spritesheetSize, spritesheetPixels, frames, tags, slices);
    }

    public List<Tag> ExportTags()
    {
        List<Tag> tags = new();

        for (int i = 0; i < TagCount; i++)
        {
            AseTag aseTag = Tags[i];
            //  Prefer UserData color (Aseprite 1.3) over tag color 
            Rgba32 color = aseTag.UserData?.Color ?? aseTag.Color;

            Tag tag = new(aseTag.Name, aseTag.From, aseTag.To, aseTag.Direction, color);
            tags.Add(tag);
        }

        return tags;
    }

    public List<Slice> ExportSlices()
    {
        List<Slice> slices = new();

        //  Slice keys in Aseprite are defined with a frame index that indicates
        //  the frame the key starts on, but doesn't give a value for what frame
        //  it ends on or is transformed on.  So, we'll interpolate the keys to
        //  create the Slice elements

        for (int i = 0; i < SliceCount; i++)
        {
            AseSlice aseSlice = Slices[i];
            string name = aseSlice.Name;

            //  If no color defined in user data, use Aseprite default,
            //  which is just blue
            Rgba32 color = aseSlice.UserData?.Color ?? Rgba32.FromRGBA(0, 0, 255, 255);

            AseSliceKey? lastKey = default;

            for (int k = 0; k < aseSlice.KeyCount; k++)
            {
                AseSliceKey key = aseSlice[k];

                Slice slice = new(Name: name,
                                  FrameIndex: key.Frame,
                                  Bounds: key.Bounds,
                                  Color: color,
                                  CenterBounds: key.CenterBounds,
                                  Pivot: key.Pivot);

                //  Perform interpolation before adding the slice
                if (lastKey is not null && lastKey.Frame < key.Frame)
                {
                    for (int offset = 1; offset < key.Frame - lastKey.Frame; offset++)
                    {
                        Slice interpolatedSlice = new(Name: name,
                                                      FrameIndex: lastKey.Frame + offset,
                                                      Bounds: lastKey.Bounds,
                                                      Color: color,
                                                      CenterBounds: lastKey.CenterBounds,
                                                      Pivot: lastKey.Pivot);
                        slices.Add(interpolatedSlice);
                    }
                }

                slices.Add(slice);
                lastKey = key;
            }

            if (lastKey?.Frame < FrameCount)
            {
                for (int offset = 1; offset < FrameCount - lastKey.Frame; offset++)
                {
                    Slice interpolatedSlice = new(Name: name,
                                                  FrameIndex: lastKey.Frame + offset,
                                                  Bounds: lastKey.Bounds,
                                                  Color: color,
                                                  CenterBounds: lastKey.CenterBounds,
                                                  Pivot: lastKey.Pivot);

                    slices.Add(interpolatedSlice);
                }
            }
        }

        return slices;
    }

    /// <summary>
    ///     Loads an Aseprite file from disk at the specified 
    ///     <paramref name="path"/>.  The result is a new instance of the
    ///     <see cref="AsepriteFile"/> record initialized with the data read
    ///     from the Aseprite file.
    /// </summary>
    /// <param name="path">
    ///     The absolute path and file name, including extension, to the
    ///     Aseprite file to load from disk.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="AsepriteFile"/> record initialized
    ///     with the data read from the Aseprite file.
    /// </returns>
    public static AsepriteFile Load(string path) => AsepriteFileReader.ReadFile(path);
}