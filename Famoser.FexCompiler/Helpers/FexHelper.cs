using System;
using System.Collections.Generic;
using System.Linq;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Models.TextRepresentation.Base;

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
            document.Content = section.Content;

            return document;
        }

        private static int FillSection(List<FexLine> lines, Section section, int startIndex, int startLevel)
        {
            int i = startIndex;
            for (; i < lines.Count; i++)
            {
                //stop if we've reached the end of our responsibility
                if (lines[i].Level < startLevel)
                {
                    break;
                }

                //if next level is bigger we need to print it in another section, else we simply print it
                if (i < lines.Count - 2 && lines[i + 1].Level > startLevel)
                {
                    //try to simply indent
                    //keep two lists; one with the indented nodes
                    var lineNodes = new List<LineNode>();
                    //one with the contents
                    var contentNodes = new List<Content> { new Paragraph(GetLineNodeSimple(lines[i].Text)) };

                    //if bigger than threshhold we need a new section for sure
                    var hasBroken = false;
                    var threshHold = startLevel + 1;
                    //check if we really need a new section
                    int j = i + 1;
                    for (; j < lines.Count; j++)
                    {
                        if (lines[j].Level == startLevel)
                        {
                            //stop at the end of this level part
                            break;
                        }
                        if (lines[j].Level > threshHold || (lines[j].Level == threshHold && lines[j].Level < 2))
                        {
                            //wops we really need a new section
                            var newSection = new Section(section) { Title = GetLineNodeSimple(lines[i].Text) };
                            contentNodes.Add(newSection);
                            i = FillSection(lines, newSection, i + 1, startLevel + 1);
                            j = i;
                            lineNodes.Clear();
                        }
                        else if (lines[j].Level == threshHold)
                        {
                            //if same level; we do not indent
                            
                            //add lineNodes if any
                            if (lineNodes.Count > 0)
                            {
                                contentNodes.Add(new Paragraph(lineNodes)
                                {
                                    ExtraIndentation = true
                                });
                                contentNodes.Clear();
                            }
                            //add the current line
                            contentNodes.Add(new Paragraph(GetLineNodeSimple(lines[j].Text)));
                        }
                        else
                        {
                            //add the current line to the indented cache
                            lineNodes.Add(GetLineNodeSimple(lines[j].Text));
                        }
                    }

                    //add lineNodes if any
                    if (lineNodes.Count > 0)
                    {
                        contentNodes.Add(new Paragraph(lineNodes)
                        {
                            ExtraIndentation = true
                        });
                        contentNodes.Clear();
                    }

                    //add content nodes to section
                    foreach (var contentNode in contentNodes)
                    {
                        section.Content.Add(contentNode);
                    }

                    //correct i
                    i = j - 1;
                }
                else
                {
                    section.Content.Add(new Paragraph(GetLineNodeSimple(lines[i].Text)));
                }

            }
            return i;
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
