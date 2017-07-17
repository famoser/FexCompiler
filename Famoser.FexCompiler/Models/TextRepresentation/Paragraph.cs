using System.Collections.Generic;
using Famoser.FexCompiler.Models.TextRepresentation.Base;

namespace Famoser.FexCompiler.Models.TextRepresentation
{
    public class Paragraph : Content
    {
        public Paragraph(List<LineNode> nodes)
        {
            LineNodes = nodes;
        }

        public Paragraph(LineNode line)
        {
            LineNodes = new List<LineNode>() { line };
        }

        public bool IsEnumeration { get; set; } = false;
        public bool VerticalSpaceBefore { get; set; } = false;
        public List<LineNode> LineNodes { get; }
    }
}
