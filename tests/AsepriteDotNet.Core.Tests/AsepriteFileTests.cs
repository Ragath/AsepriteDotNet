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
using AsepriteDotNet.Core.AseTypes;
using AsepriteDotNet.Core.Color;
using AsepriteDotNet.Core.Primitives;

namespace AsepriteDotNet.Core.Tests;

public sealed class AsepriteFileTests
{
    private static AsepriteFile CreateTestFile()
    {
        //  2x2 frame size
        Size frameSize = new Size(2, 2);

        //  Palette with four colors; red, green, blue, and transparent
        AsePalette asePalette = new(0);
        asePalette.Resize(4);
        asePalette[0] = Rgba32.Transparent;
        asePalette[1] = Rgba32.Red;
        asePalette[2] = Rgba32.Green;
        asePalette[3] = Rgba32.Blue;

        //  Tilesets in aseprite are stored similar to a vertical spritesheet
        //  where each tile's pixels is on top of each other. Since the file
        //  we're building will use 2x2 frames, each tile will be 1x1, using the
        //  colors from the palette.
        Rgba32[] tilesetPixels = new Rgba32[]
        {
            asePalette[0],
            asePalette[1],
            asePalette[2],
            asePalette[3]
        };

        AseTileset tileset = new(0, 4, new Size(1, 1), "Tileset 0", tilesetPixels);
        List<AseTileset> aseTilesets = new() { tileset };

        //  Four layers in the following order from top-to-bottom
        //  tilemapLayer    - Tilemap layer using tileset 0
        //  invisibleLayer  - Image layer that is not visible
        //  visibleLayer    - Image layer that is visible
        //  backgroundLayer - Image layer that is visible, but is marked as background layer
        AseTilemapLayer tilemapLayer = new(tileset, true, false, false, 0, BlendMode.Normal, 255, "Tilemap Layer");
        AseImageLayer invisibleLayer = new(false, false, false, 0, BlendMode.Normal, 255, "Invisible layer");
        AseImageLayer visibleLayer = new(true, false, false, 0, BlendMode.Normal, 255, "Visible Layer");
        AseImageLayer backgroundLayer = new(true, true, false, 0, BlendMode.Normal, 255, "Background Layer");
        List<AseLayer> aseLayers = new() { tilemapLayer, invisibleLayer, visibleLayer, backgroundLayer };

        //  Four cels in the following order from top-to-bottom
        //  tilemapCel      - Tilemap cel that is on the tilemap layer
        //  invisibleCel    - Image cel that is on the invisible layer, pixels
        //                  - are all red
        //  visibleCel      - Image cel that is on the visible layer, pixels
        //                    are one of each color of the palette.
        //  backgroundCel   - Image cel that is on the background layer, pixels
        //                    are all blue

        List<AseTile> tilemapCelTiles = new()
        {
            new AseTile(0, 0, 0, 0),    //  Tile ID 0   
            new AseTile(1, 0, 0, 0),    //  Tile ID 1
            new AseTile(2, 0, 0, 0),    //  Tile ID 2
            new AseTile(3, 0, 0, 0)     //  TIle ID 3
        };

        Rgba32[] visibleCelPixels = new Rgba32[]
        {
            asePalette[0], asePalette[1],
            asePalette[2], asePalette[2]
        };

        Rgba32[] invisibleCelPixels = new Rgba32[]
        {
            asePalette[1], asePalette[1],
            asePalette[2], asePalette[2]
        };

        Rgba32[] backgroundCelPixels = new Rgba32[]
        {
            asePalette[3], asePalette[3],
            asePalette[3], asePalette[3]
        };

        AseTilemapCel tilemapCel = new(frameSize, tileset.TileSize, tilemapCelTiles, tilemapLayer, Point.Empty, 255);
        AseImageCel invisibleCel = new(frameSize, invisibleCelPixels, invisibleLayer, Point.Empty, 255);
        AseImageCel visibleCel = new(frameSize, visibleCelPixels, visibleLayer, Point.Empty, 255);
        AseImageCel backgroundCel = new(frameSize, backgroundCelPixels, backgroundLayer, Point.Empty, 255);

        List<AseCel> aseCels = new() { tilemapCel, invisibleCel, visibleCel, backgroundCel };

        //  A single frame containing all the cels created
        AseFrame frame = new(frameSize, 100, aseCels);
        List<AseFrame> aseFrames = new() { frame };

        //  Two tags
        //  noUserDataTag   - Tag with no user data
        //  userDataTag     - Tag with user data
        //
        //  * Both tags start and end on frame 0 since we only have one frame
        //  * Two different LoopDirection values are used, both are the 
        //    non-default value
        //  * Both have the color red for the tag color, but the one with user
        //    data has the color green, which is what should have priority for
        //    that tag when it is exported.
        AseTag noUserDataTag = new(0, 0, LoopDirection.Reverse, Rgba32.Red, "No User Data Tag");
        AseTag userDataTag = new(0, 0, LoopDirection.PingPong, Rgba32.Red, "User Data Tag", new AseUserData("Hello World", Rgba32.Green));
        List<AseTag> aseTags = new() { noUserDataTag, userDataTag };

        //  Two slices
        //  noUserDataSlice - Slice with no user data
        //  userDataSlice   - Slice with user data
        //
        //  * Both slices have keys for frame 0 and frame 3
        //  * Only the userDataSlice has nine patch and pivot data
        //  * The slice with user data has the color red for the user data
        //    color. This means the no user data slice should default to the
        //    blue color that Aseprite uses when exported.
        AseSliceKey noUserDataSlice_key1_frame0 = new(0, new Rectangle(1, 2, 3, 4));
        AseSliceKey noUserDataSlice_key1_frame3 = new(3, new Rectangle(5, 6, 7, 8));
        List<AseSliceKey> noUserDataSliceKeys = new() { noUserDataSlice_key1_frame0, noUserDataSlice_key1_frame3 };
        AseSlice noUserDataSlice = new(false, false, "No User Data Slice", noUserDataSliceKeys);

        AseSliceKey userDataSlice_key1_frame0 = new(0, new Rectangle(10, 20, 30, 40), new Rectangle(1, 2, 3, 4), new Point(5, 6));
        AseSliceKey userDataSlice_key2_frame3 = new(3, new Rectangle(50, 60, 70, 80), new Rectangle(5, 6, 7, 8), new Point(9, 10));
        List<AseSliceKey> userDataSliceKeys = new() { userDataSlice_key1_frame0, userDataSlice_key2_frame3 };
        AseSlice userDataSlice = new(true, true, "User Data Slice", userDataSliceKeys, new AseUserData(default, Rgba32.Red));

        List<AseSlice> aseSlices = new() { noUserDataSlice, userDataSlice };

        return new AsepriteFile(frameSize, asePalette, aseFrames, aseLayers, aseTags, aseSlices, aseTilesets);

    }

    [Fact]
    public void AsepriteFile_ConstructorTest()
    {
        AsepriteFile testFile = CreateTestFile();

        //  "Actual" values come from the values in the CreateTestFile method
        //  above
        Assert.Equal(new Size(2, 2), testFile.FrameSize);
        Assert.Equal(2, testFile.FrameWidth);
        Assert.Equal(2, testFile.FrameHeight);
        Assert.Equal(4, testFile.Palette.Count);
        Assert.Equal(4, testFile.PaletteCount);
        Assert.Equal(1, testFile.Frames.Count);
        Assert.Equal(1, testFile.FrameCount);
        Assert.Equal(4, testFile.Layers.Count);
        Assert.Equal(4, testFile.LayerCount);
        Assert.Equal(2, testFile.Tags.Count);
        Assert.Equal(2, testFile.TagCount);
        Assert.Equal(2, testFile.Slices.Count);
        Assert.Equal(2, testFile.SliceCount);
    }

    [Fact]
    public void AsepriteFile_ExportTagsTest()
    {
        //  The first tag has no user data, so the tag default color should
        //  be used
        Tag noUserDataTag = new("No User Data Tag", 0, 0, LoopDirection.Reverse, Rgba32.Red);

        //  The second tag has user data with a color of green, so the green
        //  color should be what's used
        Tag userDataTag = new("User Data Tag", 0, 0, LoopDirection.PingPong, Rgba32.Green);

        List<Tag> tags = new() { noUserDataTag, userDataTag };

        AsepriteFile aseFile = CreateTestFile();

        Assert.Equal(tags, aseFile.ExportTags());
    }

    [Fact]
    public void AsepriteFile_ExportSlicesTest()
    {
        //  These are the slices that should be interpolated for the first slice
        //  in the file (the no user data slice)
        string slice1_name = "No User Data Slice";
        Rectangle slice1_frame0_bounds = new Rectangle(1, 2, 3, 4);
        Rectangle slice1_frame3_bounds = new Rectangle(5, 6, 7, 8);
        Slice slice1_frame0 = new(slice1_name, 0, slice1_frame0_bounds, Rgba32.Blue, null, null);
        Slice slice1_frame1 = new(slice1_name, 1, slice1_frame0_bounds, Rgba32.Blue, null, null);
        Slice slice1_frame2 = new(slice1_name, 2, slice1_frame0_bounds, Rgba32.Blue, null, null);
        Slice slice1_frame3 = new(slice1_name, 3, slice1_frame3_bounds, Rgba32.Blue, null, null);

        //  These are the slices that should be interpolated for the second
        //  slice in the file (the user data slice)
        string slice2_name = "User Data Slice";
        Rectangle slice2_frame0_bounds = new(10, 20, 30, 40);
        Rectangle slice2_frame3_bounds = new(50, 60, 70, 80);
        Rectangle slice2_frame0_center_bounds = new(1, 2, 3, 4);
        Rectangle slice2_frame3_center_bounds = new(5, 6, 7, 8);
        Point slice2_frame0_pivot = new(5, 6);
        Point slice2_frame3_pivot = new(9, 10);
        Slice slice2_frame0 = new(slice2_name, 0, slice2_frame0_bounds, Rgba32.Red, slice2_frame0_center_bounds, slice2_frame0_pivot);
        Slice slice2_frame1 = new(slice2_name, 1, slice2_frame0_bounds, Rgba32.Red, slice2_frame0_center_bounds, slice2_frame0_pivot);
        Slice slice2_frame2 = new(slice2_name, 2, slice2_frame0_bounds, Rgba32.Red, slice2_frame0_center_bounds, slice2_frame0_pivot);
        Slice slice2_frame3 = new(slice2_name, 3, slice2_frame3_bounds, Rgba32.Red, slice2_frame3_center_bounds, slice2_frame3_pivot);

        //  Order is important. They should be in the order that they are in
        //  the list given to the Aseprite file
        List<Slice> slices = new()
        {
            slice1_frame0, slice1_frame1, slice1_frame2, slice1_frame3,
            slice2_frame0, slice2_frame1, slice2_frame2, slice2_frame3
        };

        AsepriteFile aseFile = CreateTestFile();

        List<Slice> actual = aseFile.ExportSlices();

        Assert.Equal(slices[0], actual[0]);
        Assert.Equal(slices[1], actual[1]);
        Assert.Equal(slices[2], actual[2]);
        Assert.Equal(slices[3], actual[3]);
        Assert.Equal(slices[4], actual[4]);
        Assert.Equal(slices[5], actual[5]);
        Assert.Equal(slices[6], actual[6]);
        Assert.Equal(slices[7], actual[7]);
        // Assert.Equal(slices, aseFile.ExportSlices());
    }

    public void AsepriteFile_ExportSpritesheetTest()
    {
        AsepriteFile aseFile = CreateTestFile();
        Spritesheet spriteSheet = aseFile.ExportSpritesheet();
    }
}