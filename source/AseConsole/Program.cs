using AsepriteDotNet;

string inPath = Path.Combine(Environment.CurrentDirectory, "adventurer.aseprite");
string outPath = Path.Combine(Environment.CurrentDirectory, "output.png");

AsepriteFile aseFile = AsepriteFile.Load(inPath);
Image image = aseFile.Frames[10].FlattenFrame(true);
// Image image = aseFile.ToSS(true);
image.ToPng(outPath);