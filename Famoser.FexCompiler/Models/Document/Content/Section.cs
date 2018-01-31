using System.Collections.Generic;
using Famoser.FexCompiler.Models.Document.Content.Base;
using Famoser.FexCompiler.Models.Document.TextRepresentation;

namespace Famoser.FexCompiler.Models.Document.Content
{
    public class Section : BaseContent
    {
        public Section(LineNode header)
        {
            Header = header;
            TextContent = new List<LineNode>();
            Content = new List<BaseContent>();
        }

        public LineNode Header { get; private set; }
        public List<LineNode> TextContent { get; private set; }
        public List<BaseContent> Content { get; private set; }
    }
}
