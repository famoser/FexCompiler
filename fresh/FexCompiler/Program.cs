using FexCompiler.Services;

var options = ArgumentParser.ParseArguments(args);
if (options is null)
{
    return 1;
}

Console.WriteLine($"File: {options.FilePath}");
Console.WriteLine($"Author: {options.Author}");
Console.WriteLine($"Title: {options.Title}");

return 0;