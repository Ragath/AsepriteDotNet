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
    private string GetPath(string name)
    {
        return Path.Combine(Environment.CurrentDirectory, "Files", name);
    }

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

        AseTilemapLayer tilemapLayer = new(tileset, true, false, false, 0, BlendMode.Normal, 255, "Tilemap Layer");
        AseImageLayer invisibleLayer = new(false, false, false, 0, BlendMode.Normal, 255, "Invisible layer");
        AseImageLayer visibleLayer = new(true, false, false, 0, BlendMode.Normal, 255, "Visible Layer");
        AseImageLayer backgroundLayer = new(true, true, false, 0, BlendMode.Normal, 255, "Background Layer");
        List<AseLayer> aseLayers = new() { tilemapLayer, invisibleLayer, visibleLayer, backgroundLayer };


        List<AseTile> tilemapCelTiles = new()
        {
            new AseTile(0, 0, 0, 0),    //  Tile ID 0   
            new AseTile(1, 0, 0, 0),    //  Tile ID 1
            new AseTile(2, 0, 0, 0),    //  Tile ID 2
            new AseTile(3, 0, 0, 0)     //  TIle ID 3
        };

        Rgba32[] invisibleCelPixels = new Rgba32[]
        {
            asePalette[1], asePalette[1],
            asePalette[2], asePalette[2]
        };

        Rgba32[] visibleCelPixels = new Rgba32[]
        {
            asePalette[0], asePalette[1],
            asePalette[2], asePalette[3]
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

        List<AseCel> frame1Cels = new() { backgroundCel, visibleCel, invisibleCel, tilemapCel };


        AseFrame frame1 = new(frameSize, 100, frame1Cels);
        AseFrame frame2 = new(frameSize, 100, frame1Cels);
        List<AseFrame> aseFrames = new() { frame1, frame1 };

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
    public void AsepriteFile_ExportTagsTest()
    {
        Tag tag_1_2_reverse_black = new(nameof(tag_1_2_reverse_black), 0, 1, LoopDirection.Reverse, Rgba32.Black);
        Tag tag_3_4_pingpong_red = new(nameof(tag_3_4_pingpong_red), 2, 3, LoopDirection.PingPong, Rgba32.Red);

        List<Tag> tags = new() { tag_1_2_reverse_black, tag_3_4_pingpong_red };

        AsepriteFile aseFile = AsepriteFile.Load(GetPath("spritesheet-test.aseprite"));
        List<Tag> actual = aseFile.ExportTags();

        Assert.Equal(tags, aseFile.ExportTags());
    }

    [Fact]
    public void AsepriteFile_ExportSlicesTest()
    {
        //  These are the slices that should be interpolated from the one
        //  slice
        string sliceName = "slice";
        Slice frame0 = new(sliceName, 0, new Rectangle(0, 0, 2, 2), Rgba32.Blue, null, null);
        Slice frame1 = new(sliceName, 1, new Rectangle(0, 0, 2, 2), Rgba32.Blue, null, null);
        Slice frame2 = new(sliceName, 2, new Rectangle(0, 0, 1, 1), Rgba32.Blue, null, null);
        Slice frame3 = new(sliceName, 3, new Rectangle(0, 0, 1, 1), Rgba32.Blue, null, null);
        Slice frame4 = new(sliceName, 4, new Rectangle(1, 1, 1, 1), Rgba32.Blue, null, null);

        List<Slice> slices = new() { frame0, frame1, frame2, frame3, frame4 };

        AsepriteFile aseFile = AsepriteFile.Load(GetPath("spritesheet-test.aseprite"));
        List<Slice> actual = aseFile.ExportSlices();

        Assert.Equal(slices, actual);
    }

    [Fact]
    public void AsepriteFile_ExportSpritesheet_ImageTestOnly()
    {
        Rgba32 _ = Rgba32.Transparent;  //  Padding added from export
        Rgba32 t = Rgba32.Transparent;  //  Transparent pixel
        Rgba32 r = Rgba32.Red;          //  Red pixel
        Rgba32 g = Rgba32.Green;        //  Green pixel
        Rgba32 b = Rgba32.Blue;         //  Blue pixel
        Rgba32 w = Rgba32.White;        //  White pixel
        Rgba32 k = Rgba32.Black;        //  Black pixel

        AsepriteFile aseFile = AsepriteFile.Load(GetPath("spritesheet-test.aseprite"));

        //  Default parameters
        Spritesheet spritesheet = aseFile.ExportSpritesheet();
        Assert.Equal(new Size(4, 4), spritesheet.Size);

        Rgba32[] pixels = new Rgba32[]
        {
            t, r,    r, t,
            r, r,    r, r,

            r, r,    r, r,
            r, t,    t, r
        };

        Assert.Equal(pixels, spritesheet.Pixels);

        //  Merge Duplicates Off
        spritesheet = aseFile.ExportSpritesheet(mergeDuplicates: false);
        Assert.Equal(new Size(6, 4), spritesheet.Size);
        Assert.Equal(24, spritesheet.Pixels.Length);

        pixels = new Rgba32[]
        {
            t, r,    r, t,    r, r,
            r, r,    r, r,    r, t,

            r, r,    r, r,    t, t,
            t, r,    t, r,    t, t
        };

        Assert.Equal(pixels, spritesheet.Pixels);
        
        //  Include Invisible layers
        spritesheet = aseFile.ExportSpritesheet(onlyVisibleLayers: false);
        pixels = new Rgba32[]
        {
            w, k,    k, w,
            k, k,    k, k,

            k, k,    k, k,
            k, w,    w, k
        };

        Assert.Equal(pixels, spritesheet.Pixels);


        //  Include Background Layer
        spritesheet = aseFile.ExportSpritesheet(includeBackgroundLayer: true);
        pixels = new Rgba32[]
        {
            b, r,    r, b,
            r, r,    r, r,

            r, r,    r, r,
            r, b,    b, r
        };

        Assert.Equal(pixels, spritesheet.Pixels);

        //  Border padding = 2
        spritesheet = aseFile.ExportSpritesheet(borderPadding: 2);
        Assert.Equal(new Size(8, 8), spritesheet.Size);
        Assert.Equal(64, spritesheet.Pixels.Length);
        pixels = new Rgba32[]
        {
            _, _,    _, _,    _, _,    _, _,
            _, _,    _, _,    _, _,    _, _, 

            _, _,    t, r,    r, t,    _, _,
            _, _,    r, r,    r, r,    _, _,

            _, _,    r, r,    r, r,    _, _, 
            _, _,    r, t,    t, r,    _, _,
            
            _, _,    _, _,    _, _,    _, _, 
            _, _,    _, _,    _, _,    _, _, 
        };

        Assert.Equal(pixels, spritesheet.Pixels);

        //  Spacing 2
        spritesheet = aseFile.ExportSpritesheet(spacing: 2);
        Assert.Equal(new Size(6, 6), spritesheet.Size);
        Assert.Equal(36, spritesheet.Pixels.Length);

        pixels = new Rgba32[]
        {
            t, r,  _, _,  r, t,
            r, r,  _, _,  r, r,
            _, _,  _, _,  _, _,
            _, _,  _, _,  _, _,
            r, r,  _, _,  r, r,
            r, t,  _, _,  t, r
        };

        Assert.Equal(pixels, spritesheet.Pixels);

        //  Inner Padding = 2
        spritesheet = aseFile.ExportSpritesheet(innerPadding: 2);
        Assert.Equal(new Size(12, 12), spritesheet.Size);
        Assert.Equal(144, spritesheet.Pixels.Length);

        pixels = new Rgba32[]
        {
            _, _, _, _, _, _,    _, _, _, _, _, _,
            _, _, _, _, _, _,    _, _, _, _, _, _,
            _, _, t, r, _, _,    _, _, r, t, _, _, 
            _, _, r, r, _, _,    _, _, r, r, _, _,
            _, _, _, _, _, _,    _, _, _, _, _, _, 
            _, _, _, _, _, _,    _, _, _, _, _, _,

            _, _, _, _, _, _,    _, _, _, _, _, _,
            _, _, _, _, _, _,    _, _, _, _, _, _,
            _, _, r, r, _, _,    _, _, r, r, _, _,
            _, _, r, t, _, _,    _, _, t, r, _, _,
            _, _, _, _, _, _,    _, _, _, _, _, _,
            _, _, _, _, _, _,    _, _, _, _, _, _
        };

        Assert.Equal(pixels, spritesheet.Pixels);

        //  Border Padding = 2
        //  Spacing = 2
        //  Inner Padding = 2
        spritesheet = aseFile.ExportSpritesheet(borderPadding: 2, spacing: 2, innerPadding: 2);
        Assert.Equal(new Size(18, 18), spritesheet.Size);
        Assert.Equal(324, spritesheet.Pixels.Length);
        pixels = new Rgba32[]
        {
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _, 
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _, 
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _, 
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _, 
            _, _, _, _, t, r, _, _,    _, _,    _, _, r, t, _, _, _, _,
            _, _, _, _, r, r, _, _,    _, _,    _, _, r, r, _, _, _, _,
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _, 
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _,

            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _,
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _,

            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _,
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _,
            _, _, _, _, r, r, _, _,    _, _,    _, _, r, r, _, _, _, _,
            _, _, _, _, r, t, _, _,    _, _,    _, _, t, r, _, _, _, _,
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _,
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _,
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _, 
            _, _, _, _, _, _, _, _,    _, _,    _, _, _, _, _, _, _, _, 
        };

        Assert.Equal(pixels, spritesheet.Pixels);



    }
}
