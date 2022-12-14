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

namespace AsepriteDotNet.Common;

/// <summary>
///     Represents an x- and y-coordinate location in a two dimensional plane.
/// </summary>
public struct Location : IEquatable<Location>
{

    /// <summary>
    ///     Represents a <see cref="Location"/> value who's x- and y-coordinate
    ///     elements are initialized to zero.
    /// </summary>
    public static readonly Location Empty = new Location(0, 0);

    private int _x;
    private int _y;

    /// <summary>
    ///     Gets the x-coordinate element of this <see cref="Location"/>.
    /// </summary>
    public int X
    {
        readonly get => _x;
        set => _x = value;
    }

    /// <summary>
    ///     Gets the y-coordinate element of this <see cref="Location"/>.
    /// </summary>
    public int Y
    {
        readonly get => _y;
        set => _y = value;
    }

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="Location"/> is
    ///     empty, meaning that its x- and y-coordinate elements are set to
    ///     zero.
    /// </summary>
    public readonly bool IsEmpty => _x == 0 && _y == 0;

    /// <summary>
    ///     Initializes a new <see cref="Location"/> value.
    /// </summary>
    /// <param name="x">
    ///     The x-coordinate element of this <see cref="Location"/>.
    /// </param>
    /// <param name="y">
    ///     The y-coordinate element of this <see cref="Location"/>.
    /// </param>
    public Location(int x, int y) => (_x, _y) = (x, y);

    /// <summary>
    ///     Returns a value that indicates whether the specified 
    ///     <see cref="object"/> is equal to this <see cref="Location"/>.
    /// </summary>
    /// <param name="obj">
    ///     The <see cref="object"/> to check for equality with this 
    ///     <see cref="Location"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="object"/> is
    ///     equal to this <see cref="Location"/>; otherwise, 
    ///     <see langword="false"/>.
    /// </returns>
    public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is Location other && Equals(other);

    /// <summary>
    ///     Returns a value that indicates whether the specified 
    ///     <see cref="Location"/> is equal to this <see cref="Location"/>.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="Location"/> to check for equality
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="Location"/> value
    ///     is equal to this <see cref="Location"/> value; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    public readonly bool Equals(Location other) => this == other;

    /// <summary>   
    ///     Returns the hash code for this <see cref="Location"/> value.
    /// </summary>
    /// <returns>
    ///     A 32-bit signed integer that is the hash code for this
    ///     <see cref="Location"/> value.
    /// </returns>
    public override readonly int GetHashCode() => HashCode.Combine(_x, _y);

    /// <summary>
    ///     Adds the x- and y-coordinate elements of two <see cref="Location"/>
    ///     values.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Location"/> value on the left side of the addition
    ///     operator.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Location"/> value on the right side fo the addition
    ///     operator.
    /// </param>
    /// <returns>
    ///     A new <see cref="Location"/> value who's x- and y-coordinate elements
    ///     are the sum of the two <see cref="Location"/> values given.
    /// </returns>
    public static Location operator +(Location left, Location right) => Add(left, right);

    /// <summary>
    ///     Subtracts the x- and y-coordinate elements of one
    ///     <see cref="Location"/> value from another.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Location"/> value on the left side of the subtraction
    ///     operator.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Location"/> value on the right side fo the subtraction
    ///     operator.
    /// </param>
    /// <returns>
    ///     A new <see cref="Location"/> value who's x- and y-coordinate elements
    ///     are the result of subtracting the x- and y-coordinate elements of the
    ///     <paramref name="right"/> <see cref="Location"/> from the x- and
    ///     y-coordinate elements of the <paramref name="left"/> 
    ///     <see cref="Location"/>.
    /// </returns>
    public static Location operator -(Location left, Location right) => Subtract(left, right);

    /// <summary>
    ///     Compares two <see cref="Location"/> values for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Location"/> value on the left side of the equality
    ///     operator.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Location"/> value on the right side of the equality
    ///     operator.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two <see cref="Location"/> values are
    ///     equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Location left, Location right) => left._x == right._x && left._y == right._y;

    /// <summary>
    ///     Compares two <see cref="Location"/> values for inequality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Location"/> value on the left side of the inequality
    ///     operator.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Location"/> value on the right side of the inequality
    ///     operator.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two <see cref="Location"/> values are
    ///     unequal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Location left, Location right) => !(left == right);

    /// <summary>
    ///     Adds the x- and y-coordinate elements of <paramref name="point2"/>
    ///     to the x- and y-coordinate elements of <paramref name="point1"/>.
    ///     The result is a new <see cref="Location"/> value where the x- and
    ///     y-coordinate elements are the result of the addition.
    ///     (point1 + point2)
    /// </summary>
    /// <param name="point1">
    ///     The <see cref="Location"/> value that will have the x- and y-coordinate
    ///     elements of <paramref name="point2"/> added to its x- and
    ///     y-coordinate elements.
    /// </param>
    /// <param name="point2">
    ///     The <see cref="Location"/> value who's x- and y-coordinate elements 
    ///     will be added to the x- and y-coordinate elements of 
    ///     <paramref name="point1"/>.
    /// </param>
    /// <returns>
    ///     A new <see cref="Location"/> value who's x- and y-coordinate elements
    ///     are  the result of adding the x- and y-coordinate elements of
    ///     <paramref name="point2"/> to the x- and y-coordinate elements of
    ///     <paramref name="point1"/>.
    /// </returns>
    public static Location Add(Location point1, Location point2) => new Location(unchecked(point1._x + point2._x), unchecked(point1._y + point2._y));

    /// <summary>
    ///     Subtracts the x- and y-coordinate elements of 
    ///     <paramref name="point2"/> from the x- and y-coordinate elements of
    ///     <paramref name="point1"/>.  The result is a new <see cref="Location"/>
    ///     value where the x- and y-coordinate elements are the result of the
    ///     subtraction.
    ///     (point1 - point2)
    /// </summary>
    /// <param name="point1">
    ///     The <see cref="Location"/> value that will have the x- and y-coordinate
    ///     elements of <paramref name="point2"/> subtracted from it's
    ///     x- and y-coordinate elements.
    /// </param>
    /// <param name="point2">
    ///     The <see cref="Location"/> value who's x- and y-coordinate elements
    ///     will be subtracted from the x- and y-coordinate elements of
    ///     <paramref name="point1"/>.
    /// </param>
    /// <returns>
    ///     A new <see cref="Location"/> value who's x- and y-coordinate elements
    ///     are  the result of subtracting the x- and y-coordinate elements of
    ///     <paramref name="point2"/> from the x- and y-coordinate elements of 
    ///     <paramref name="point1"/>.
    /// </returns>
    public static Location Subtract(Location point1, Location point2) => new Location(unchecked(point1._x - point2._x), unchecked(point1._y - point2._y));

    /// <summary>
    ///     Returns a string representation of this <see cref="Location"/>.
    /// </summary>
    /// <returns>
    ///     A new string representation of this <see cref="Location"/>.
    /// </returns>
    public override readonly string ToString() => $"{{X={X}, Y={Y}}}";
}