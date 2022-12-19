using AsepriteDotNet;
using AsepriteDotNet.IO;

string inPath = Path.Combine(Environment.CurrentDirectory, "adventurer.aseprite");
string outPath = Path.Combine(Environment.CurrentDirectory, "output.png");

FileReadOptions options = new()
{
    OnlyVisibleLayers = true,
    MergeDuplicates = true,
    BorderPadding = 0,
    Spacing = 0,
    InnerPadding = 0,
};

Aseprite ase = Aseprite.Load(inPath, options);

ase.Spritesheet.Image.ToPng(outPath);

System.Text.Json.JsonSerializerOptions jsonOptions = new()
{
    WriteIndented = true
};
string json = System.Text.Json.JsonSerializer.Serialize(ase.Spritesheet.Animations, jsonOptions);
Console.WriteLine(json);