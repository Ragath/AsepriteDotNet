// /* -----------------------------------------------------------------------------
// Copyright 2022 Christopher Whitley

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ----------------------------------------------------------------------------- */
// using AsepriteDotNet.Color;
// using AsepriteDotNet.IO;
// using AsepriteDotNet.Primitives;

// namespace AsepriteDotNet;


// public sealed class Image : IEquatable<Image>
// {
//     public Size Size { get; set; }
//     public Rgba32[] Pixels { get; set; }
//     public List<Rectangle> Frames { get; set; }

//     internal Image(Size size, Rgba32[] pixels)
//     {
//         Size = size;
//         Pixels = pixels;
//         Frames = new() { new Rectangle(0, 0, size.Width, size.Height) };
//     }

//     internal Image(Size size, Rgba32[] pixels, List<Rectangle> frames)
//     {
//         Size = size;
//         Pixels = pixels;
//         Frames = frames;
//     }


//     public void ToPng(string path)
//     {
//         PngWriter.SaveTo(path, this);
//     }

//     internal static Image Pack(Size frameSize, List<Image> images, bool mergeDuplicates, int borderPadding, int spacing, int innerPadding)
//     {
//         int nFrames = images.Count;
//         List<Rectangle> sourceRects = new();
//         Dictionary<int, Rectangle> originalToDuplicateLookup = new();
//         Dictionary<int, int> frameDuplicateMap = new();

//         if (mergeDuplicates)
//         {
//             for (int i = 0; i < images.Count; i++)
//             {
//                 for (int d = 0; d < i; d++)
//                 {
//                     if (images[i] == images[d])
//                     {
//                         frameDuplicateMap.Add(i, d);
//                         nFrames--;
//                         break;
//                     }
//                 }
//             }
//         }

//         //  Determine the number of columns and ros needed to pack the frames
//         //  into the image
//         double sqrt = Math.Sqrt(nFrames);
//         int columns = (int)Math.Floor(sqrt);
//         if (Math.Abs(sqrt % 1) >= double.Epsilon)
//         {
//             columns++;
//         }

//         int rows = nFrames / columns;
//         if (nFrames % columns != 0)
//         {
//             rows++;
//         }

//         //  Determine the final width and height of hte image based on the
//         //  number of columns and rows, adjusting for padding and spacing
//         int width = (columns * frameSize.Width) +
//                     (borderPadding * 2) +
//                     (spacing * (columns - 1)) +
//                     (innerPadding * 2 * columns);

//         int height = (rows * frameSize.Height) +
//                      (borderPadding * 2) +
//                      (spacing * (columns - 1)) +
//                      (innerPadding * 2 * rows);

//         Size imageSize = new(width, height);

//         Rgba32[] imagePixels = new Rgba32[width * height];

//         int fOffset = 0;    //  Offset for when we detect merged frames

//         for (int fNum = 0; fNum < images.Count; fNum++)
//         {
//             if (!mergeDuplicates || !frameDuplicateMap.ContainsKey(fNum))
//             {
//                 //  Calculate the x- and y-coordinate position of the frame's
//                 //  top-left pixel relative to the top-left of the final image.
//                 int fCol = (fNum - fOffset) % columns;
//                 int fRow = (fNum - fOffset) / columns;

//                 //  Inject the pixel color data from the frame into the final
//                 //  image color array
//                 Rgba32[] fPixels = images[fNum].Pixels;

//                 for (int pNum = 0; pNum < fPixels.Length; pNum++)
//                 {
//                     int x = (pNum % frameSize.Width) + (fCol * frameSize.Width);
//                     int y = (pNum / frameSize.Width) + (fRow * frameSize.Height);

//                     //  Adjust x- and y-coordinate for any padding and/or
//                     //  spacing
//                     x += borderPadding +
//                          (spacing * fCol) +
//                          (innerPadding * (fCol + 1 + fCol));

//                     y += borderPadding +
//                          (spacing * fRow) +
//                          (innerPadding * (fRow + 1 + fRow));

//                     int index = y * width + x;
//                     imagePixels[index] = fPixels[pNum];
//                 }

//                 //  Now create the frame region
//                 Rectangle sourceRect = new(fCol * frameSize.Width, fRow * frameSize.Height, frameSize.Width, frameSize.Height);

//                 sourceRect.X += borderPadding +
//                                 (spacing * fCol) +
//                                 (innerPadding * (fCol + 1 + fCol));

//                 sourceRect.Y += borderPadding +
//                                 (spacing * fRow) +
//                                 (innerPadding * (fRow + 1 + fRow));

//                 sourceRects.Add(sourceRect);
//                 originalToDuplicateLookup.Add(fNum, sourceRect);
//             }
//             else
//             {
//                 //  We ar emerging duplicates and it was detected that the
//                 //  current frame to process is a duplicate.  So we still 
//                 //  need to add the source rect.
//                 Rectangle original = originalToDuplicateLookup[frameDuplicateMap[fNum]];
//                 sourceRects.Add(original);
//                 fOffset++;

//             }
//         }

//         return new Image(imageSize, imagePixels, sourceRects);
//     }

//     public override bool Equals(object? obj) => obj is Image other && Equals(other);
//     public bool Equals(Image? other) => this == other;

//     public static bool operator ==(Image? left, Image? right)
//     {
//         if (ReferenceEquals(left, right))
//         {
//             return true;
//         }

//         if (left is null || right is null)
//         {
//             return false;
//         }

//         if (left.Size != right.Size)
//         {
//             return false;
//         }

//         return left.Pixels.SequenceEqual(right.Pixels);
//     }

//     public static bool operator !=(Image left, Image right) => !(left == right);
// }