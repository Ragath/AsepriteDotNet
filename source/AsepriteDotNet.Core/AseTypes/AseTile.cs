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
namespace AsepriteDotNet.Core.AseTypes;

/// <summary>
///     Represents a single tile of a tilemap in an Aseprite image.
/// </summary>
/// <param name="ID">
///     The ID of this <see cref="AseTile"/>.
/// </param>
/// <param name="XFlip">
///     <para>
///         The X-Flip of this <see cref="AseTile"/>.
///     </para>
///     <para>
///         Tile X-Flip is not implemented in Aseprite yet, so this will always
///         be 0.
///         <see href="https://github.com/aseprite/aseprite/issues/3603"/>
///     </para>
/// </param>
/// <param name="YFlip">
///     <para>
///         The Y-Flip of this <see cref="AseTile"/>.
///     </para>
///     <para>
///         Tile Y-Flip is not implemented in Aseprite yet, so this will always
///         be 0.
///         <see href="https://github.com/aseprite/aseprite/issues/3603"/>
///     </para>
/// </param>
/// <param name="Rotate">
///     <para>
///         The rotation of this <see cref="AseTile"/>.
///     </para>
///     <para>
///         Tile rotation is not implemented in Aseprite yet, so this will 
///         alway be 0.
///         <see href="https://github.com/aseprite/aseprite/issues/3603"/>
///     </para>
/// </param>
public record AseTile(uint ID, uint XFlip, uint YFlip, uint Rotate);