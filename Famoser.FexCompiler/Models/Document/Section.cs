using System.Collections.Generic;

namespace Famoser.FexCompiler.Models.Document
{
    public class Section
    {
        public Section(Content header)
        {
            Header = header;
            Content = new List<Content>();
            Children = new List<Section>();
        }

        public Content Header { get; private set; }
        public List<Content> Content { get; private set; }
        public List<Section> Children { get; private set; }
    }
}
