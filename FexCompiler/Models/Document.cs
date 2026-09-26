namespace FexCompiler.Models
{
    public sealed record Document(string Title, string Author, Statistics Statistics, List<Content> Content, List<Section> Sections);
    public sealed record Section(string Header, List<Content> Content, List<Section> Children);
    public sealed record Content(string Text, bool IsVerbatim);
    public sealed record Statistics(int LineCount, int WordCount, int CharacterCount);
}
