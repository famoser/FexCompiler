using System.Collections.Generic;
using Famoser.FexCompiler.Models.Content.Base;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Models.Content
{
    public class Paragraph : BaseContent
    {
        public Paragraph(LineNode header)
        {
            Header = header;
        }

        public LineNode Header { get; }
        public List<LineNode> Content { get; } = new List<LineNode>();
    }
}
