using System.Collections.Generic;
using Famoser.FexCompiler.Models.ContentTypes.Base;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Models
{
    public class DocumentModel
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public List<Content> Content { get; set; }
        public string[] RawLines { get; set; }
        public DocumentStats DocumentStats { get; set; }
    }
}
