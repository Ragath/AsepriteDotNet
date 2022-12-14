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
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using AsepriteDotNet.Common;

namespace AsepriteDotNet.AsepriteTypes;

/// <summary>
///     Represents a group layer that contains child layers in an Aseprite
///     image.
/// </summary>
public sealed class AsepriteGroupLayer : AsepriteLayer, IEnumerable<AsepriteLayer>
{
    private readonly List<AsepriteLayer> _children;

    /// <summary>
    ///     Gets a read-only collection of all <see cref="AsepriteLayer"/> elements that
    ///     are children of this <see cref="AsepriteGroupLayer"/>.
    /// </summary>
    public ReadOnlyCollection<AsepriteLayer> Children { get; }

    internal AsepriteGroupLayer(bool isVisible, bool isBackground, bool isReference, int childLevel, BlendMode blend, int opacity, string name)
        : base(isVisible, isBackground, isReference, childLevel, blend, opacity, name)
    {
        _children = new();
        Children = _children.AsReadOnly();
    }

    internal void AddChild(AsepriteLayer layer)
    {
        Debug.Assert(layer is not AsepriteGroupLayer, "Cannot add group layer as child");
        _children.Add(layer);
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the child
    ///     <see cref="AsepriteLayer"/> elements in this <see cref="AsepriteGroupLayer"/>.
    /// </summary>
    /// <returns>
    ///     An enumerator that iterates through the child <see cref="AsepriteLayer"/>
    ///     elements in this <see cref="AsepriteGroupLayer"/>.
    /// </returns>
    public IEnumerator<AsepriteLayer> GetEnumerator() => _children.GetEnumerator();

    /// <summary>
    ///     Returns an enumerator that iterates through the child
    ///     <see cref="AsepriteLayer"/> elements in this <see cref="AsepriteGroupLayer"/>.
    /// </summary>
    /// <returns>
    ///     An enumerator that iterates through the child <see cref="AsepriteLayer"/>
    ///     elements in this <see cref="AsepriteGroupLayer"/>.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();
}