using System.Collections.Generic;
using Famoser.FexCompiler.Models.TextRepresentation;

namespace Famoser.FexCompiler.Models
{
    public class Document
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public List<Section> Sections { get; set; }
    }
}
