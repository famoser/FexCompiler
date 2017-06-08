using System.Collections.Generic;

namespace Famoser.FexCompiler.Models.TextRepresentation
{
    public class LineNode
    {
        public LineNode(List<TextNode> nodes)
        {
            TextNodes = nodes;
        }

        public LineNode(TextNode text)
        {
            TextNodes = new List<TextNode> {text};
        }

        public List<TextNode> TextNodes { get; }
    }
}
