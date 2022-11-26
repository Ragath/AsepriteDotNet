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

public abstract class Cel : IUserData
{
    private int _opacity = 255;

    /// <summary>
    ///     Gets or Sets an <see cref="int"/> value that indicates the index of
    ///     the layer this <see cref="Cel"/> is on.
    /// </summary>
    public int LayerIndex { get; set; }

    /// <summary>
    ///     Gets or Sets a <see cref="Point"/> value that indicates the 
    ///     top-left coordinate position of this <see cref="Cel"/>.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    ///     Gets or Sets an <see cref="int"/> value that defines the opacity
    ///     level of this <see cref="Cel" />.  When set, the value will be
    ///     clamped in the inclusive range of 0 to 255.
    /// </summary>
    public int Opacity
    {
        get => _opacity;
        set
        {
            _opacity = Math.Clamp(value, 0, 255);
        }
    }

    /// <summary>
    ///     Gets or Sets a <see cref="bool"/> value that indicates if this
    ///     <see cref="Cel"/> instance has extra data.
    /// </summary>
    [MemberNotNullWhen(true, nameof(ExtraData))]
    public bool HasExtraData => ExtraData is not null;

    /// <summary>
    ///     Gets or Sets an instance of the  <see cref="CelExtra"/> class that
    ///     defines extra data values for this <see cref="Cel"/>.
    /// </summary>
    public CelExtra? ExtraData { get; set; } = default;

    /// <summary>
    ///     Gets or Sets the <see cref="UserData"/> for this <see cref="Cel"/>.
    /// </summary>
    public UserData UserData { get; set; } = new();
}