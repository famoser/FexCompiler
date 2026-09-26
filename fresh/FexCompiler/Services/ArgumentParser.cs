using System.Diagnostics;
using System.Globalization;

namespace FexCompiler.Services;

public static class ArgumentParser
{
    public static ParseResult ParseArguments(string[] args)
    {
        string? filePath = null;
        string? author = null;
        string? title = null;

        for (var i = 0; i < args.Length; i++)
        {
            var argument = args[i];

            if (argument == "--author")
            {
                if (i + 1 >= args.Length)
                {
                    return ParseResult.Failure("Missing value for --author.");
                }

                author = args[++i];
                continue;
            }

            if (argument.StartsWith("--author=", StringComparison.Ordinal))
            {
                author = argument["--author=".Length..];
                continue;
            }

            if (argument == "--title")
            {
                if (i + 1 >= args.Length)
                {
                    return ParseResult.Failure("Missing value for --title.");
                }

                title = args[++i];
                continue;
            }

            if (argument.StartsWith("--title=", StringComparison.Ordinal))
            {
                title = argument["--title=".Length..];
                continue;
            }

            if (argument.StartsWith("--", StringComparison.Ordinal))
            {
                return ParseResult.Failure($"Unknown option: {argument}");
            }

            if (filePath is not null)
            {
                return ParseResult.Failure("Only one file path argument is supported.");
            }

            filePath = argument;
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return ParseResult.Failure("Missing file path argument.");
        }

        var fullFilePath = Path.GetFullPath(filePath);

        if (!File.Exists(fullFilePath))
        {
            return ParseResult.Failure($"File does not exist: {fullFilePath}");
        }

        author ??= GetGitConfigValue("user.name");

        if (string.IsNullOrWhiteSpace(author))
        {
            return ParseResult.Failure("Missing author. Provide --author or configure git config user.name.");
        }

        title ??= BuildDefaultTitle(fullFilePath);

        return ParseResult.SuccessResult(new CommandLineOptions(fullFilePath, author, title));
    }

    static string BuildDefaultTitle(string filePath)
    {
        var parentDirectory = Path.GetFileName(Path.GetDirectoryName(filePath));
        var fileName = Path.GetFileNameWithoutExtension(filePath);

        var formattedParentDirectory = ToTitleCase(parentDirectory);
        var formattedFileName = ToTitleCase(fileName);

        return $"{formattedParentDirectory} — {formattedFileName}";
    }

    static string ToTitleCase(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var words = value
            .Replace('-', ' ')
            .Replace('_', ' ')
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return string.Join(' ', words.Select(word =>
        {
            if (word.Length == 1)
            {
                return word.ToUpper(CultureInfo.CurrentCulture);
            }

            return char.ToUpper(word[0], CultureInfo.CurrentCulture) + word[1..].ToLower(CultureInfo.CurrentCulture);
        }));
    }

    static string? GetGitConfigValue(string key)
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "git",
                ArgumentList = { "config", "--get", key },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            });

            if (process is null)
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            return process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output)
                ? output
                : null;
        }
        catch
        {
            return null;
        }
    }

    public static void PrintUsage()
    {
        Console.Error.WriteLine("Usage:");
        Console.Error.WriteLine("  FexCompiler <file-path> [--author <author>] [--title <title>]");
    }

    public sealed record CommandLineOptions(string FilePath, string Author, string Title);

    public sealed record ParseResult(bool Success, CommandLineOptions? Options, string? Error)
    {
        public static ParseResult SuccessResult(CommandLineOptions options)
        {
            return new ParseResult(true, options, null);
        }

        public static ParseResult Failure(string error)
        {
            return new ParseResult(false, null, error);
        }
    }
}