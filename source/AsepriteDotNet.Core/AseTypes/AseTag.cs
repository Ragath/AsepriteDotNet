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
using AsepriteDotNet.Core.Color;

namespace AsepriteDotNet.Core.AseTypes;

/// <summary>
///     Represents an animation tag in an Aseprite image.
/// </summary>
/// <param name="From">
///     The index of the <see cref="AseFrame"/> this <see cref="AseTag"/> starts
///     on.
/// </param>
/// <param name="To">
///     The index of the <see cref="AseFrame"/> this <see cref="AseTag"/> ends
///     on.
/// </param>
/// <param name="Direction">    
///     The animation <see cref="LoopDirection"/> used by this 
///     <see cref="AseTag"/>.
/// </param>
/// <param name="Color">
///     <para>
///         The <see cref="Rgba32"/> color of this <see cref="AseTag"/>.
///     </para>
///     <para>
///         Starting with Aseprite 1.3, if this <see cref="AseTag"/> has
///         a <see cref="UserData"/> value, the color of this
///          <see cref="AseTag"/> should come from there instead.
///     </para>
/// </param>
/// <param name="Name">
///     The name of this <see cref="AseTag"/>.
/// </param>
/// <param name="UserData">
///     The custom <see cref="UserData"/> that was set for this 
///     <see cref="AseTag"/> within Aseprite; if any was set, otherwise,
///     <see langword="null"/>.
/// </param>
public record AseTag(int From, int To, LoopDirection Direction, Rgba32 Color, string Name, AseUserData? UserData = default);