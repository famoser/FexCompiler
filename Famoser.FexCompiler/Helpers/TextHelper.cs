using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.ContentTypes;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Helpers
{
    public class TextHelper
    {
        public static void Improve(DocumentModel doc)
        {
            foreach (var content in doc.Content)
            {
                if (content is Section)
                    TryCollapse((Section) content, 0);
            }
        }

        private static bool TryCollapse(Section sec, int level)
        {
            if (sec.Content.Count(s => s is Paragraph) == sec.Content.Count)
            {
                //collapse paragraphs to this level
                if (level >= 2)
                {
                    return true;
                }
            }

            bool collapsedBefore = false;
            for (int i = 0; i < sec.Content.Count; i++)
            {
                var myCont = sec.Content[i] as Section;
                if (myCont != null)
                {
                    if (TryCollapse(myCont, level + 1))
                    {
                        //replace section with indented paragraph
                        var myLines = new List<LineNode>();
                        foreach (var content in myCont.Content)
                        {
                            var myPara = (Paragraph) content;
                            myLines.AddRange(myPara.LineNodes);
                        }
                        sec.Content[i] = new Paragraph(myLines);
                        sec.Content.Insert(i, new Paragraph(myCont.Title)
                        {
                            VerticalSpaceUnitsBefore = 1
                        });
                        i++;

                        collapsedBefore = true;
                    }
                    else if (collapsedBefore)
                    {
                        collapsedBefore = false;
                        var paragraph = sec.Content[i] as Paragraph;
                        if (paragraph != null)
                        {
                            paragraph.VerticalSpaceUnitsBefore = 2;
                        }
                    }
                }
            }

            return false;
        }
    }
}
