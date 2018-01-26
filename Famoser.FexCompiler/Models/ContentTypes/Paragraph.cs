using System.Collections.Generic;
using Famoser.FexCompiler.Models.ContentTypes.Base;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Models.ContentTypes
{
    public class Paragraph : Content
    {
        public Paragraph(LineNode header)
        {
            Header = header;
        }

        public LineNode Header { get; }

        public List<LineNode> Content { get; } = new List<LineNode>();
    }
}
