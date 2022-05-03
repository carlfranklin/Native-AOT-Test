using System.Diagnostics;

Console.WriteLine("Application Startup Timer. Enter path to .exe filename or drag it into the app");
string inputFileName = Console.ReadLine();
if (string.IsNullOrEmpty(inputFileName)) return;
// remove quotes
inputFileName = inputFileName.Replace("/q", "");

var process = new Process();
process.StartInfo.FileName = inputFileName;
process.StartInfo.UseShellExecute = false;


for (int i = 0; i < 3; i++)
{
    var startTime = DateTime.Now;
    process.Start();
    var elapsedMS = DateTime.Now.Subtract(startTime).TotalMilliseconds;
    Console.WriteLine($"Startup for IL: {elapsedMS}");
    Console.WriteLine("Press ENTER to reload");
    Console.ReadLine();
}

