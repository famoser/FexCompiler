using System.Collections.Generic;
using Famoser.FexCompiler.Models.Content.Base;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Models.Content
{
    public class Section : BaseContent
    {
        public LineNode Header { get; }
        public LineNode Description { get; }
        public List<BaseContent> Content { get; } = new List<BaseContent>();
    }
}
