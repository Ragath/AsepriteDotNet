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

public sealed class TilesheetCollection
{
    private List<Tilesheet> _tilesheets = new();

    /// <summary>
    ///     Returns a new collection of all <see cref="Tilesheet"/> elements 
    ///     from this <see cref="TilesheetCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    ///     The name of the tilesheets.
    /// </param>
    /// <returns>
    ///     A new collection of all <see cref="Tilesheet"/> elements from this
    ///     <see cref="TilesheetCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </returns>
    public List<Tilesheet> this[string name] => ByName(name);

    /// <summary>
    ///     The number of <see cref="Slice"/> elements in this 
    ///     <see cref="SliceCollection"/>.
    /// </summary>
    public int Count => _tilesheets.Count;

    internal TilesheetCollection() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TilesheetCollection"/>
    ///     class that contains elements copied from the given 
    ///     <paramref name="tilesheets"/> collection.
    /// </summary>
    /// <param name="tilesheets">
    ///     The collection who's elements are copied. 
    /// </param>
    public TilesheetCollection(List<Tilesheet> tilesheets)
    {
        _tilesheets = tilesheets;
    }

    internal void Add(Tilesheet tilesheet) => _tilesheets.Add(tilesheet);

    /// <summary>
    ///     Returns a new collection of all <see cref="Tilesheet"/> elements 
    ///     from this <see cref="TilesheetCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    ///     The name of the slices.
    /// </param>
    /// <returns>
    ///     A new collection of all <see cref="Tilesheet"/> elements from this
    ///     <see cref="TilesheetCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </returns>
    public List<Tilesheet> ByName(string name)
    {
        List<Tilesheet> toReturn = new();

        for (int i = 0; i < _tilesheets.Count; i++)
        {
            if (_tilesheets[i].Name == name)
            {
                toReturn.Add(_tilesheets[i]);
            }
        }

        return toReturn;
    }

    /// <summary>
    ///     Returns a new collection containing the name of each 
    ///     <see cref="Tilesheet"/> element in this 
    ///     <see cref="TilesheetCollection"/>.
    /// </summary>
    /// <returns>
    ///     A new collection containing the name of each <see cref="Tilesheet"/>
    ///     element in this <see cref="TilesheetCollection"/>.
    /// </returns>
    public List<string> TilesheetNames()
    {
        List<string> toReturn = new();

        for (int i = 0; i < _tilesheets.Count; i++)
        {
            toReturn.Add(_tilesheets[i].Name);
        }

        return toReturn;
    }
}