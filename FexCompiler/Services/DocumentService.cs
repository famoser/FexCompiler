using FexCompiler.Models;

namespace FexCompiler.Services;

public static class DocumentService
{
    public static Document Create(string title, string author, List<Line> lines)
    {
        var index = 0;
        var content = new List<Content>();
        var sections = new List<Section>();

        while (index < lines.Count)
        {
            var line = lines[index];

            if (HasChildren(lines, index))
            {
                sections.Add(CreateSection(lines, ref index));
            }
            else
            {
                content.Add(new Content(line.Text, line.IsVerbatim));
                index++;
            }
        }
        
        var characterCount = lines.Sum(s => s.Text.Length);
        var wordCount = lines
            .Sum(s1 => s1.Text.Split([" "], StringSplitOptions.RemoveEmptyEntries)
                .Count(s => s.Trim().Length > 0));
        var statistics = new Statistics(lines.Count, wordCount, characterCount);

        return new Document(title, author, statistics, content, sections);
    }

    private static Section CreateSection(List<Line> lines, ref int index)
    {
        var headerLine = lines[index];
        var section = new Section(headerLine.Text, [], []);
        var sectionLevel = headerLine.Level;

        index++;

        while (index < lines.Count && lines[index].Level > sectionLevel)
        {
            var line = lines[index];

            if (HasChildren(lines, index))
            {
                section.Children.Add(CreateSection(lines, ref index));
            }
            else
            {
                if (section.Children.Count > 0)
                {
                    Console.WriteLine("Found content after children in line " + index + ": " + line.Text);
                }
                
                section.Content.Add(new Content(line.Text, line.IsVerbatim));
                index++;
            }
        }

        return section;
    }

    private static bool HasChildren(List<Line> lines, int index)
    {
        return index + 1 < lines.Count && lines[index + 1].Level > lines[index].Level;
    }
}