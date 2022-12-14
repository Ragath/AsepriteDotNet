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
using System.Diagnostics.CodeAnalysis;
namespace AsepriteDotNet.AsepriteTypes;

/// <summary>
///     Represents custom user data set for a component in an Aseprite image.
/// </summary>
public sealed class AsepriteUserData
{
    /// <summary>
    ///     Gets whether <see cref="AsepriteUserData.Text"/> has been set for this
    ///     <see cref="AsepriteUserData"/>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Text))]
    public bool HasText => Text is not null;

    /// <summary>
    ///     Gets whether <see cref="AsepriteUserData.Color"/> has been set for this
    ///     <see cref="AsepriteUserData"/>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Color))]
    public bool HasColor => Color is not null;

    /// <summary>
    ///     Gets the text set for this <see cref="AsepriteUserData"/>, or 
    ///     <see langword="null"/> if none was set.
    /// </summary>
    public string? Text { get; internal set; } = default;

    /// <summary>
    ///     Gets the <see cref="Color"/> set for this <see cref="AsepriteUserData"/>, or 
    ///     <see langword="null"/> if none was set.
    /// </summary>
    public Rgba32? Color { get; internal set; } = default;

    internal AsepriteUserData() { }
}