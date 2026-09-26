using FexCompiler.Services;

var parseResult = ArgumentParser.ParseArguments(args);
if (!parseResult.Success)
{
    Console.Error.WriteLine(parseResult.Error);
    ArgumentParser.PrintUsage();
    return 1;
}

var options = parseResult.Options;

Console.WriteLine($"File: {options.FilePath}");
Console.WriteLine($"Author: {options.Author}");
Console.WriteLine($"Title: {options.Title}");

return 0;
