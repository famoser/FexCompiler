using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Helpers
{
    public class TextHelper
    {
        public static void Improve(Document doc)
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
                if (level > 2)
                {
                    return true;
                }
            }

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
                        var para = new Paragraph(myLines);
                        sec.Content[i] = para;
                        sec.Content.Insert(i, new Paragraph(myCont.Title));
                        i++;
                    }
                }
            }

            return false;
        }
    }
}
