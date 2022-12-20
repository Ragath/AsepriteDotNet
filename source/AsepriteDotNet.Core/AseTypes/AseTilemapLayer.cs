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
///     Represents a tile map layer in an Aseprite image.
/// </summary>
/// <param name="Tileset">
///     The <see cref="AseTileset"/> used by this <see cref="AseTilemapLayer"/>.
/// </param>
/// <param name="IsVisible">
///     Whether this <see cref="AseTilemapLayer"/> is visible.
/// </param>
/// <param name="IsBackground">
///     Whether this <see cref="AseTilemapLayer"/> is a background layer.
/// </param>
/// <param name="IsReference">
///     Whether this <see cref="AseTilemapLayer"/> is a reference layer.
/// </param>
/// <param name="ChildLevel">
///     The child level of this <see cref="AseTilemapLayer"/> relative to the
///     previous <see cref="AseLayer"/>.
/// </param>
/// <param name="BlendMode">
///     The <see cref="AsepriteDotNet.BlendMode"/> used by this
///     <see cref="AseTilemapLayer"/>.
/// </param>
/// <param name="Opacity">
///     The opacity level of this <see cref="AseTilemapLayer"/>.
/// </param>
/// <param name="Name">
///     The name of this <see cref="AseTilemapLayer"/>.
/// </param>
/// <param name="UserData">
///     The custom <see cref="AseUserData"/> that was set for this
///     <see cref="AseTilemapLayer"/> in Aseprite, if any was set; otherwise,
///     <see langword="null"/>.
/// </param>
public record AseTilemapLayer(AseTileset Tileset, bool IsVisible, bool IsBackground, bool IsReference, int ChildLevel, BlendMode BlendMode, int Opacity, string Name, AseUserData? UserData = default)
    : AseLayer(IsVisible, IsBackground, IsReference, ChildLevel, BlendMode, Opacity, Name, UserData);