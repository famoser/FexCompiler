using System;
using System.Collections.Generic;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Helpers
{
    public class FexHelper
    {
        public static Document ParseDocument(List<string> lines, string title, ConfigModel configModel)
        {
            var document = new Document
            {
                Author = configModel.Author,
                Title = title
            };

            int level = -2;
            Dictionary<int, List<LineNode>> enumerationNodes = new Dictionary<int, List<LineNode>>();
            Section section = new Section(null);
            var root = section;
            lines.Add("");
            for (var index = 0; index < lines.Count - 1; index++)
            {
                var fexLine = GetFexLine(lines[index]);
                var fexLine2 = GetFexLine(lines[index + 1]);

                //if the line is null don't change anything
                if (fexLine == null)
                    continue;

                //check the next line for irregularities 
                if (fexLine2 != null)
                {
                    if (fexLine2.Text.Length >= 3 && fexLine2.Text.Substring(0, 3) == "===")
                    {
                        //header
                        fexLine.Level = -1;
                        index++;
                    }

                    if (fexLine2.Text.Trim('=', ' ').Length == 0)
                    {

                    }
                }




                while (level > fexLine.Level)
                {
                    //go out
                    if (enumerationNodes.ContainsKey(level) && enumerationNodes[level].Count > 0)
                        section.Paragraphs.Add(new Paragraph(enumerationNodes[level]) { IsEnumeration = true });

                    enumerationNodes[level] = new List<LineNode>();
                    section = section.Parent;
                    level--;
                }

                if (level < fexLine.Level)
                {
                    //go in
                    var newSection = new Section(section);
                    section.Sections.Add(newSection);
                    section = newSection;

                    section.Title = ParseText(fexLine.Text);
                    level++;
                    continue;
                }

                if (level == fexLine.Level)
                {
                    if (fexLine.IsEnumeration)
                    {
                        enumerationNodes[level].Add(GetTextNodeSimple(fexLine.Text));
                    }
                    else
                    {
                        section.Paragraphs.Add(new Paragraph(GetTextNodeSimple(fexLine.Text)));
                    }
                }
            }
            document.Sections.AddRange(root.Sections);

            return document;
        }

        private static FexLine GetFexLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;
            var fexLine = new FexLine();

            //get level
            while (line.StartsWith("\t"))
            {
                line = line.Substring(1);
                fexLine.Level++;
            }
            line = line.TrimStart();

            //get enumeration
            if (line.StartsWith("-"))
            {
                fexLine.IsEnumeration = true;
                line = line.Substring(1).TrimStart();
            }

            fexLine.Text = line.Trim();
            return fexLine;
        }

        private static LineNode GetTextNodeSimple(string line)
        {
            var res = new List<TextNode>();
            if (line.Contains(":"))
            {
                var index = line.IndexOf(":", StringComparison.Ordinal);
                res.Add(new TextNode() { TextType = TextType.Bold, Text = ParseText(line.Substring(0, index + 1)) });
                res.Add(new TextNode() { TextType = TextType.Normal, Text = ParseText(line.Substring(index + 1)) });
            }
            else
            {
                res.Add(new TextNode() { TextType = TextType.Normal, Text = ParseText(line) });
            }
            return new LineNode(res);
        }

        private static string ParseText(string str)
        {
            var res = str.Trim();
            return res.Trim(':');
        }
    }
}
