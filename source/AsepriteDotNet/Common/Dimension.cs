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
///     Represents the extent of something in terms of the width and height.
/// </summary>
public struct Dimension : IEquatable<Dimension>
{
    /// <summary>
    ///     Represents a <see cref="Dimension"/> value who's width and height
    ///     elements are initialized to zero.
    /// </summary>
    public static readonly Dimension Empty = new Dimension(0, 0);

    private int _w;
    private int _h;

    /// <summary>
    ///     Gets the width element of this <see cref="Dimension"/>.
    /// </summary>
    public int Width
    {
        readonly get => _w;
        set => _w = value;
    }

    /// <summary>
    ///     Gets the height element of this <see cref="Dimension"/>.
    /// </summary>
    public int Height
    {
        readonly get => _h;
        set => _h = value;
    }

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="Dimension"/> is
    ///     empty, meaning that its width and height elements are set to zero.
    /// </summary>
    public readonly bool IsEmpty => _w == 0 && _h == 0;

    /// <summary>
    ///     Initializes a new <see cref="Dimension"/> value.
    /// </summary>
    /// <param name="w">
    ///     The width element of this <see cref="Dimension"/>.
    /// </param>
    /// <param name="h">
    ///     The height element of this <see cref="Dimension"/>.
    /// </param>
    public Dimension(int w, int h) => (_w, _h) = (w, h);

    /// <summary>
    ///     Returns a value that indicates whether the specified 
    ///     <see cref="object"/> is equal to this <see cref="Dimension"/>.
    /// </summary>
    /// <param name="obj">
    ///     The <see cref="object"/> to check for equality with this 
    ///     <see cref="Dimension"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="object"/> is
    ///     equal to this <see cref="Dimension"/>; otherwise, 
    ///     <see langword="false"/>.
    /// </returns>
    public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is Dimension other && Equals(other);

    /// <summary>
    ///     Returns a value that indicates whether the specified 
    ///     <see cref="Dimension"/> is equal to this <see cref="Dimension"/>.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="Dimension"/> to check for equality
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="Dimension"/> value
    ///     is equal to this <see cref="Dimension"/> value; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    public readonly bool Equals(Dimension other) => this == other;

    /// <summary>   
    ///     Returns the hash code for this <see cref="Dimension"/> value.
    /// </summary>
    /// <returns>
    ///     A 32-bit signed integer that is the hash code for this
    ///     <see cref="Dimension"/> value.
    /// </returns>
    public override readonly int GetHashCode() => HashCode.Combine(_w, _h);

    /// <summary>
    ///     Adds the width and height elements of two <see cref="Dimension"/>
    ///     values.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Dimension"/> value on the left side of the addition
    ///     operator.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Dimension"/> value on the right side fo the addition
    ///     operator.
    /// </param>
    /// <returns>
    ///     A new <see cref="Dimension"/> value who's width and height elements are
    ///     the sum of the two <see cref="Dimension"/> values given.
    /// </returns>
    public static Dimension operator +(Dimension left, Dimension right) => Add(left, right);

    /// <summary>
    ///     Subtracts the width and height elements of one <see cref="Dimension"/> 
    ///     value from another.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Dimension"/> value on the left side of the subtraction
    ///     operator.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Dimension"/> value on the right side fo the subtraction
    ///     operator.
    /// </param>
    /// <returns>
    ///     A new <see cref="Dimension"/> value who's width and height  elements are 
    ///     the result of subtracting the width and height elements of the
    ///     <paramref name="right"/> <see cref="Dimension"/> from the width and
    ///     height elements of the <paramref name="left"/> <see cref="Dimension"/>.
    /// </returns>
    public static Dimension operator -(Dimension left, Dimension right) => Subtract(left, right);

    /// <summary>
    ///     Compares two <see cref="Dimension"/> values for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Dimension"/> value on the left side of the equality
    ///     operator.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Dimension"/> value on the right side of the equality
    ///     operator.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two <see cref="Dimension"/> values are
    ///     equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Dimension left, Dimension right) => left._w == right._w && left._h == right._h;

    /// <summary>
    ///     Compares two <see cref="Dimension"/> values for inequality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Dimension"/> value on the left side of the inequality
    ///     operator.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Dimension"/> value on the right side of the inequality
    ///     operator.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two <see cref="Dimension"/> values are
    ///     unequal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Dimension left, Dimension right) => !(left == right);

    /// <summary>
    ///     Adds the width and height elements of <paramref name="size2"/> to
    ///     the width and height elements of <paramref name="size1"/>. The
    ///     result is a new <see cref="Dimension"/> value where the width and height
    ///     elements are the result of the addition.
    ///     (size1 + size2)
    /// </summary>
    /// <param name="size1">
    ///     The <see cref="Dimension"/> value that will have the width and height
    ///     elements of <paramref name="size2"/> added to its width and height
    ///     elements.
    /// </param>
    /// <param name="size2">
    ///     The <see cref="Dimension"/> value who's width and height elements will
    ///     be added to the width and height elements of 
    ///     <paramref name="size1"/>.
    /// </param>
    /// <returns>
    ///     A new <see cref="Dimension"/> value who's width and height elements are 
    ///     the result of adding the width and height elements of
    ///     <paramref name="size2"/> to the width and height elements of
    ///     <paramref name="size1"/>.
    /// </returns>
    public static Dimension Add(Dimension size1, Dimension size2) => new Dimension(unchecked(size1._w + size2._w), unchecked(size1._h + size2._h));

    /// <summary>
    ///     Subtracts the width and height elements of <paramref name="size2"/>
    ///     from the width and height elements of <paramref name="size1"/>.  The
    ///     result is a new <see cref="Dimension"/> value where the width and height
    ///     elements are the result of the subtraction.
    ///     (size1 - size2)
    /// </summary>
    /// <param name="size1">
    ///     The <see cref="Dimension"/> value that will have the width and height
    ///     elements of <paramref name="size2"/> subtracted from it's width and
    ///     height elements.
    /// </param>
    /// <param name="size2">
    ///     The <see cref="Dimension"/> value who's width and height elements will
    ///     be subtracted from the width and height elements of
    ///     <paramref name="size1"/>.
    /// </param>
    /// <returns>
    ///     A new <see cref="Dimension"/> value who's width and height elements are 
    ///     the result of subtracting the width and height elements of
    ///     <paramref name="size2"/> from the width and height elements of 
    ///     <paramref name="size1"/>.
    /// </returns>
    public static Dimension Subtract(Dimension size1, Dimension size2) => new Dimension(unchecked(size1._w - size2._w), unchecked(size1._h - size2._h));

    /// <summary>
    ///     Returns a string representation of this <see cref="Dimension"/>.
    /// </summary>
    /// <returns>
    ///     A new string representation of this <see cref="Dimension"/>.
    /// </returns>
    public override readonly string ToString() => $"{{Width={Width}, Height={Height}}}";
}