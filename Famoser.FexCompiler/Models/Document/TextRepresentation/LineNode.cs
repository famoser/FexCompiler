using System.Collections.Generic;
using Famoser.FexCompiler.Models.Document.Content;

namespace Famoser.FexCompiler.Models.Document.TextRepresentation
{
    public class LineNode
    {
        public LineNode(List<TextNode> nodes)
        {
            TextNodes = nodes;
        }
        public LineNode(Code code)
        {
            CodeNode = code;
        }

        public List<TextNode> TextNodes { get; private set; }

        public Code CodeNode { get; private set; }
    }
}
