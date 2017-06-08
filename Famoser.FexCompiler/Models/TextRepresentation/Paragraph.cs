using System.Collections.Generic;

namespace Famoser.FexCompiler.Models.TextRepresentation
{
    public class Paragraph
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
        public List<LineNode> LineNodes { get; }
    }
}
