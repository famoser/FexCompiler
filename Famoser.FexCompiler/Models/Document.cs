using System.Collections.Generic;
using Famoser.FexCompiler.Models.TextRepresentation;
using Famoser.FexCompiler.Models.TextRepresentation.Base;

namespace Famoser.FexCompiler.Models
{
    public class Document
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public List<Content> Content { get; set; }
        public DocumentStats DocumentStats { get; set; }
    }
}
