using System.Collections.Generic;
using Famoser.FexCompiler.Models.ContentTypes.Base;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Models.ContentTypes
{
    public class Section : Content
    {
        public Section(Section parent)
        {
            Parent = parent;
        }

        public LineNode Title { get; set; }
        public List<Content> Content { get; } = new List<Content>();
        public Section Parent { get; set; }
    }
}
