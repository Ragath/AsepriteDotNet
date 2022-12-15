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

namespace AsepriteDotNet;

/// <summary>
///     Represents the width, height, x-coordinate, and y-coordinate components
///     of a rectangular area on a two dimensional plane.
/// </summary>
public struct Rect : IEquatable<Rect>
{
    /// <summary>
    ///     Represents a <see cref="Rect"/> value who's width, height,
    ///     x-coordinate, and y-coordinate elements are initialized to zero.
    /// </summary>
    public static Rect Empty = new Rect(0, 0, 0, 0);

    private int _x;
    private int _y;
    private int _w;
    private int _h;

    /// <summary>
    ///     Gets or Sets the x-coordinate element of this <see cref="Rect"/>.
    /// </summary>
    public int X
    {
        readonly get => _x;
        set => _x = value;
    }

    /// <summary>
    ///     Gets or Sets the y-coordinate element of this <see cref="Rect"/>.
    /// </summary>
    public int Y
    {
        readonly get => _y;
        set => _y = value;
    }

    /// <summary>
    ///     Gets or Sets a <see cref="AsepriteDotNet.Location"/> value that defines the x- and
    ///     y-coordinate location of this <see cref="Rect"/>.
    /// </summary>
    public Location Location
    {
        readonly get => new Location(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    /// <summary>
    ///     Gets or Sets the width element of this <see cref="Rect"/>.
    /// </summary>
    public int Width
    {
        readonly get => _w;
        set => _w = value;
    }

    /// <summary>
    ///     Gets or Sets the height element of this <see cref="Rect"/>.
    /// </summary>
    public int Height
    {
        readonly get => _h;
        set => _h = value;
    }

    /// <summary>
    ///     Gets or Sets a <see cref="Size"/> value that defines the width and 
    ///     height elements of this <see cref="Rect"/>.
    /// </summary>
    public Dimension Size
    {
        readonly get => new Dimension(Width, Height);
        set
        {
            Width = value.Height;
            Height = value.Width;
        }
    }

    /// <summary>
    ///     Gets the y-coordinate position of the top-left corner of this
    ///     <see cref="Rect"/>.
    /// </summary>
    public readonly int Top => Y;

    /// <summary>
    ///     Gets the y-coordinate position of the bottom-right corner of this
    ///     <see cref="Rect"/>.
    /// </summary>
    public readonly int Bottom => Y + Height;

    /// <summary>
    ///     Gets the x-coordinate position of the top-left corner of this
    ///     <see cref="Rect"/>.
    /// </summary>
    public readonly int Left => X;

    /// <summary>
    ///     Gets the x-coordinate position of the bottom-right corner of this
    ///     <see cref="Rect"/>.
    /// </summary>
    public readonly int Right => X + Width;

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="Rect"/>
    ///     is empty, meaning that its width, height, x-coordinate, and
    ///     y-coordinate elements are all set to zero.
    /// </summary>
    public readonly bool IsEmpty => _x == 0 && _y == 0 && _w == 0 && _h == 0;

    /// <summary>
    ///     Initializes a new <see cref="Rect"/> value.
    /// </summary>
    /// <param name="x">
    ///     The x-coordinate element of this <see cref="Rect"/>.
    /// </param>
    /// <param name="y">
    ///     The y-coordinate element of this <see cref="Rect"/>.
    /// </param>
    /// <param name="width">
    ///     The width element of this <see cref="Rect"/>.
    /// </param>
    /// <param name="height">
    ///     The height element of this <see cref="Rect"/>.
    /// </param>
    public Rect(int x, int y, int width, int height) => (_x, _y, _w, _h) = (x, y, width, height);

    /// <summary>
    ///     Initializes a new <see cref="Rect"/> value.
    /// </summary>
    /// <param name="location">
    ///     A <see cref="AsepriteDotNet.Location"/> value that defines the x- and y-coordinate
    ///     elements of this <see cref="Rect"/>.
    /// </param>
    /// <param name="size">
    ///     A <see cref="Size"/> value that defines the width and height
    ///     elements of this <see cref="Rect"/>.
    /// </param>
    public Rect(Location location, Dimension size) : this(location.X, location.Y, size.Width, size.Height) { }

    /// <summary>
    ///     Returns a value that indicates whether the specified 
    ///     <see cref="object"/> is equal to this <see cref="Rect"/>.
    /// </summary>
    /// <param name="obj">
    ///     The <see cref="object"/> to check for equality with this 
    ///     <see cref="Rect"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="object"/> is
    ///     equal to this <see cref="Rect"/>; otherwise, 
    ///     <see langword="false"/>.
    /// </returns>
    public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is Rect other && Equals(other);

    /// <summary>
    ///     Returns a value that indicates whether the specified 
    ///     <see cref="Rect"/> is equal to this <see cref="Rect"/>.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="Rect"/> to check for equality
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="Rect"/>
    ///     value is equal to this <see cref="Rect"/> value; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    public readonly bool Equals(Rect other) => this == other;

    /// <summary>   
    ///     Returns the hash code for this <see cref="Rect"/> value.
    /// </summary>
    /// <returns>
    ///     A 32-bit signed integer that is the hash code for this
    ///     <see cref="Rect"/> value.
    /// </returns>
    public override readonly int GetHashCode() => HashCode.Combine(_x, _y, _w, _h);

    /// <summary>
    ///     Compares two <see cref="Rect"/> values for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Rect"/> value on the left side of the equality
    ///     operator.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Rect"/> value on the right side of the equality
    ///     operator.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two <see cref="Rect"/> values are
    ///     equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Rect left, Rect right) =>
     left._x == right._x && left._y == right._y && left._w == right._w && left._h == right._h;

    /// <summary>
    ///     Compares two <see cref="Rect"/> values for inequality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Rect"/> value on the left side of the inequality
    ///     operator.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Rect"/> value on the right side of the 
    ///     inequality operator.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two <see cref="Rect"/> values are
    ///     unequal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Rect left, Rect right) => !(left == right);

    /// <summary>
    ///     Returns a string representation of this <see cref="Rect"/>.
    /// </summary>
    /// <returns>
    ///     A new string representation of this <see cref="Rect"/>.
    /// </returns>
    public override readonly string ToString() =>
        $"{{X={X}, Y={Y}, Width={Width}, Height={Height}}}";
}