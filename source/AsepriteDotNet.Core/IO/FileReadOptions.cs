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
namespace AsepriteDotNet.Core.IO;

/// <summary>
///     Represents the options to adhere to when generating a new spritesheet.
/// </summary>
/// <param name="OnlyVisibleLayers">
///     <para>
///         Indicates whether only cels that are on visible layers should be
///         included in the generated spritesheet image.
///     </para>
///     <para>
///         The default is <see langword="true"/>.
///     </para>
/// </param>
/// <param name="MergeDuplicates">
///     <para>
///         When generating the spritesheet image, indicates whether duplicate
///         frames should be merged into a single frame for the final image.
///     </para>
///     <para>
///         When generating a tilesheet image, indicates whether duplicate
///         tiles should be merged into a single tile for the file image.
///     </para>
///     <para>
///         The default is <see langword="true"/>.
///     </para>
/// </param>
/// <param name="BorderPadding">
///     <para>
///         When generating the spritesheet image, indicates the amount of
///         transparent pixels to add between each frame and the edge of the
///         spritesheet.
///     </para>
///     <para>
///         When generating a tilesheet image, indicates the amount of
///         transparent pixels to add between each tile and the edge of the
///         tilesheet.
///     </para>
///     <para>
///         The default is 0.
///     </para>
/// </param>
/// <param name="Spacing">
///     <para>
///         When generating the spritesheet image, indicates the amount of
///         transparent pixels to add between each frame.
///     </para>
///     <para>
///         When generating a tilesheet image, indicates the amount of
///         transparent pixels to add between each tile.
///     </para>
///     <para>
///         The default is 0.
///     </para>
/// </param>
/// <param name="InnerPadding">
///     <para>
///         When generating the spritesheet image, indicates the amount of
///         transparent pixels to add to the inside of each frame's edge.
///     </para>
///     <para>
///         When generating a tilesheet image, indicates the amount of
///         transparent pixels to add to the inside of each tile's edge.
///     </para>
///     <para>
///         The default is 0.
///     </para>
/// </param>
public sealed record FileReadOptions(bool OnlyVisibleLayers = true,
                                     bool MergeDuplicates = true,
                                     int BorderPadding = 0,
                                     int Spacing = 0,
                                     int InnerPadding = 0)
{
    /// <summary>
    ///     Returns a new instance of the <see cref="FileReadOptions"/> record
    ///     with the default values set for all options.
    /// </summary>
    public static readonly FileReadOptions Default = new();
}
