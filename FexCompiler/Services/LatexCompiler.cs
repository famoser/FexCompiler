using System.Diagnostics;
using FexCompiler.Models;

namespace FexCompiler.Services;

public static class LatexCompiler
{
    public static bool Compile(string latex, string filepath)
    {
        var workingDirectory = Path.GetDirectoryName(filepath);
        var filebasename = Path.Combine(workingDirectory ?? string.Empty, Path.GetFileNameWithoutExtension(filepath));

        var texFilepath = $"{filebasename}.tex";
        var logFilepath = $"{filebasename}.log";
        var auxFilepath = $"{filebasename}.aux";

        File.WriteAllText(texFilepath, latex);

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "pdflatex",
            Arguments = $"-interaction=nonstopmode \"{texFilepath}\"",
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);
        if (process is null)
        {
            return false;
        }

        var compilationFailed = false;

        while (!process.StandardOutput.EndOfStream)
        {
            var line = process.StandardOutput.ReadLine();
            if (line?.StartsWith('!') == true)
            {
                Console.WriteLine("Compilation failed");
                compilationFailed = true;
            }
        }

        process.WaitForExit();

        if (compilationFailed || process.ExitCode != 0)
        {
            return false;
        }

        File.Delete(texFilepath);
        foreach (var tempFilePaths in new [] { logFilepath, auxFilepath })
        {
            if (File.Exists(tempFilePaths))
            {
                File.Delete(tempFilePaths);
            }
        }

        return true;
    }
}