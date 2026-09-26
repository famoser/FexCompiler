using FexCompiler.Services;

var configuration = ArgumentParser.ParseArguments(args);
if (configuration is null)
{
    return 1;
}

var fileContent = await File.ReadAllLinesAsync(configuration.FilePath);
var lines = Parser.Parse(fileContent);
var document = DocumentService.Create(configuration.Title, configuration.Author, lines);
var latex = LatexGenerator.Generate(document);
var successful = LatexCompiler.Compile(latex, configuration.FilePath);
if (successful)
{
    Console.WriteLine($"Created pdf for {configuration.FilePath}");
}

return 0;