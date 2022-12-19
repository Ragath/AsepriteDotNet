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

public sealed class TagCollection
{
    private List<Tag> _tags = new();

    /// <summary>
    ///     Returns a new collection of all <see cref="Tag"/> elements from
    ///     this <see cref="TagCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    ///     The name of the tags.
    /// </param>
    /// <returns>
    ///     A new collection of all <see cref="Tag"/> elements from this
    ///     <see cref="TagCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </returns>
    public List<Tag> this[string name] => ByName(name);

    /// <summary>
    ///     The number of <see cref="Tag"/> elements in this 
    ///     <see cref="TagCollection"/>.
    /// </summary>
    public int Count => _tags.Count;

    internal TagCollection() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TagCollection"/>
    ///     class that contains elements copied from the given 
    ///     <paramref name="tags"/> collection.
    /// </summary>
    /// <param name="tags">
    ///     The collection who's elements are copied. 
    /// </param>
    public TagCollection(List<Tag> tags)
    {
        _tags = tags;
    }

    internal void Add(Tag tag) => _tags.Add(tag);

    /// <summary>
    ///     Returns a new collection of all <see cref="Tag"/> elements from
    ///     this <see cref="TagCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    ///     The name of the tags.
    /// </param>
    /// <returns>
    ///     A new collection of all <see cref="Tag"/> elements from this
    ///     <see cref="TagCollection"/> that have the specified
    ///     <paramref name="name"/>.
    /// </returns>
    public List<Tag> ByName(string name)
    {
        List<Tag> toReturn = new();

        for (int i = 0; i < _tags.Count; i++)
        {
            if (_tags[i].Name == name)
            {
                toReturn.Add(_tags[i]);
            }
        }

        return toReturn;
    }

    /// <summary>
    ///     Returns a new collection containing the name of each 
    ///     <see cref="Tag"/> element in this <see cref="TagCollection"/>.
    /// </summary>
    /// <returns>
    ///     A new collection containing the name of each <see cref="Tag"/>
    ///     element in this <see cref="TagCollection"/>.
    /// </returns>
    public List<string> TagNames()
    {
        List<string> toReturn = new();

        for (int i = 0; i < _tags.Count; i++)
        {
            toReturn.Add(_tags[i].Name);
        }

        return toReturn;
    }
}