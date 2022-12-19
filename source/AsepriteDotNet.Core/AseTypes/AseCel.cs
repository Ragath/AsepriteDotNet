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
using AsepriteDotNet.Core.Primitives;

namespace AsepriteDotNet.Core.AseTypes;

/// <summary>
///     Represents a single frame cel in an Aseprite image.
/// </summary>
/// <param name="Layer">
///     The <see cref="AseLayer"/> that this <see cref="AseCel"/> is on.
/// </param>
/// <param name="Position">
///     The x- and y-coordinate position of the top-left corner of this
///     <see cref="AseCel"/> relative to the bounds of the 
///     <see cref="AseFrame"/> it is contained within.
/// </param>
/// <param name="Opacity">
///     The opacity level of this <see cref="AseCel"/>.
/// </param>
/// <param name="UserData">
///     The custom <see cref="UserData"/> that was set for this 
///     <see cref="AseCel"/> within Aseprite, if any was set; otherwise,
///     <see langword="null"/>.
/// </param>
public abstract record AseCel(AseLayer Layer, Point Position, int Opacity, AseUserData? UserData = default)
{
    /// <summary>
    ///     The x-coordinate position of the top-left corner of this
    ///     <see cref="AseCel"/> relative to the bounds of the 
    ///     <see cref="AseFrame"/> it is contained within
    /// </summary>
    public int X = Position.X;

    /// <summary>
    ///     The y-coordinate position of the top-left corner of this
    ///     <see cref="AseCel"/> relative to the bounds of the 
    ///     <see cref="AseFrame"/> it is contained within
    /// </summary>
    public int Y = Position.Y;
}