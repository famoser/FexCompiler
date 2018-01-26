using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Helpers.Interface;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.Content;
using Famoser.FexCompiler.Models.Content.Base;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Helpers
{
    public class ContentService : IProcessService<List<BaseContent>>
    {
        private readonly List<FexLine> _fexLines;

        public ContentService(List<FexLine> fexLines)
        {
            _fexLines = fexLines;
        }

        public List<BaseContent> Process()
        {

        }

        private static void FillSection(Section section, int startIndex, int stopIndex, int levelCorrection)
        {
            int i = startIndex;
            for (; i <= stopIndex; i++)
            {
                if (lines[i].IsCode)
                {
                    section.Content.Add(new Code(lines[i].Text));
                }
                else
                {
                    var nextLevel = i < stopIndex ? lines[i + 1].Level - levelCorrection : -100;

                    //if nextLevel is bigger we recursively call
                    if (nextLevel > 0)
                    {
                        var newStartIndex = i + 1;
                        var newStopIndex = stopIndex;
                        int j = newStartIndex;
                        for (; j <= stopIndex; j++)
                        {
                            if (lines[j].Level - levelCorrection == 0)
                            {
                                newStopIndex = j - 1;
                                break;
                            }
                        }

                        //wops we really need a new section
                        var newSection = new Section(section) { Title = GetLineNodeSimple(lines[i].Text, true) };
                        FillSection(lines, newSection, newStartIndex, newStopIndex, levelCorrection + 1);
                        i = newStopIndex;
                        section.Content.Add(newSection);
                    }
                    else
                    {
                        section.Content.Add(new Paragraph(GetLineNodeSimple(lines[i].Text, false)));
                    }
                }
            }
        }
        private static LineNode GetLineNodeSimple(string line, bool isTitle)
        {
            var res = new List<TextNode>();
            line = line.Trim();
            res.Add(new TextNode() { TextType = isTitle ? TextType.Bold : TextType.Normal, Text = ParseText(line) });
            return new LineNode(res);
        }

        private static string ParseText(string str)
        {
            return str.Trim();
        }

    }
}
