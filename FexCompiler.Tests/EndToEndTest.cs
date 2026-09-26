using System.Diagnostics;

namespace FexCompiler.Tests;

public class EndToEndTest
{
    [Fact]
    public async Task FexCompiler_RunsSuccessfully_ForBasicFile()
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), $"fexcompiler-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectory);

        try
        {
            var inputFilePath = Path.Combine(tempDirectory, "input.fex");
            await File.WriteAllTextAsync(
                inputFilePath,
                """
                hello world
                ====

                test:
                    this is a test file
                    it is used to E2E test the compiler
                """);

            var compilerDllPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "FexCompiler.dll"));
            Assert.True(File.Exists(compilerDllPath), $"Could not find compiler at {compilerDllPath}");

            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            process.StartInfo.ArgumentList.Add(compilerDllPath);
            process.StartInfo.ArgumentList.Add(inputFilePath);
            process.StartInfo.ArgumentList.Add("--author");
            process.StartInfo.ArgumentList.Add("Test Author");
            process.StartInfo.ArgumentList.Add("--title");
            process.StartInfo.ArgumentList.Add("Test Title");

            process.Start();

            var standardOutput = await process.StandardOutput.ReadToEndAsync();
            var standardError = await process.StandardError.ReadToEndAsync();

            var exited = process.WaitForExit(TimeSpan.FromSeconds(30));
            Assert.True(exited, "FexCompiler did not finish within the timeout.");

            Assert.True(
                process.ExitCode == 0,
                $"""
                FexCompiler failed with exit code {process.ExitCode}.

                stdout:
                {standardOutput}

                stderr:
                {standardError}
                """);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }
}
