using System;
using System.Collections.Generic;
using System.Linq;
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

            var normalized = NormalizeLines(lines);
            Section section = new Section(null);
            FillSection(normalized, section, 0, 0);
            document.Sections = section.Sections;

            return document;
        }

        private static int FillSection(List<FexLine> lines, Section section, int startIndex, int startLevel)
        {
            int i = startIndex;
            int endIndex = 0;

            //check for bigger level (then we need to start new section)
            bool startSection = false;
            endIndex = lines.Count - 1;
            for (; i < lines.Count; i++)
            {
                if (lines[i].Level > startLevel)
                {
                    startSection = true;
                }
                if (lines[i].Level < startLevel)
                {
                    endIndex = i;
                    break;
                }
            }

            if (!startSection)
            {
                //fill paragraphs
                for (i = startIndex; i < endIndex; i++)
                {
                    section.Paragraphs.Add(new Paragraph(GetLineNodeSimple(lines[i].Text)));
                }
            }
            else
            {
                //create new sections
                for (i = startIndex; i < endIndex; )
                {
                    var newSection = new Section(section) { Title = GetLineNodeSimple(lines[i].Text) };
                    section.Sections.Add(newSection);
                    i = FillSection(lines, newSection, i + 1, startLevel + 1);
                }

            }
            return endIndex;
        }

        private static List<FexLine> NormalizeLines(List<string> fileInput)
        {
            var res = new List<FexLine>();
            fileInput.Add("");
            for (var index = 0; index < fileInput.Count - 1; index++)
            {
                var line = fileInput[index];
                var line2 = fileInput[index + 1];
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var fexLine = new FexLine { Level = 2 };

                //get level
                while (line.StartsWith("\t"))
                {
                    line = line.Substring(1);
                    fexLine.Level++;
                }

                //if next line is only === and at least 3 then set level to -1
                if (line2.StartsWith("==="))
                {
                    fexLine.Level = 0;
                    //skip === line
                    index++;
                }
                if (line2.StartsWith("---"))
                {
                    fexLine.Level = 1;
                    //skip === line
                    index++;
                }
                fexLine.Text = line.Trim();
                res.Add(fexLine);
            }

            //normalize indexes
            //1: make all indexes come immediately after each other
            var shift = 0;
            var zeroStreak = 0;
            for (int i = 0; i < 200; i++)
            {
                var i2 = i;
                var iEnum = res.Where(s => s.Level == i2).ToList();
                if (shift != 0)
                {
                    foreach (var entry in iEnum)
                    {
                        entry.Level = entry.Level - shift;
                    }
                }
                if (iEnum.Count == 0)
                {
                    shift++;
                    if (zeroStreak++ == 5)
                    {
                        break;
                    }
                }
                else
                {
                    zeroStreak = 0;
                }
            }

            return res;
        }

        private static LineNode GetLineNodeSimple(string line)
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
