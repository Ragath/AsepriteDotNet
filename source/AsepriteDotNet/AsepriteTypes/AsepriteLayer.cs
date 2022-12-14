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
namespace AsepriteDotNet.AsepriteTypes;

/// <summary>
///     Represents a layer in an Aseprite image.
/// </summary>
public abstract class AsepriteLayer : IAsepriteUserData
{
    /// <summary>
    ///     Gets whether this <see cref="AsepriteLayer"/> is visible.
    /// </summary>
    public bool IsVisible { get; }

    /// <summary>
    ///     Gets whether this <see cref="AsepriteLayer"/> is a background layer.
    /// </summary>
    public bool IsBackgroundLayer { get; }

    /// <summary>
    ///     Gets whether this <see cref="AsepriteLayer"/> is a reference layer.
    /// </summary>
    public bool IsReferenceLayer { get; }

    /// <summary>
    ///     Gets the child level of this <see cref="AsepriteLayer"/> relative to the
    ///     previous <see cref="AsepriteLayer"/>.
    /// </summary>
    public virtual int ChildLevel { get; }

    /// <summary>
    ///     Gets the <see cref="BlendMode"/> used by this <see cref="AsepriteLayer"/>.
    /// </summary>
    public BlendMode BlendMode { get; }

    /// <summary>
    ///     Gets the opacity level of this <see cref="AsepriteLayer"/>.
    /// </summary>
    public int Opacity { get; }

    /// <summary>
    ///     Gets the name of this <see cref="AsepriteLayer"/>.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the <see cref="UserData"/> of this <see cref="AsepriteLayer"/>.
    /// </summary>
    public AsepriteUserData UserData { get; internal set; } = new();

    internal AsepriteLayer(bool isVisible, bool isBackground, bool isReference, int childLevel, BlendMode blend, int opacity, string name)
    {
        IsVisible = isVisible;
        IsBackgroundLayer = isBackground;
        IsReferenceLayer = isReference;
        ChildLevel = childLevel;
        BlendMode = blend;
        Opacity = opacity;
        Name = name;
    }
}