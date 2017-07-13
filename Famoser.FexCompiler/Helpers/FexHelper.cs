using System;
using System.Collections.Generic;
using System.Linq;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Models.TextRepresentation.Base;
using Newtonsoft.Json.Linq;

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
            FillSection(normalized, section, 0, normalized.Count - 1, 0);
            document.Content = section.Content;

            return document;
        }

        private static void FillSection(List<FexLine> lines, Section section, int startIndex, int stopIndex, int levelCorrection)
        {
            int i = startIndex;
            for (; i <= stopIndex; i++)
            {
                var nextLevel = i < stopIndex ? lines[i + 1].Level - levelCorrection : -100;

                //if nextLevel is bigger we recursively call
                if (nextLevel > 0)
                {
                    var newStartIndex = i + 1;
                    var newStopIndex = stopIndex;
                    int j = newStartIndex;
                    for (; j < stopIndex; j++)
                    {
                        if (lines[j].Level - levelCorrection == 0)
                        {
                            newStopIndex = j - 1;
                            break;
                        }
                    }

                    //wops we really need a new section
                    var newSection = new Section(section) { Title = GetLineNodeSimple(lines[i].Text) };
                    FillSection(lines, newSection, newStartIndex, newStopIndex, levelCorrection + 1);
                    i = newStopIndex;
                    section.Content.Add(newSection);
                }
                else
                {
                    section.Content.Add(new Paragraph(GetLineNodeSimple(lines[i].Text)));
                }
            }
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
            //maximum level 200
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
            return str.Trim();
        }
    }
}
