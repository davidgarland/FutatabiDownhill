using FutatabiDownhill.Properties;
using Octodiff.Core;
using Octodiff.Diagnostics;
using System.Net;

// Find the assetto dir, using the place 99% of people have it as the default and asking them otherwise.
string assettoDir = @"C:\Program Files (x86)\Steam\steamapps\common\assettocorsa";
while (!Directory.Exists(assettoDir))
{
    Console.WriteLine($"Could not find assettocorsa directory at '{assettoDir}'.");
    Console.Write("Please enter your assettocorsa directory: ");
    assettoDir = Console.ReadLine() ?? "";
}
Console.WriteLine($"Located assettocorsa directory at '{assettoDir}'.");

// Find the futatabi track. If they don't have it, pester them to install it and don't continue.
string futatabiDir = Path.Combine(assettoDir, @"content\tracks\futatabi");
string futatabiKn5Path = Path.Combine(futatabiDir, "futatabi.kn5");
while (!Directory.Exists(futatabiDir) || !File.Exists(futatabiKn5Path))
{
    Console.WriteLine($"Could not find a valid install of futatabi at '{futatabiDir}'.");
    Console.WriteLine(@"Please download and install it from:");
    Console.WriteLine("https://www.racedepartment.com/downloads/90s-golden-drift-spot-project-7-mountain-futatabi-%E5%86%8D%E5%BA%A6%E5%B1%B1.22990/");
    Console.Write("Press enter to continue once that's done.");
    Console.ReadLine();
}
Console.WriteLine($"Located futatabi at '{futatabiDir}'.");

// Stop if the user already has futatabi downhill.
if (File.Exists(Path.Combine(futatabiDir, "futatabi downhill.kn5")))
{
    Console.WriteLine("You already have futatabi downhill. Exiting..");
    Environment.Exit(0);
}

// 'Resources.futatabi_downhill' is the 'futatabi downhill.octodelta' file in the source repo.
// See https://stackoverflow.com/questions/14371567/embed-text-files-in-a-net-assembly-or-executable
using (var downhillDelta = new MemoryStream(Resources.futatabi_downhill_kn5))
using (var futatabi = new FileStream(futatabiKn5Path, FileMode.Open, FileAccess.Read, FileShare.Read))
using (var downhill = new FileStream(Path.Combine(futatabiDir, "futatabi downhill.kn5"), FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
{
    // Use octodiff to apply the patch to the default 'futatabi.kn5' to create a new file 'futatabi downhill.kn5'.
    var deltaApplier = new DeltaApplier { SkipHashCheck = false };
    deltaApplier.Apply
        ( futatabi
        , new BinaryDeltaReader(downhillDelta, new ConsoleProgressReporter())
        , downhill

        );
}

File.Copy(futatabiKn5Path, Path.Combine(futatabiDir, "futatabi uphill.kn5"));

File.Delete(Path.Combine(futatabiDir, "map.png"));

void MkIni(string name)
{
    var contents = $"[MODEL_0]\r\nFILE={name}.kn5\r\nPOSITION=0,0,0\r\nROTATION=0,0,0";
    File.WriteAllText(Path.Combine(futatabiDir, $"models_{name}.ini"), contents);
}
MkIni("futatabi uphill");
MkIni("futatabi downhill");

string MkSubdir(string path, string subpath)
{
    var newPath = Path.Combine(path, subpath);
    Directory.CreateDirectory(newPath);
    return newPath;
}

// futatabi/uphill
var uphillDir = MkSubdir(futatabiDir, "futatabi uphill");
var uphillData = Path.Combine(uphillDir, "data");
var futatabiData = Path.Combine(futatabiDir, "data");

File.Delete(Path.Combine(futatabiData, "map.ini"));
Directory.Move(futatabiData, uphillData);
Directory.Move(Path.Combine(futatabiDir, "ai"), Path.Combine(uphillDir, "ai"));
File.WriteAllText(Path.Combine(uphillData, "map.ini"), Resources.map_uphill_ini);

File.WriteAllBytes(Path.Combine(uphillDir, "map.png"), Resources.map_uphill);

// futatabi/downhill
var downhillDir = MkSubdir(futatabiDir, "futatabi downhill");
var downhillData = MkSubdir(downhillDir, "data");
foreach (var file in Directory.GetFiles(uphillData))
{
    var fileName = Path.GetFileName(file);
    if (fileName == "cameras.ini") continue;
    File.Copy(file, Path.Combine(downhillData, fileName));
}
File.WriteAllText(Path.Combine(downhillData, "cameras.ini"), Resources.cameras);
File.WriteAllText(Path.Combine(downhillData, "map.ini"), Resources.map_downhill_ini);

File.WriteAllBytes(Path.Combine(downhillDir, "map.png"), Resources.map_downhill);

var downhillAi = MkSubdir(downhillDir, "ai");
File.WriteAllBytes(Path.Combine(downhillAi, "fast_lane.ai"), Resources.fast_lane);

// futatabi/ui
var uiDir = MkSubdir(futatabiDir, "ui");

File.Delete(Path.Combine(uiDir, "ui_track.json"));
File.Delete(Path.Combine(uiDir, "outline.png"));

var uiUphillDir   = MkSubdir(uiDir, "futatabi uphill");
File.WriteAllBytes(Path.Combine(uiUphillDir, "ui_track.json"), Resources.uphill_ui_track);
File.Move(Path.Combine(uiDir, "preview.png"), Path.Combine(uiUphillDir, "preview.png"));
File.WriteAllBytes(Path.Combine(uiUphillDir, "outline.png"), Resources.outline);
File.WriteAllBytes(Path.Combine(uiUphillDir, "outline_cropped.png"), Resources.outline_cropped);

var uiDownhillDir = MkSubdir(uiDir, "futatabi downhill");
File.WriteAllBytes(Path.Combine(uiDownhillDir, "ui_track.json"), Resources.downhill_ui_track);
File.WriteAllBytes(Path.Combine(uiDownhillDir, "preview.png"), Resources.preview);
File.WriteAllBytes(Path.Combine(uiDownhillDir, "outline.png"), Resources.outline);
File.WriteAllBytes(Path.Combine(uiDownhillDir, "outline_cropped.png"), Resources.outline_cropped);

// futatabi/extension
var extensionDir = MkSubdir(futatabiDir, "extension");
File.WriteAllText(Path.Combine(extensionDir, "ext_config.ini"), Resources.ext_config);

