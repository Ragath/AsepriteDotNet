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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace AsepriteDotNet.Document;

/// <summary>
///     Represents a single frame cel of an Aseprite image.
/// </summary>
public abstract class Cel : IUserData
{
    /// <summary>
    ///     Gets the index of the <see cref="Layer"/> this <see cref="Cel"/>
    ///     is on.
    /// </summary>
    public int LayerIndex { get; internal set; }

    /// <summary>
    ///     Gets the top-left coordinate position of this <see cref="Cel"/>
    ///     relative to the bounds of the sprite.
    /// </summary>
    public Point Position { get; internal set; }

    /// <summary>
    ///     Gets the opacity level of this <see cref="Cel"/>,
    /// </summary>
    public int Opacity { get; internal set; }

    /// <summary>
    ///     Gets whether this <see cref="Cel"/> has extra data.
    /// </summary>
    [MemberNotNullWhen(true, nameof(ExtraData))]
    public bool HasExtraData => ExtraData is not null;

    /// <summary>
    ///     Gets the <see cref="CelExtra"/> instance that contains the extra
    ///     data for this <see cref="Cel"/>; or <see langword="null"/> if there
    ///     is no extra data provided.
    /// </summary>
    public CelExtra? ExtraData { get; internal set; } = default;

    /// <summary>
    ///     Gets the <see cref="UserData"/> set for this <see cref="Cel"/>.
    /// </summary>
    public UserData UserData { get; set; } = new();
}