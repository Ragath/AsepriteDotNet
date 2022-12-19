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
namespace AsepriteDotNet.Core.AseTypes;

public record AseSlice(bool IsNinePatch, bool HasPivot, string Name, List<AseSliceKey> Keys, AseUserData? UserData = default)
{
    /// <summary>
    ///     The total number of <see cref="AseSliceKey"/> elements in this
    ///     <see cref="AseSlice"/>.
    /// </summary>
    public int KeyCount => Keys.Count;

    /// <summary>
    ///     Returns the <see cref="AseSliceKey"/> element at the specified
    ///     <paramref name="index"/> from this <see cref="AseSlice"/>.
    /// </summary>
    /// <param name="index">
    ///     The index of the <see cref="AseSliceKey"/> within this
    ///     <see cref="AseSlice"/> to return.
    /// </param>
    /// <returns>
    ///     The <see cref="AseSliceKey"/> element at the specified
    ///     <paramref name="index"/> from this <see cref="AseSlice"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the specified <paramref name="index"/> is less than zero
    ///     or is greater than or equal to <see cref="KeyCount"/>.
    /// </exception>
    public AseSliceKey this[int index]
    {
        get
        {
            if (index < 0 || index >= KeyCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Keys[index];
        }
    }
}