using System.Collections.Generic;

namespace Famoser.FexCompiler.Models.TextRepresentation
{
    public class Section
    {
        public Section(Section parent)
        {
            Parent = parent;
        }

        public string Title { get; set; }
        public List<Paragraph> Paragraphs { get; } = new List<Paragraph>();
        public List<Section> Sections { get; } = new List<Section>();
        public Section Parent { get; set; }
    }
}
