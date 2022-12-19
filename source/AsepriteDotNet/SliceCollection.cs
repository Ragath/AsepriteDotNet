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
namespace AsepriteDotNet;

public sealed class SliceCollection
{
    private List<Slice> _slices = new();

    /// <summary>
    ///     Returns a new collection of all <see cref="Slice"/> elements from
    ///     this <see cref="SliceCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    ///     The name of the slices.
    /// </param>
    /// <returns>
    ///     A new collection of all <see cref="Slice"/> elements from this
    ///     <see cref="SliceCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </returns>
    public List<Slice> this[string name] => ByName(name);

    /// <summary>
    ///     The number of <see cref="Slice"/> elements in this 
    ///     <see cref="SliceCollection"/>.
    /// </summary>
    public int Count => _slices.Count;

    internal SliceCollection() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SliceCollection"/>
    ///     class that contains elements copied from the given 
    ///     <paramref name="slices"/> collection.
    /// </summary>
    /// <param name="slices">
    ///     The collection who's elements are copied. 
    /// </param>
    public SliceCollection(List<Slice> slices)
    {
        _slices = slices;
    }

    internal void Add(Slice slice) => _slices.Add(slice);

    /// <summary>
    ///     Returns a new collection of all <see cref="Slice"/> elements from
    ///     this <see cref="SliceCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    ///     The name of the slices.
    /// </param>
    /// <returns>
    ///     A new collection of all <see cref="Slice"/> elements from this
    ///     <see cref="SliceCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </returns>
    public List<Slice> ByName(string name)
    {
        List<Slice> toReturn = new();

        for (int i = 0; i < _slices.Count; i++)
        {
            if (_slices[i].Name == name)
            {
                toReturn.Add(_slices[i]);
            }
        }

        return toReturn;
    }

    /// <summary>
    ///     Returns a new collection of all <see cref="Slice"/> elements from
    ///     this <see cref="SliceCollection"/> that are valid on the frame of
    ///     the specified <paramref name="frameIndex"/>.
    /// </summary>
    /// <param name="frameIndex">
    ///     The index of the frame.
    /// </param>
    /// <returns>
    ///     A new collection of all <see cref="Slice"/> elements from this
    ///     <see cref="SliceCollection"/> that are valid on the frame of the
    ///     specified <paramref name="frameIndex"/>.
    /// </returns>
    public List<Slice> ByFrame(int frameIndex)
    {
        List<Slice> toReturn = new();

        for (int i = 0; i < _slices.Count; i++)
        {
            if (_slices[i].FrameIndex == frameIndex)
            {
                toReturn.Add(_slices[i]);
            }
        }

        return toReturn;
    }

    /// <summary>
    ///     Returns a new collection containing the name of each 
    ///     <see cref="Slice"/> element in this <see cref="SliceCollection"/>.
    /// </summary>
    /// <returns>
    ///     A new collection containing the name of each <see cref="Slice"/>
    ///     element in this <see cref="SliceCollection"/>.
    /// </returns>
    public List<string> SliceNames()
    {
        List<string> toReturn = new();

        for (int i = 0; i < _slices.Count; i++)
        {
            toReturn.Add(_slices[i].Name);
        }

        return toReturn;
    }
}