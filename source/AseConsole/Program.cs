using System.Text.Json;

using AsepriteDotNet;
using AsepriteDotNet.Core;
using AsepriteDotNet.Core.Serialization;

string inPath = Path.Combine(Environment.CurrentDirectory, "adventurer.aseprite");
string outPath = Path.Combine(Environment.CurrentDirectory, "output.png");
string jsonOut = Path.Combine(Environment.CurrentDirectory, "adventurer.json");

AsepriteFile aseFile = AsepriteFile.Load(inPath);
Spritesheet spritesheet = aseFile.GenerateSpritesheet();
spritesheet.ExportAsPng(outPath);

JsonSerializerOptions options = new()
{
    WriteIndented = false,
    Converters =
    {
        new SpritesheetConverter()
    }
};

string json = JsonSerializer.Serialize(spritesheet, options);
File.WriteAllText(jsonOut, json);

Spritesheet? deserialized = JsonSerializer.Deserialize<Spritesheet>(json, options);
if (deserialized is not null)
{
    deserialized.ExportAsPng(outPath);
}
// Console.WriteLine(json);