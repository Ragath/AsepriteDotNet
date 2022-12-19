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
using System.Collections.ObjectModel;

using AsepriteDotNet.Color;
using AsepriteDotNet.IO;
using AsepriteDotNet.Primitives;

namespace AsepriteDotNet;

/// <summary>
///     Represents an Aseprite file loaded from disk.
/// </summary>
public sealed class Aseprite
{
    /// <summary>
    ///     Gets the generated <see cref="Spritesheet"/> that was loaded from
    ///     the Aseprite file.
    /// </summary>
    public Spritesheet Spritesheet { get; }

    /// <summary>
    ///     Gets the <see cref="TilesheetCollection"/> that contains all 
    ///     <see cref="Tilesheet"/> elements that were loaded from the 
    ///     Aseprite file.
    /// </summary>
    public TilesheetCollection Tilesheets { get; }

    internal Aseprite(Spritesheet spritesheet, TilesheetCollection tilesheets) =>
        (Spritesheet, Tilesheets) = (spritesheet, tilesheets);

    /// <summary>
    ///     Loads the Aseprite from disk at the specified 
    ///     <paramref name="filePath"/>.  The result is a new instance of the 
    ///     <see cref="Aseprite"/> class initialized from the data read from the
    ///     Aseprite file.
    /// </summary>
    /// <param name="filePath">
    ///     The absolute file path to the Aseprite file to load.
    /// </param>
    /// <param name="options">
    ///     The options to adhere to when reading the Aseprite file. Affects
    ///     how the image data for spritesheets and tilesheets are generated.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Aseprite"/> class initialized from
    ///     the data read from the Aseprite file at the specified 
    ///     <paramref name="filePath"/>.
    /// </returns>
    public static Aseprite Load(string filePath, FileReadOptions? options)
    {
        options = options ?? FileReadOptions.Default;
        return AsepriteFileReader.ReadFile(filePath, options);
    }
}
