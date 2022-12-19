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

using AsepriteDotNet.Core.Color;

namespace AsepriteDotNet.Core.AseTypes;

/// <summary>
///     Represents custom user data set that can be set for various components
///     within Aseprite.
/// </summary>
/// <param name="Text">
///     The custom text that was set for this <see cref="AseUserData"/>, if any
///     was set; otherwise, <see langword="null"/>.
/// </param>
/// <param name="Color">
///     The custom <see cref="Rgba32"/> color value that was set for this
///     <see cref="AseUserData"/>, if any was set; otherwise, 
///     <see langword="null"/>.
/// </param>
public record AseUserData(string? Text, Rgba32? Color)
{
    /// <summary>
    ///     Indicates whether this <see cref="AseUserData"/> has a
    ///     <see cref="Text"/> value.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Text))]
    public bool HasText => Text is not null;

    /// <summary>
    ///     Indicates whether this <see cref="AseUserData"/> has a
    ///     <see cref="Color"/> value.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Color))]
    public bool HasColor => Color is not null;
}