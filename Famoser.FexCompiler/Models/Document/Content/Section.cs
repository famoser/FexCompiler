using System.Collections.Generic;
using Famoser.FexCompiler.Models.Document.TextRepresentation;

namespace Famoser.FexCompiler.Models.Document.Content
{
    public class Section
    {
        public Section(LineNode header)
        {
            Header = header;
            TextContent = new List<LineNode>();
            Content = new List<Section>();
        }

        public LineNode Header { get; private set; }
        public List<LineNode> TextContent { get; private set; }
        public List<Section> Content { get; private set; }
    }
}
