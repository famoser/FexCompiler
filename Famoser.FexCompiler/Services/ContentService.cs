﻿using System.Collections.Generic;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.Document;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services
{
    public class ContentService : IProcessService<Section>
    {
        private readonly List<FexLine> _lines;

        public ContentService(List<FexLine> lines)
        {
            _lines = lines;
        }

        public Section Process()
        {
            var section = new Section(null);
            FillSection(section);
            return section;
        }

        private int FillSection(Section section, int start = 0, int rootLevel = 0)
        {
            for (var i = start; i < _lines.Count; i++)
            {
                var currentLine = _lines[i];
                if (currentLine.Level == rootLevel)
                {
                    //create new section
                    var newSection = new Section(GetLineNode(currentLine));
                    i++;

                    //fill section
                    for (; i < _lines.Count; i++)
                    {
                        var innerLine = _lines[i];
                        if (innerLine.Level == rootLevel + 1)
                        {
                            if (i + 1 < _lines.Count && _lines[i + 1].Level > rootLevel + 1)
                            {
                                //currentLine is a header of a new section, process this by recursive call
                                i = FillSection(newSection, i, rootLevel + 1);
                                i--;
                            }
                            else
                            {
                                //safe to insert, as currentLine is no header for sure
                                newSection.Content.Add(GetLineNode(innerLine));
                            }
                        }
                        else
                        {
                            //new section; restart loop
                            i--;
                            break;
                        }
                    }
                    section.Children.Add(newSection);
                }
                else
                {
                    return i;
                }
            }

            return _lines.Count;
        }

        private Content GetLineNode(FexLine line)
        {
            return line.IsCode ? Content.FromCode(line.Text) : Content.FromText(line.Text.Trim());
        }
    }
}
