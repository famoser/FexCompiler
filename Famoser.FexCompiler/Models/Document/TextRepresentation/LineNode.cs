using System.Collections.Generic;

namespace Famoser.FexCompiler.Models.TextRepresentation
{
    public class LineNode
    {
        public LineNode(List<TextNode> nodes)
        {
            TextNodes = nodes;
        }

        public List<TextNode> TextNodes { get; private set; }
    }
}
