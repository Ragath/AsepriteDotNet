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
using AsepriteDotNet.Core.Color;

namespace AsepriteDotNet.Core;

/// <summary>
///     Represents an animation tag.
/// </summary>
/// <param name="Name">
///     The name of this <see cref="Tag"/>.
/// </param>
/// <param name="From">
///     The index of the <see cref="Frame"/> that the animation for this
///     <see cref="Tag"/> starts on.
/// </param>
/// <param name="To">
///     The index of the <see cref="Frame"/> that the animation for this
///     <see cref="Tag"/> ends on.
/// </param>
/// <param name="Direction">
///     The direction the animation for this <see cref="Tag"/> plays.
/// </param>
/// <param name="Color">
///     The <see cref="Rgba32"/> color of this <see cref="Tag"/>.
/// </param>
public record Tag(string Name, int From, int To, LoopDirection Direction, Rgba32 Color);