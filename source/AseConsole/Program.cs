using AsepriteDotNet.Core;
using AsepriteDotNet.Core.Color;
using AsepriteDotNet.Core.IO;

class Image
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Rgba32[] Pixels { get; set; }
}

internal class Program
{
    private static void Main(string[] args)
    {
        string inPath = Path.Combine(Environment.CurrentDirectory, "adventurer.aseprite");
        string outPath = Path.Combine(Environment.CurrentDirectory, "output", "image.png");
        string multiOutPath = Path.Combine(Environment.CurrentDirectory, "output", "frame{0}.png");
        string jsonOut = Path.Combine(Environment.CurrentDirectory, "adventurer.json");

        AsepriteFile aseFile = AsepriteFile.Load(inPath);
        // aseFile.Frames[0].ExportAsPng(outPath);

    }

    private static Rgba32[] Trim(Rgba32[] colors, int width, int height, out int newWidth, out int newHeight)
    {
        // Assume the image has a width of `width` and a height of `height`, and the array of color values is called `colors`

        // First, we'll need to determine the top, bottom, left, and right edges of the non-transparent pixels in the image
        int top = height;
        int bottom = 0;
        int left = width;
        int right = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                Rgba32 color = colors[index];
                if (color.A > 0)
                {
                    top = Math.Min(top, y);
                    bottom = Math.Max(bottom, y);
                    left = Math.Min(left, x);
                    right = Math.Max(right, x);
                }
            }
        }

        // Now that we have the edges of the non-transparent pixels, we can calculate the new width and height of the trimmed image
        newWidth = right - left + 1;
        newHeight = bottom - top + 1;

        // Allocate a new array to hold the trimmed color values
        Rgba32[] trimmedColors = new Rgba32[newWidth * newHeight];

        // Copy the color values from the original array to the trimmed array, taking into account the new width and height and the position of the top-left pixel
        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                int oldIndex = (y + top) * width + (x + left);
                int newIndex = y * newWidth + x;
                trimmedColors[newIndex] = colors[oldIndex];
            }
        }

        // Finally, replace the original array with the trimmed array
        return trimmedColors;
    }

}

