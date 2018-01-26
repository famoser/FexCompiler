using System.Collections.Generic;
using Famoser.FexCompiler.Models.Content.Base;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Models.Content
{
    public class Section : BaseContent
    {
        public Section(LineNode header)
        {
            Header = header;
        }

        public LineNode Header { get; }
        public List<LineNode> TextContent { get; } = new List<LineNode>();
        public List<BaseContent> Content { get; } = new List<BaseContent>();
    }
}
