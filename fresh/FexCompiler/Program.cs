using FexCompiler.Services;

var configuration = ArgumentParser.ParseArguments(args);
if (configuration is null)
{
    return 1;
}

var fileContent = await File.ReadAllLinesAsync(configuration.FilePath);
var lines = Parser.Parse(fileContent);
var document = DocumentService.Create(configuration.Title, configuration.Author, lines);
var latex = LatexService.GenerateLatex(document);



Console.WriteLine($"File: {configuration.FilePath}");
Console.WriteLine($"Author: {configuration.Author}");
Console.WriteLine($"Title: {configuration.Title}");

return 0;